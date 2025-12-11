**[中文文档](README_CN.md)**
[![NuGet](https://img.shields.io/nuget/v/Mxd.DtoGenerator.svg)](https://www.nuget.org/packages/Mxd.DtoGenerator)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mxd.DtoGenerator.svg)](https://www.nuget.org/packages/Mxd.DtoGenerator)
# Roslyn DTO Generator

A high-performance, compile-time **Source Generator** for .NET that automatically creates DTO classes and mapping methods based on your entity definitions.

🚀 **Zero Reflection** | ⚡ **Compile-Time Safety** | 🛠 **Highly Customizable**

## ✨ Features

- **Auto DTO Generation**: Automatically generates `public partial class {Name}Dto` from your entities.
- **Built-in Mapper**: Generates `FromEntity()` and `ToEntity()` methods automatically.
- **Bidirectional Mapping**: Supports Entity ↔ DTO conversion.
- **Zero Overhead**: No runtime reflection (System.Reflection); code is generated at compile time.
- **Flexible Configuration**:
  - `[DtoName]`: Rename properties in the DTO.
  - `[DtoIgnore]`: Exclude sensitive properties (e.g., PasswordHash).
  - `[DtoVirtualProperty]`: Add calculated/aggregated properties (e.g., FirstName + LastName -> FullName).
- **Custom Logic Hooks**: Support for partial methods or enforced interfaces (`IDtoMapperHooks`) for custom mapping logic.
- **Modern C# Support**: Supports `required` modifiers, `init` properties, and nullable reference types.

## 📦 Installation

*(If you plan to publish to NuGet, add command here. For now, referencing locally:)*

1. Add the `DtoGenerator` project reference to your project.
2. Add the `DtoGenerator.Source` project as an **Analyzer**.

## 🚀 Quick Start

### 1. Define your Entity

Simply add `[GenerateDto]` to your class.

```csharp
using DtoGenerator;

namespace MyApp.Models;

[GenerateDto]
public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    
    [DtoIgnore] // Won't appear in DTO
    public string PasswordHash { get; set; } 
}
```
## 2. Use the Generated DTO
The generator creates `UserDto` in the background immediately.
``` csharp
var user = new User { Id = 1, Username = "admin", PasswordHash = "###" };

// Entity -> DTO
var dto = UserDto.FromEntity(user);
Console.WriteLine(dto.Username); // "admin"

// DTO -> Entity
var newUser = dto.ToEntity();
```
## 📚 Advanced Usage
### 1. Inheriting Base Class Properties
By default, only properties declared in the current class are included.

To **include all public properties from the entire inheritance chain**, use:
``` csharp
[GenerateDto(IncludeBaseProperties = true)]
public class AdminUser : User
{
    public string Role { get; set; }
}
```
This will generate a `AdminUserDto` containing:

- `Id`, `Username` (from base `User`)
- `Role` (from `AdminUser`)

> 🔹 Note: Only `public instance properties` from base classes are included. Private, protected, or static members are ignored.
### 2. Renaming Properties
Map `UserEntity.UserEmail` to `UserDto.Email`.
``` csharp
[DtoName("Email")]
public string UserEmail { get; set; }
```
### 3. Virtual / Calculated Properties
Combine fields into a new property in the DTO. Use entity to refer to the source object.
``` csharp
[DtoVirtualProperty("FullName", typeof(string), "entity.FirstName + \" \" + entity.LastName")]
public class User { ... }
```
### 4. Custom Logic & Hooks
You can hook into the mapping process to handle complex scenarios (e.g., splitting a string back into two fields during `ToEntity`).
#### Option A: Optional Partial Methods (Default)
Simply create a partial class file for your DTO.
``` csharp
// UserDto.Custom.cs
public partial class UserDto
{
    partial void OnEntityCreated(User targetEntity)
    {
        // Custom reverse mapping logic
        Console.WriteLine("Mapping finished!");
    }
}
```
#### Option B: Enforced Interface (Strict Mode)
Force the implementation of hooks using `EnforceHooks = true`.
``` csharp
[GenerateDto(EnforceHooks = true)]
public class User { ... }
```
