using System;
using System.Collections.Generic;
using System.Text;

namespace TestGenerator.Models;

internal partial class UserDto
{
    // 钩子 1: Entity -> Dto 转换后触发
    public void OnDtoCreated(User sourceEntity)
    {
        // 这里可以做日志，或者处理一些生成器无法处理的复杂计算
        Console.WriteLine($"[Hook] DTO created for User {sourceEntity.Id}");
    }

    // 钩子 2: Dto -> Entity 转换后触发
    // 这里是我们处理反向逻辑（拆分 FullName）的地方
    public void OnEntityCreated(User targetEntity)
    {
        if (!string.IsNullOrWhiteSpace(this.FullName))
        {
            var parts = this.FullName.Split([' '], 2);
            targetEntity.FirstName = parts.Length > 0 ? parts[0] : "";
            targetEntity.LastName = parts.Length > 1 ? parts[1] : "";

            Console.WriteLine($"[Hook] Splitted FullName '{this.FullName}' back to First/Last Name.");
        }
    }
}
