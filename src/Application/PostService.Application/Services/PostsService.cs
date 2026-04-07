using PostService.Application.Abstractions.Integrations;
using PostService.Application.Abstractions.Persistence;
using PostService.Application.Abstractions.Persistence.Queries;
using PostService.Application.Contracts.Posts;
using PostService.Application.Contracts.Posts.Models;
using PostService.Application.Contracts.Posts.Operations;
using PostService.Application.Mapping;
using PostService.Application.Models.Posts;
using PostService.Application.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PostService.Application.Services;

public class PostsService : IPostsService
{
    private readonly IPersistenceContext _context;
    private readonly IUserGateway _userGateway;

    public PostsService(IPersistenceContext context, IUserGateway userGateway)
    {
        _context = context;
        _userGateway = userGateway;
    }

    public async Task<CreatePost.Response> CreatePostAsync(
        CreatePost.Request request,
        CancellationToken cancellationToken)
    {
        User? author = await _userGateway.FindUserById(request.AuthorId, cancellationToken);

        if (author is null)
        {
            return new CreatePost.Response.AuthorNotFound($"Author not found: {request.AuthorId}");
        }

        var post = new Post(
            PostId.Default,
            request.Name,
            request.Description,
            request.MarkdownContent,
            author.UserId,
            DateTime.UtcNow,
            DateTime.UtcNow);

        Post addedPost = await _context.PostRepository.AddAsync(post, cancellationToken);

        return new CreatePost.Response.Success(addedPost.MapToDto());
    }

    public async IAsyncEnumerable<PostDto> QueryAsync(
        PostDtoQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var persistenceQuery = PostQuery.Build(builder =>
            builder.WithPostIds(query.PostIds.Select(id => new PostId(id)))
                .WithNameSubstring(query.NameSubstring)
                .WithDescriptionSubstring(query.DescriptionSubstring)
                .WithMarkdownContentSubstring(query.MarkdownContentSubstring)
                .WithAuthorIds(query.AuthorIds.Select(id => new UserId(id)))
                .WithCreatedBefore(query.CreatedBefore)
                .WithCreatedAfter(query.CreatedAfter)
                .WithUpdatedAfter(query.UpdatedAfter)
                .WithUpdatedBefore(query.UpdatedBefore));

        await foreach (Post post in _context.PostRepository.QueryAsync(persistenceQuery, cancellationToken))
        {
            yield return post.MapToDto();
        }
    }

    public async IAsyncEnumerable<PostDto> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (Post post in _context.PostRepository.QueryAsync(
                           PostQuery.Build(builder => builder),
                           cancellationToken))
        {
            yield return post.MapToDto();
        }
    }

    public async Task<PostDto?> FindByPostId(Guid postId, CancellationToken cancellationToken)
    {
        Post? post = await _context.PostRepository
            .QueryAsync(PostQuery.Build(builder => builder.WithPostId(new PostId(postId))), cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            return null;
        }

        return post.MapToDto();
    }

    public async Task<UpdatePost.Response> UpdatePostAsync(
        UpdatePost.Request request,
        CancellationToken cancellationToken)
    {
        Post? existing = await _context.PostRepository
            .QueryAsync(PostQuery.Build(builder => builder.WithPostId(new PostId(request.PostId))), cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (existing is null)
        {
            return new UpdatePost.Response.PersistenceFailure($"Post not found: {request.PostId}");
        }

        Post updated = existing with
        {
            Name = request.Name ?? existing.Name,
            Description = request.Description ?? existing.Description,
            MarkdownContent = request.MarkdownContent ?? existing.MarkdownContent,
        };

        Post savedPost = await _context.PostRepository.UpdateAsync(updated, cancellationToken);

        return new UpdatePost.Response.Success(savedPost.MapToDto());
    }

    public async IAsyncEnumerable<PostDto> GetByAuthorIdAsync(
        Guid authorId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (Post post in _context.PostRepository.QueryAsync(
                           PostQuery.Build(builder => builder.WithAuthorId(new UserId(authorId))),
                           cancellationToken))
        {
            yield return post.MapToDto();
        }
    }

    public async Task<DeletePost.Response> DeletePostAsync(
        DeletePost.Request request,
        CancellationToken cancellationToken)
    {
        Post? post = await _context.PostRepository.QueryAsync(
                PostQuery.Build(builder => builder.WithPostId(new PostId(request.PostId))),
                cancellationToken)
            .SingleOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            return new DeletePost.Response.PostNotFound();
        }

        try
        {
            await _context.PostRepository.DeleteAsync(post, cancellationToken);
            return new DeletePost.Response.Success(post.MapToDto());
        }
        catch (Exception exception)
        {
            return new DeletePost.Response.PersistenceFailure($"Failed to delete post: {exception.Message}");
        }
    }
}