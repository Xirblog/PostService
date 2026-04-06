using PostService.Application.Contracts.Posts.Models;
using PostService.Application.Models.Posts;

namespace PostService.Application.Mapping;

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