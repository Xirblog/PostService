using System;

namespace PostService.Application.Contracts.Posts.Models;

public sealed record PostDto(
    Guid PostId,
    string Name,
    string Description,
    string MarkdownContent,
    Guid AuthorId,
    DateTime CreatedAt,
    DateTime UpdatedAt);