using System;

namespace PostService.Application.Models.Posts;

public readonly record struct PostId(Guid Value)
{
    public static PostId Default => new(Guid.Empty);
}