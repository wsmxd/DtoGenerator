using DtoGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestGenerator.Models
{
    [GenerateDto(IncludeBaseProperties =true, UseSourceAccessModifier =true)]
    internal class User1 : User
    {
        public string? Department { get; set; }
    }
}
