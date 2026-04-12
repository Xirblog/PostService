using BlogPostService.Application.Models.Posts;
using BlogPostService.Application.Models.Users;
using System;

namespace BlogPostService.Infrastructure.Npgsql.Models;

internal sealed record PostModel(
    Guid PostId,
    string Name,
    string Description,
    string MarkdownContent,
    Guid AuthorId,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public Post ToPost()
    {
        return new Post(
            new PostId(PostId),
            Name,
            Description,
            MarkdownContent,
            new UserId(AuthorId),
            CreatedAt,
            UpdatedAt);
    }
}