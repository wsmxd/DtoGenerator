using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DtoGenerator.Source
{
    public static class SymbolExtensions
    {
        extension(INamedTypeSymbol type)
        {
            /// <summary>
            /// 获取类型的所有基类型（不包括自身），从直接基类开始到 System.Object
            /// </summary>
            public IEnumerable<INamedTypeSymbol> GetBaseTypes()
            {
                var current = type.BaseType;
                while (current != null)
                {
                    yield return current;
                    current = current.BaseType;
                }
            }

            /// <summary>
            /// 获取类型的所有基类型（包括自身）
            /// </summary>
            public IEnumerable<INamedTypeSymbol> GetBaseTypesAndThis()
            {
                yield return type;

                foreach (var baseType in type.GetBaseTypes())
                {
                    yield return baseType;
                }
            }


            public IEnumerable<IPropertySymbol> GetAllProperties(
                bool includeInterfaces = true,
                bool includeHidden = false)
            {
                var visited = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);

                // 获取所有基类型（包括自身）
                var allTypes = type.GetBaseTypesAndThis();

                if (includeInterfaces)
                {
                    allTypes = allTypes.Concat(type.AllInterfaces);
                }

                foreach (var currentType in allTypes)
                {
                    foreach (var property in currentType.GetMembers().OfType<IPropertySymbol>())
                    {
                        // 如果不需要隐藏的属性，检查是否被覆盖
                        if (!includeHidden)
                        {
                            // 检查属性是否被派生类中的属性覆盖
                            var isOverridden = type.GetBaseTypes()
                                .Any(baseType => baseType.GetMembers()
                                    .OfType<IPropertySymbol>()
                                    .Any(p =>
                                        SymbolEqualityComparer.Default.Equals(
                                            p.OverriddenProperty,
                                            property)));

                            if (isOverridden && !SymbolEqualityComparer.Default.Equals(currentType, type))
                            {
                                continue; // 跳过被覆盖的属性
                            }
                        }

                        if (visited.Add(property))
                        {
                            yield return property;
                        }
                    }
                }
            }

            public IEnumerable<IPropertySymbol> GetAllPropertiesOptimized()
            {
                var seen = new HashSet<string>();

                // 1. 首先处理当前类型和基类（从派生类到基类）
                var current = type;
                while (current != null)
                {
                    foreach (var property in current.GetMembers().OfType<IPropertySymbol>())
                    {
                        // 使用名称+类型来避免重名但不同类型的情况
                        var key = $"{property.Name}|{property.Type.ToDisplayString()}";

                        if (seen.Add(key))
                        {
                            yield return property;
                        }
                    }

                    current = current.BaseType;
                }

                // 2. 处理所有接口
                foreach (var interfaceType in type.AllInterfaces)
                {
                    foreach (var property in interfaceType.GetMembers().OfType<IPropertySymbol>())
                    {
                        var key = $"{property.Name}|{property.Type.ToDisplayString()}";

                        if (seen.Add(key))
                        {
                            yield return property;
                        }
                    }
                }
            }
        }
    }
}
