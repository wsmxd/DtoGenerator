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

// Test UserWithPrivateSet
Console.WriteLine("\n--- Testing UserWithPrivateSet ---");
var userWithPrivateSet = new UserWithPrivateSet(3, "Alice", "alice@example.com");
var userWithPrivateSetDto = UserWithPrivateSetDto.FromEntity(userWithPrivateSet);
Console.WriteLine($"DTO Created: {userWithPrivateSetDto}");

// ToEntity should skip the Name property since it has a private setter
var convertedUser = userWithPrivateSetDto.ToEntity();
Console.WriteLine($"Converted back - Id: {convertedUser.Id}, Name: '{convertedUser.Name}', Email: {convertedUser.Email}");
Console.WriteLine("Note: Name is empty in converted entity because it has a private setter and cannot be set via object initializer");

// Test UserWithMixedAccessors
Console.WriteLine("\n--- Testing UserWithMixedAccessors ---");
var userMixed = new UserWithMixedAccessors(4, "Bob", "bob@example.com", "Test");
var userMixedDto = UserWithMixedAccessorsDto.FromEntity(userMixed);
Console.WriteLine($"DTO Created: {userMixedDto}");

var convertedMixed = userMixedDto.ToEntity();
Console.WriteLine($"Converted back - Id: {convertedMixed.Id}, ReadOnlyName: '{convertedMixed.ReadOnlyName}', InitOnlyEmail: '{convertedMixed.InitOnlyEmail}', NormalProperty: '{convertedMixed.NormalProperty}'");
Console.WriteLine("Note: ReadOnlyName is empty (private set), but InitOnlyEmail and NormalProperty are preserved");
