using BlogPostService.Application.Contracts.Posts;
using BlogPostService.Application.Contracts.Posts.Models;
using BlogPostService.Application.Contracts.Posts.Operations;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PostService.Presentation.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CreatePostOp = BlogPostService.Application.Contracts.Posts.Operations.CreatePost;
using DeletePostOp = BlogPostService.Application.Contracts.Posts.Operations.DeletePost;
using UpdatePostOp = BlogPostService.Application.Contracts.Posts.Operations.UpdatePost;

namespace PostService.Presentation.Grpc.Services;

public class GrpcPostsService : Protos.PostService.PostServiceBase
{
    private readonly IPostsService _postService;

    private static DateTime NormalizeToUtc(DateTime dateTime)
    {
        return dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            _ => throw new ArgumentOutOfRangeException(nameof(dateTime.Kind), dateTime.Kind, "Unexpected DateTime kind."),
        };
    }

    public GrpcPostsService(IPostsService postService)
    {
        _postService = postService;
    }

    public override async Task<CreatePostResponse> CreatePost(CreatePostRequest request, ServerCallContext context)
    {
        CreatePostOp.Response response = await _postService.CreatePostAsync(
            new CreatePostOp.Request(
                request.Name,
                request.Description,
                request.MarkdownContent,
                Guid.Parse(request.AuthorId)),
            context.CancellationToken);

        return response switch
        {
            CreatePostOp.Response.AuthorNotFound authorNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                authorNotFound.Message)),
            CreatePostOp.Response.PersistenceFailure persistenceFailure => throw new RpcException(
                new Status(StatusCode.Internal, persistenceFailure.Message)),
            CreatePostOp.Response.Success success => new CreatePostResponse
            {
                PostId = success.Post.PostId.ToString(),
            },
            _ => throw new UnreachableException(),
        };
    }

    public override async Task<QueryPostsResponse> QueryPosts(QueryPostsRequest request, ServerCallContext context)
    {
        var query = PostDtoQuery.Build(builder => builder
            .WithPostIds(request.PostIds.Select(Guid.Parse))
            .WithNameSubstring(request.NameSubstring)
            .WithDescriptionSubstring(request.DescriptionSubstring)
            .WithMarkdownContentSubstring(request.MarkdownContentSubstring)
            .WithAuthorIds(request.AuthorIds.Select(Guid.Parse))
            .WithCreatedBefore(request.CreatedBefore?.ToDateTime())
            .WithCreatedAfter(request.CreatedAfter?.ToDateTime())
            .WithUpdatedBefore(request.UpdatedBefore?.ToDateTime())
            .WithUpdatedAfter(request.UpdatedAfter?.ToDateTime()));

        List<PostDto> queryPosts = await _postService.QueryAsync(query, context.CancellationToken).ToListAsync();

        var response = new QueryPostsResponse();
        response.Posts.AddRange(queryPosts.Select(post => new Post
        {
            PostId = post.PostId.ToString(),
            Name = post.Name,
            Description = post.Description,
            MarkdownContent = post.MarkdownContent,
            AuthorId = post.AuthorId.ToString(),
            CreatedAt = NormalizeToUtc(post.CreatedAt).ToTimestamp(),
            UpdatedAt = NormalizeToUtc(post.UpdatedAt).ToTimestamp(),
        }));

        return response;
    }

    public override async Task<UpdatePostResponse> UpdatePost(UpdatePostRequest request, ServerCallContext context)
    {
        UpdatePostOp.Response response = await _postService.UpdatePostAsync(
            new UpdatePostOp.Request(
                Guid.Parse(request.PostId),
                request.Name,
                request.Description,
                request.MarkdownContent),
            context.CancellationToken);

        return response switch
        {
            UpdatePostOp.Response.AuthorNotFound authorNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                authorNotFound.Message)),
            UpdatePostOp.Response.PersistenceFailure persistenceFailure => throw new RpcException(
                new Status(StatusCode.Internal, persistenceFailure.Message)),
            UpdatePostOp.Response.Success => new UpdatePostResponse
            {
                Success = true,
            },
            _ => throw new UnreachableException(),
        };
    }

    public override async Task<DeletePostResponse> DeletePost(DeletePostRequest request, ServerCallContext context)
    {
        DeletePostOp.Response response = await _postService.DeletePostAsync(
            new DeletePostOp.Request(Guid.Parse(request.PostId)),
            context.CancellationToken);

        return response switch
        {
            DeletePostOp.Response.PersistenceFailure persistenceFailure => throw new RpcException(
                new Status(StatusCode.Internal, persistenceFailure.Message)),
            DeletePostOp.Response.PostNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Post was not found.")),
            DeletePostOp.Response.Success => new DeletePostResponse
            {
                Success = true,
            },
            _ => throw new UnreachableException(),
        };
    }
}