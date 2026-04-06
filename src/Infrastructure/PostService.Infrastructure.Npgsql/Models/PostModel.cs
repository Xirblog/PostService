using PostService.Application.Models.Posts;
using PostService.Application.Models.Users;
using System;

namespace PostService.Infrastructure.Npgsql.Models;

internal sealed record PostModel(
    Guid PostId,
    string Name,
    string Description,
    string MarkdownContent,
    Guid AuthorId,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public Post ToPost() => new Post(
        new PostId(PostId),
        Name,
        Description,
        MarkdownContent,
        new UserId(AuthorId),
        CreatedAt,
        UpdatedAt);
}