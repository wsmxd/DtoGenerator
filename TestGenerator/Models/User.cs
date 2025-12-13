using DtoGenerator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace TestGenerator.Models
{
    [GenerateDto(EnforceHooks = true, UseSourceAccessModifier = true)]
    [DtoVirtualProperty("FullName", typeof(string), "entity.FirstName + \" \" + entity.LastName")]
    [DtoVirtualProperty("DisplayName", typeof(string), ExpressionMemberName = nameof(DisplayNameExpression))]
    [DtoVirtualProperty("RealAge", typeof(int), ExpressionMemberName=nameof(RealAgeExpression))]
    internal class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public static Expression<Func<User, string>> DisplayNameExpression => u => $"{u.FirstName} · {u.LastName}";


        public string? Email { get; set; }
        [DtoIgnore] public string? Password { get; set; }
        public int Age { get; set; }

        public static Expression<Func<User, int>> RealAgeExpression => a => a.Age + 10;
    }
}
