using System;

namespace DtoGenerator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class GenerateDtoAttribute : Attribute
    {
        /// <summary>
        /// DTO 类名的后缀，默认 "Dto" (例如 User -> UserDto)
        /// </summary>
        public string Suffix { get; }

        /// <summary>
        /// 是否自动生成 ToEntity/FromEntity 映射方法 (默认 True)
        /// </summary>
        public bool GenerateMapper { get; set; } = true;

        /// <summary>
        /// 是否强制实现 IDtoMapperHooks 接口。
        /// 如果为 true，编译时必须手动实现 OnDtoCreated 和 OnEntityCreated，否则报错。
        /// </summary>
        public bool EnforceHooks { get; set; } = false;

        /// <summary>
        /// 是否使用源类的访问修饰符来生成 DTO 类。
        /// 默认为false，即生成的 DTO 类为 public。
        /// </summary>
        public bool UseSourceAccessModifier { get; set; } = false;

        /// <summary>
        /// 是否包含基类的公共属性（默认 false）
        /// </summary>
        public bool IncludeBaseProperties { get; set; } = false;

        public GenerateDtoAttribute(string suffix = "Dto")
        {
            Suffix = suffix;
        }
    }
}
