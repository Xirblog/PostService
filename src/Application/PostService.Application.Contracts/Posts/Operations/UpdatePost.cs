using PostService.Application.Contracts.Posts.Models;
using System;

namespace PostService.Application.Contracts.Posts.Operations;

public class UpdatePost
{
    public sealed record Request(
        Guid PostId,
        string? Name,
        string? Description,
        string? MarkdownContent);

    public abstract record Response
    {
        private Response() { }

        public record Success(PostDto Post) : Response;

        public record AuthorNotFound(string Message) : Response;

        public record PersistenceFailure(string Message) : Response;
    }
}