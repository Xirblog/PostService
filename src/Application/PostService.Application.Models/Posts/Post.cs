using PostService.Application.Models.Users;
using System;

namespace PostService.Application.Models.Posts;

public sealed record Post(
    PostId PostId,
    string Name,
    string Description,
    string MarkdownContent,
    UserId AuthorId,
    DateTime CreatedAt,
    DateTime UpdatedAt);