using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PostService.Application.Contracts.Posts;
using PostService.Application.Contracts.Posts.Models;
using PostService.Application.Contracts.Posts.Operations;
using PostService.Presentation.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Presentation.Grpc.Services;

public class GrpcPostsService : Protos.PostService.PostServiceBase
{
    private readonly IPostsService _postService;

    public GrpcPostsService(IPostsService postService)
    {
        _postService = postService;
    }

    public override async Task<CreatePostResponse> CreatePost(CreatePostRequest request, ServerCallContext context)
    {
        CreatePost.Response response = await _postService.CreatePostAsync(
            new CreatePost.Request(
                request.Name,
                request.Description,
                request.MarkdownContent,
                Guid.Parse(request.AuthorId)),
            context.CancellationToken);

        return response switch
        {
            CreatePost.Response.AuthorNotFound authorNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                authorNotFound.Message)),
            CreatePost.Response.PersistenceFailure persistenceFailure => throw new RpcException(
                new Status(StatusCode.Internal, persistenceFailure.Message)),
            CreatePost.Response.Success success => new CreatePostResponse
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
            CreatedAt = post.CreatedAt.ToTimestamp(),
            UpdatedAt = post.UpdatedAt.ToTimestamp(),
        }));

        return response;
    }

    public override async Task<UpdatePostResponse> UpdatePost(UpdatePostRequest request, ServerCallContext context)
    {
        UpdatePost.Response response = await _postService.UpdatePostAsync(
            new UpdatePost.Request(
                Guid.Parse(request.PostId),
                request.Name,
                request.Description,
                request.MarkdownContent),
            context.CancellationToken);

        return response switch
        {
            UpdatePost.Response.AuthorNotFound authorNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                authorNotFound.Message)),
            UpdatePost.Response.PersistenceFailure persistenceFailure => throw new RpcException(
                new Status(StatusCode.Internal, persistenceFailure.Message)),
            Application.Contracts.Posts.Operations.UpdatePost.Response.Success => new UpdatePostResponse
            {
                Success = true,
            },
            _ => throw new UnreachableException(),
        };
    }

    public override async Task<DeletePostResponse> DeletePost(DeletePostRequest request, ServerCallContext context)
    {
        DeletePost.Response response = await _postService.DeletePostAsync(
            new DeletePost.Request(Guid.Parse(request.PostId)),
            context.CancellationToken);

        return response switch
        {
            DeletePost.Response.PersistenceFailure persistenceFailure => throw new RpcException(
                new Status(StatusCode.Internal, persistenceFailure.Message)),
            DeletePost.Response.PostNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                "Post was not found.")),
            DeletePost.Response.Success => new DeletePostResponse
            {
                Success = true,
            },
            _ => throw new UnreachableException(),
        };
    }
}