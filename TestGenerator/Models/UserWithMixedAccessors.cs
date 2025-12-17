using DtoGenerator;
using System;

namespace TestGenerator.Models
{
    [GenerateDto]
    public class UserWithMixedAccessors
    {
        public int Id { get; set; }
        public string ReadOnlyName { get; private set; } = string.Empty;
        public string InitOnlyEmail { get; init; } = string.Empty;
        public string NormalProperty { get; set; } = string.Empty;

        public UserWithMixedAccessors()
        {
        }

        public UserWithMixedAccessors(int id, string name, string email, string normal)
        {
            Id = id;
            ReadOnlyName = name;
            InitOnlyEmail = email;
            NormalProperty = normal;
        }
    }
}
