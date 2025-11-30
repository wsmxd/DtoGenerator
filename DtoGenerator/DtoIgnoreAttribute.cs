using System;

namespace DtoGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DtoIgnoreAttribute : Attribute { }
}
