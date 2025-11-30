using TestGenerator.Models;

var user = new User
{
    Id = 1,
    FirstName = "John",
    LastName = "Doe",
    Email = "123@123.com",
    Password = "123456"
};
var userDto = UserDto.FromEntity(user);
Console.WriteLine(userDto);
var user2 = userDto.ToEntity();
Console.WriteLine(user2.FirstName);