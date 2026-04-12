using BlogPostService.Application.Abstractions.Persistence;
using BlogPostService.Application.Abstractions.Persistence.Repositories;

namespace BlogPostService.Infrastructure.Npgsql;

public class NpgsqlPersistenceContext : IPersistenceContext
{
    public NpgsqlPersistenceContext(IPostRepository postRepository)
    {
        PostRepository = postRepository;
    }

    public IPostRepository PostRepository { get; }
}