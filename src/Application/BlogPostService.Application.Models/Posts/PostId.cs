using System;

namespace BlogPostService.Application.Models.Posts;

public readonly record struct PostId(Guid Value)
{
    public static PostId Default => new(Guid.Empty);
}