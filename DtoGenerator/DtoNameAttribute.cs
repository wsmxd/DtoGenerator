using System;
using System.Collections.Generic;
using System.Text;

namespace DtoGenerator
{
    // 修复：添加 AttributeUsage，限制只能用于属性
    [AttributeUsage(AttributeTargets.Property)]
    public class DtoNameAttribute : Attribute
    {
        public string Name { get; }
        /// <summary>
        /// 指定 DTO 中的属性名
        /// </summary>
        /// <param name="name">DTO 中的属性名 (如 "Email")</param>
        public DtoNameAttribute(string name)
        {
            Name = name;
        }
    }
}
