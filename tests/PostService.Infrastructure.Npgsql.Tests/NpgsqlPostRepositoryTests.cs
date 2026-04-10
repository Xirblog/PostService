using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PostService.Application.Abstractions.Persistence.Queries;
using PostService.Application.Models.Posts;
using PostService.Application.Models.Users;
using PostService.Infrastructure.Npgsql.Migrations;
using PostService.Infrastructure.Npgsql.Repositories;

namespace PostService.Infrastructure.Npgsql.Tests;

public class NpgsqlPostRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18.3-alpine").Build();
    private ServiceProvider? _serviceProvider;

    [Fact]
    public async Task QueryAsync_EmptyQuery_ReturnsAllPosts()
    {
        // Arrange
        await using var dataSource = NpgsqlDataSource.Create(_container.GetConnectionString());
        var repository = new NpgsqlPostRepository(dataSource);
        var newPost = new Post(
            PostId.Default,
            "Name",
            "Description",
            "### Test",
            UserId.Default,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await repository.AddAsync(newPost, CancellationToken.None);
        await repository.AddAsync(newPost, CancellationToken.None);

        // Act
        List<Post> response = await repository
            .QueryAsync(PostQuery.Build(builder => builder), CancellationToken.None)
            .ToListAsync();

        // Assert
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task QueryAsync_PostId_ReturnsSinglePost()
    {
        // Arrange
        await using var dataSource = NpgsqlDataSource.Create(_container.GetConnectionString());
        var repository = new NpgsqlPostRepository(dataSource);
        var newPost = new Post(
            PostId.Default,
            "Name",
            "Description",
            "### Test",
            UserId.Default,
            DateTime.UtcNow,
            DateTime.UtcNow);
        Post addedPost = await repository.AddAsync(newPost, CancellationToken.None);

        // Act
        Post queriedPost = await repository
            .QueryAsync(PostQuery.Build(builder => builder.WithPostId(addedPost.PostId)), CancellationToken.None)
            .SingleAsync();

        // Assert
        Assert.Equal(addedPost.PostId, queriedPost.PostId);
        Assert.Equal(addedPost.Name, queriedPost.Name);
        Assert.Equal(addedPost.Description, queriedPost.Description);
        Assert.Equal(addedPost.MarkdownContent, queriedPost.MarkdownContent);
        Assert.Equal(addedPost.AuthorId, queriedPost.AuthorId);
        Assert.Equal(addedPost.CreatedAt, queriedPost.CreatedAt);
        Assert.Equal(addedPost.UpdatedAt, queriedPost.UpdatedAt);
    }

    public async Task InitializeAsync()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        await _container.StartAsync();

        _serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(_container.GetConnectionString())
                .ScanIn(typeof(M1_Init).Assembly)
                .For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        using IServiceScope scope = _serviceProvider.CreateScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        await _container.DisposeAsync();
    }
}