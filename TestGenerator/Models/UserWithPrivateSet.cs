using DtoGenerator;
using System;

namespace TestGenerator.Models
{
    [GenerateDto]
    public class UserWithPrivateSet
    {
        public int Id { get; set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
