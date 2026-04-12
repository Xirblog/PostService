using BlogPostService.Application.Contracts.Posts.Models;
using BlogPostService.Application.Models.Posts;

namespace BlogPostService.Application.Mapping;

public static class PostMappingExtensions
{
    public static PostDto MapToDto(this Post post)
    {
        return new PostDto(
            post.PostId.Value,
            post.Name,
            post.Description,
            post.MarkdownContent,
            post.AuthorId.Value,
            post.CreatedAt,
            post.UpdatedAt);
    }
}