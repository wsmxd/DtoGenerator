# Roslyn DTO Generator (C#)

这是一个基于 .NET Roslyn 的高性能**源生成器 (Source Generator)**。它能在编译时根据你的实体类（Entity）自动生成 DTO 类以及高性能的映射代码。

🚀 **零反射** | ⚡ **编译时安全** | 🛠 **高度可配置**

## ✨ 核心特性

- **自动 DTO 生成**：自动生成与实体对应的 `public partial class {Name}Dto`。
- **内置映射器**：自动生成 `FromEntity()` 和 `ToEntity()` 方法。
- **双向映射**：支持 Entity ↔ DTO 互相转换。
- **高性能**：完全不使用运行时反射 (Reflection)，所有代码均为硬编码的赋值语句，性能与手写代码一致。
- **灵活配置**：
  - `[DtoName]`: 重命名 DTO 中的属性。
  - `[DtoIgnore]`: 忽略敏感字段（如密码哈希）。
  - `[DtoVirtualProperty]`: 支持虚拟/聚合属性（例如：将 FirstName + LastName 映射为 FullName）。
- **自定义钩子**：支持分部方法 (Partial Methods) 或强制接口 (`IDtoMapperHooks`) 来处理复杂的自定义映射逻辑。
- **现代 C# 支持**：完美支持 `required` 关键字、`init` 属性和可空引用类型。

## 📦 安装说明

*(如果是本地调试)*

1. 将 `DtoGenerator` 项目作为普通引用添加到你的业务项目。
2. 将 `DtoGenerator.Source` 项目作为 **Analyzer** (分析器) 添加到你的业务项目。

## 🚀 快速开始

### 1. 定义实体

只需在类上添加 `[GenerateDto]` 特性。

```csharp
using DtoGenerator;

namespace MyApp.Models;

[GenerateDto]
public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    
    [DtoIgnore] // 该字段不会出现在 DTO 中
    public string PasswordHash { get; set; } 
}
```
## 2. 使用生成的 DTO
生成器会在后台实时生成代码，你可以直接使用：
```csharp
var user = new User { Id = 1, Username = "admin", PasswordHash = "###" };

// Entity -> DTO
var dto = UserDto.FromEntity(user);
Console.WriteLine(dto.Username); // 输出 "admin"

// DTO -> Entity
var newUser = dto.ToEntity();
```

## 📚 高级用法
### 1. 属性重命名
将实体的 `UserEmail` 映射为 DTO 的 `Email`。
```csharp
[DtoName("Email")]
public string UserEmail { get; set; }
```
### 2. 虚拟属性（聚合字段）
在 DTO 中增加一个实体里不存在的字段，并定义计算逻辑。在表达式中使用 entity 代表源对象。
```csharp
[DtoVirtualProperty("FullName", typeof(string), "entity.FirstName + \" \" + entity.LastName")]
public class User { ... }
```
### 3. 自定义逻辑与钩子 (Hooks)
有时你需要手动处理反向映射（例如把 FullName 拆回两个字段），或者在映射前后记录日志。
#### 方式 A: 可选分部方法 (默认)
只需创建一个 UserDto 的分部类文件即可：
```csharp
// 手动创建 UserDto.Custom.cs
public partial class UserDto
{
    partial void OnEntityCreated(User targetEntity)
    {
        // 在这里写反向映射逻辑
        Console.WriteLine("映射完成，正在处理自定义逻辑...");
    }
}
```
#### 方式 B: 强制接口实现 (严格模式)
如果你希望团队成员必须处理某些逻辑，可以开启 EnforceHooks。
```csharp
[GenerateDto(EnforceHooks = true)]
public class User { ... }
```