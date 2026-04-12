using BlogPostService.Application.Abstractions.Persistence.Repositories;

namespace BlogPostService.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IPostRepository PostRepository { get; }
}