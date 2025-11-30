using DtoGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestGenerator.Models
{
    [GenerateDto(EnforceHooks =true)]
    [DtoVirtualProperty("FullName", typeof(string), "entity.FirstName + \" \" + entity.LastName")]
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        [DtoIgnore] public string? Password { get; set; }
    }
}
