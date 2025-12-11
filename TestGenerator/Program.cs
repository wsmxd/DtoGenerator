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

var user1 = new User1
{
    Id = 2,
    FirstName = "Jane",
    LastName = "Smith",
    Email = "123@123.com",
    Department = "HR"
};
var user1Dto = User1Dto.FromEntity(user1);
Console.WriteLine(user1Dto); // 输出: User1Dto { Department = HR, Id = 2, FirstName = Jane, LastName = Smith, Email = 123@123.com }