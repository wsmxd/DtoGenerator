using System;
using System.Collections.Generic;
using System.Text;

// TinyMapper.Generator/Polyfills.cs

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    // 支持 record 和 init 属性 (解决 CS0518)
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit { }

    // 如果你使用了 required 关键字
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        public string FeatureName { get; }
        public bool IsOptional { get; set; }

        public const string RefStructs = "RefStructs";
        public const string RequiredMembers = "RequiredMembers";
    }
}
