using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

#nullable enable
namespace DtoGenerator.Source;

[Generator]
public class DtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "DtoGenerator.GenerateDtoAttribute",
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetDtoClassInfo(ctx))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, Execute);
    }

    private static DtoClassInfo? GetDtoClassInfo(GeneratorAttributeSyntaxContext context)
    {
        var sourceSymbol = (INamedTypeSymbol)context.TargetSymbol;

        // context.Attributes 只包含触发生成的 [GenerateDto]
        var attr = context.Attributes.FirstOrDefault(a => a.AttributeClass?.Name == "GenerateDtoAttribute");

        if (attr == null) return null;

        // 获取参数: EnforceHooks
        var enforceHooks = attr.NamedArguments
            .FirstOrDefault(kv => kv.Key == "EnforceHooks").Value.Value is bool a && a;

        // 获取参数: Suffix
        var suffix = attr.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? "Dto";

        // 获取参数: GenerateMapper
        var generateMapperArg = attr.NamedArguments.FirstOrDefault(kv => kv.Key == "GenerateMapper").Value.Value;
        var generateMapper = generateMapperArg is bool b ? b : true;

        var targetClassName = sourceSymbol.Name + suffix;
        var targetNamespace = sourceSymbol.ContainingNamespace.ToDisplayString();

        // 获取参数: UseSourceAccessModifier
        var useSourceAccessModifierArg = attr.NamedArguments
            .FirstOrDefault(kv => kv.Key == "UseSourceAccessModifier").Value.Value;
        var useSourceAccessModifier = useSourceAccessModifierArg is bool c && c;
        string targetAccessModifier;
        if (useSourceAccessModifier)
            targetAccessModifier = sourceSymbol.DeclaredAccessibility.ToString().ToLower();
        else
        {
            targetAccessModifier = "public";
        }

        var properties = new List<DtoPropertyInfo>();

        // 1. 获取普通属性 (来自 Entity 成员)
        foreach (var prop in sourceSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (prop.DeclaredAccessibility != Accessibility.Public || prop.IsStatic) continue;
            if (prop.GetAttributes().Any(a => a.AttributeClass?.Name == "DtoIgnoreAttribute")) continue;

            string targetName = prop.Name;
            var nameAttr = prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "DtoNameAttribute");
            if (nameAttr != null && nameAttr.ConstructorArguments.Length > 0)
            {
                targetName = nameAttr.ConstructorArguments[0].Value?.ToString() ?? prop.Name;
            }

            properties.Add(new DtoPropertyInfo(
                OriginalName: prop.Name,
                TargetName: targetName,
                TypeFullName: prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                IsRequired: prop.IsRequired,
                IsVirtual: false,
                ValueExpression: null
            ));
        }

        // 2. 获取虚拟属性 (来自 DtoVirtualPropertyAttribute)
        // 🔥 关键修复：context.Attributes 不包含其他 Attribute，必须从 sourceSymbol.GetAttributes() 获取
        var virtualAttrs = sourceSymbol.GetAttributes()
            .Where(a => a.AttributeClass?.Name == "DtoVirtualPropertyAttribute");

        foreach (var vAttr in virtualAttrs)
        {
            var args = vAttr.ConstructorArguments;
            // 构造函数参数顺序：[0]Name, [1]Type, [2]Expression
            if (args.Length == 3)
            {
                var name = args[0].Value?.ToString() ?? "Unknown";
                // args[1] 是 typeof(T)，在 Roslyn 中是 ITypeSymbol
                var typeSymbol = args[1].Value as ITypeSymbol;
                var expression = args[2].Value?.ToString() ?? "default";

                if (typeSymbol != null)
                {
                    properties.Add(new DtoPropertyInfo(
                        OriginalName: null,
                        TargetName: name,
                        TypeFullName: typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        IsRequired: false,
                        IsVirtual: true,
                        ValueExpression: expression
                    ));
                }
            }
        }

        return new DtoClassInfo(
            SourceNamespace: targetNamespace,
            SourceClassName: sourceSymbol.Name,
            TargetClassAccessModifier: targetAccessModifier,
            TargetClassName: targetClassName,
            GenerateMapper: generateMapper,
            EnforceHooks: enforceHooks,
            Properties: properties
        );
    }

    private static void Execute(SourceProductionContext context, DtoClassInfo? info)
    {
        if (info is null) return;

        // 生成属性定义
        var propBuilder = new StringBuilder();
        foreach (var p in info.Properties)
        {
            var requiredPrefix = p.IsRequired ? "required " : "";
            propBuilder.AppendLine($"        public {requiredPrefix}{p.TypeFullName} {p.TargetName} {{ get; set; }}");
        }

        // 生成 ToString 内容
        var toStringBody = string.Join(", ", info.Properties.Select(p => $"{p.TargetName} = {{{p.TargetName}}}"));

        // 1. 决定基类/接口声明
        var entityTypeFullName = $"global::{info.SourceNamespace}.{info.SourceClassName}";

        string interfaceDeclaration = "";
        if (info.EnforceHooks)
        {
            interfaceDeclaration = $" : DtoGenerator.IDtoMapperHooks<{entityTypeFullName}>";
        }

        string mapperCode = info.GenerateMapper ? GenerateMapperMethods(info, entityTypeFullName) : "";

        // 🔥 修复关键点：定义转义用的大括号变量
        // 在生成的代码中，我们需要输出 "{{" 来显示一个 "{"
        var ob = "{{";
        var cb = "}}";

        var code = $$"""
            // <auto-generated />
            #nullable enable
            using System;

            namespace {{info.SourceNamespace}}
            {
                {{info.TargetClassAccessModifier}} partial class {{info.TargetClassName}}{{interfaceDeclaration}}
                {
            {{propBuilder}}

                    public override string ToString()
                    {
                        // 修复：使用变量 ob 和 cb 来生成双大括号
                        return $"{{info.TargetClassName}} {{ob}} {{toStringBody}} {{cb}}";
                    }

            {{mapperCode}}
                }
            }
            """;

        context.AddSource($"{info.TargetClassName}.g.cs", SourceText.From(code, Encoding.UTF8));
    }

    private static string GenerateMapperMethods(DtoClassInfo info, string entityType)
    {
        var sb = new StringBuilder();

        // ---------------------------------------------------------
        // 关键逻辑分支：根据是否强制接口，生成不同的代码
        // ---------------------------------------------------------

        if (!info.EnforceHooks)
        {
            // === 模式 A：可选 (Partial Methods) ===
            sb.AppendLine($"        partial void OnDtoCreated({entityType} sourceEntity);");
            sb.AppendLine($"        partial void OnEntityCreated({entityType} targetEntity);");
        }
        // === 模式 B：强制 (Interface Methods) ===
        // 不需要生成方法声明，接口强制用户实现

        sb.AppendLine();

        // --- FromEntity ---
        sb.AppendLine($"        public static {info.TargetClassName} FromEntity({entityType} entity)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (entity is null) throw new ArgumentNullException(nameof(entity));");
        sb.AppendLine($"            var dto = new {info.TargetClassName}");
        sb.AppendLine("            {");
        foreach (var p in info.Properties)
        {
            if (p.IsVirtual)
                sb.AppendLine($"                {p.TargetName} = {p.ValueExpression},");
            else
                sb.AppendLine($"                {p.TargetName} = entity.{p.OriginalName},");
        }
        sb.AppendLine("            };");

        // 调用钩子
        sb.AppendLine("            dto.OnDtoCreated(entity);");

        sb.AppendLine("            return dto;");
        sb.AppendLine("        }");
        sb.AppendLine();

        // --- ToEntity ---
        sb.AppendLine($"        public {entityType} ToEntity()");
        sb.AppendLine("        {");
        sb.AppendLine($"            var entity = new {entityType}");
        sb.AppendLine("            {");
        foreach (var p in info.Properties)
        {
            if (p.IsVirtual) continue;
            sb.AppendLine($"                {p.OriginalName} = this.{p.TargetName},");
        }
        sb.AppendLine("            };");

        // 调用钩子
        sb.AppendLine("            this.OnEntityCreated(entity);");

        sb.AppendLine("            return entity;");
        sb.AppendLine("        }");

        return sb.ToString();
    }

    private record DtoClassInfo(
        string SourceNamespace,
        string SourceClassName,
        string TargetClassAccessModifier,
        string TargetClassName,
        bool GenerateMapper,
        bool EnforceHooks,
        List<DtoPropertyInfo> Properties
    );

    private record DtoPropertyInfo(
        string? OriginalName,
        string TargetName,
        string TypeFullName,
        bool IsRequired,
        bool IsVirtual,
        string? ValueExpression
    );
}