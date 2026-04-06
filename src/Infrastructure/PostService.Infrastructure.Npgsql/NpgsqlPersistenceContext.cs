using PostService.Application.Abstractions.Persistence;
using PostService.Application.Abstractions.Persistence.Repositories;

namespace PostService.Infrastructure.Npgsql;

public class NpgsqlPersistenceContext : IPersistenceContext
{
    public NpgsqlPersistenceContext(IPostRepository postRepository)
    {
        PostRepository = postRepository;
    }

    public IPostRepository PostRepository { get; }
}