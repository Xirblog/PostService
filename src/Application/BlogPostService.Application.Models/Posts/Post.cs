using BlogPostService.Application.Models.Users;
using System;

namespace BlogPostService.Application.Models.Posts;

public sealed record Post(
    PostId PostId,
    string Name,
    string Description,
    string MarkdownContent,
    UserId AuthorId,
    DateTime CreatedAt,
    DateTime UpdatedAt);