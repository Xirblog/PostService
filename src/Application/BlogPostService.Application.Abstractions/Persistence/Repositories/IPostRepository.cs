using BlogPostService.Application.Abstractions.Persistence.Queries;
using BlogPostService.Application.Models.Posts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlogPostService.Application.Abstractions.Persistence.Repositories;

public interface IPostRepository
{
    Task<Post> AddAsync(Post post, CancellationToken cancellationToken);

    IAsyncEnumerable<Post> QueryAsync(PostQuery query, CancellationToken cancellationToken);

    Task<Post> UpdateAsync(Post post, CancellationToken cancellationToken);

    Task DeleteAsync(Post post, CancellationToken cancellationToken);
}