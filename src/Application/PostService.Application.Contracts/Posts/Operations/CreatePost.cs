using PostService.Application.Contracts.Posts.Models;
using System;

namespace PostService.Application.Contracts.Posts.Operations;

public class CreatePost
{
    public sealed record Request(
        string Name,
        string Description,
        string MarkdownContent,
        Guid AuthorId);

    public abstract record Response
    {
        private Response() { }

        public record Success(PostDto Post) : Response;

        public record AuthorNotFound(string Message) : Response;

        public record PersistenceFailure(string Message) : Response;
    }
}