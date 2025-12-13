using System;
#nullable enable
namespace DtoGenerator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DtoVirtualPropertyAttribute : Attribute
    {
        public string Name { get; }
        public Type Type { get; }
        public string? ValueExpression { get; }
        public string? ExpressionMemberName { get; set; }

        /// <summary>
        /// 定义一个 DTO 虚拟属性
        /// </summary>
        /// <param name="name">DTO 中的属性名 (如 "FullName")</param>
        /// <param name="type">属性类型 (如 typeof(string))</param>
        /// <param name="valueExpression">
        /// 赋值表达式。可以使用 'entity' 变量访问原对象。
        /// 例如: "entity.FirstName + \" \" + entity.LastName"
        /// </param>
        public DtoVirtualPropertyAttribute(string name, Type type, string? valueExpression = null)
        {
            Name = name;
            Type = type;
            ValueExpression = valueExpression;
        }
    }
}
