using PostService.Application.Contracts.Posts.Models;
using PostService.Application.Contracts.Posts.Operations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PostService.Application.Contracts.Posts;

public interface IPostsService
{
    Task<CreatePost.Response> CreatePostAsync(CreatePost.Request request, CancellationToken cancellationToken);

    IAsyncEnumerable<PostDto> QueryAsync(PostDtoQuery query, CancellationToken cancellationToken);

    IAsyncEnumerable<PostDto> GetAllAsync(CancellationToken cancellationToken);

    Task<PostDto?> FindByPostId(Guid postId, CancellationToken cancellationToken);

    IAsyncEnumerable<PostDto> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);

    Task<UpdatePost.Response> UpdatePostAsync(UpdatePost.Request request, CancellationToken cancellationToken);

    Task<DeletePost.Response> DeletePostAsync(DeletePost.Request request, CancellationToken cancellationToken);
}