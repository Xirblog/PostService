using PostService.Application.Abstractions.Persistence.Repositories;

namespace PostService.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    IPostRepository PostRepository { get; }
}