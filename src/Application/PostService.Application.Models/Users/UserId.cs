using System;

namespace PostService.Application.Models.Users;

public readonly record struct UserId(Guid Value)
{
    public static UserId Default => new(Guid.Empty);
}