using Dapper;
using Npgsql;
using PostService.Application.Abstractions.Persistence.Queries;
using PostService.Application.Abstractions.Persistence.Repositories;
using PostService.Application.Models.Posts;
using PostService.Infrastructure.Npgsql.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PostService.Infrastructure.Npgsql.Repositories;

public class NpgsqlPostRepository : IPostRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlPostRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Post> AddAsync(Post post, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO posts (post_id, name, description, markdown_content, author_id, created_at, updated_at)
            VALUES (@PostId, @Name, @Description, @MarkdownContent, @AuthorId, @CreatedAt, @UpdatedAt)
            RETURNING post_id, name, description, markdown_content, author_id, created_at, updated_at;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        PostModel model = await connection.QuerySingleAsync<PostModel>(sql, new
        {
            PostId = post.PostId.Value,
            post.Name,
            post.Description,
            post.MarkdownContent,
            AuthorId = post.AuthorId.Value,
            post.CreatedAt,
            post.UpdatedAt,
        });

        return model.ToPost();
    }

    public async IAsyncEnumerable<Post> QueryAsync(
        PostQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (query.PostIds.Length > 0)
        {
            conditions.Add("post_id = ANY(@PostIds)");
            parameters.Add("PostIds", query.PostIds.Select(id => id.Value).ToArray());
        }

        if (query.AuthorIds.Length > 0)
        {
            conditions.Add("author_id = ANY(@AuthorIds)");
            parameters.Add("AuthorIds", query.AuthorIds.Select(id => id.Value).ToArray());
        }

        if (query.NameSubstring is not null)
        {
            conditions.Add("name ILIKE @NameSubstring");
            parameters.Add("NameSubstring", $"%{query.NameSubstring}%");
        }

        if (query.DescriptionSubstring is not null)
        {
            conditions.Add("description ILIKE @DescriptionSubstring");
            parameters.Add("DescriptionSubstring", $"%{query.DescriptionSubstring}%");
        }

        if (query.MarkdownContentSubstring is not null)
        {
            conditions.Add("markdown_content ILIKE @MarkdownContentSubstring");
            parameters.Add("MarkdownContentSubstring", $"%{query.MarkdownContentSubstring}%");
        }

        if (query.CreatedAfter is not null)
        {
            conditions.Add("created_at >= @CreatedAfter");
            parameters.Add("CreatedAfter", query.CreatedAfter.Value);
        }

        if (query.CreatedBefore is not null)
        {
            conditions.Add("created_at <= @CreatedBefore");
            parameters.Add("CreatedBefore", query.CreatedBefore.Value);
        }

        if (query.UpdatedAfter is not null)
        {
            conditions.Add("updated_at >= @UpdatedAfter");
            parameters.Add("UpdatedAfter", query.UpdatedAfter.Value);
        }

        if (query.UpdatedBefore is not null)
        {
            conditions.Add("updated_at <= @UpdatedBefore");
            parameters.Add("UpdatedBefore", query.UpdatedBefore.Value);
        }

        var sql = new StringBuilder(
            "SELECT post_id, name, description, markdown_content, author_id, created_at, updated_at FROM posts");

        if (conditions.Count > 0)
        {
            sql.Append(" WHERE ").AppendJoin(" AND ", conditions);
        }

        sql.Append(';');

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        IEnumerable<PostModel> records = await connection.QueryAsync<PostModel>(sql.ToString(), parameters);

        foreach (PostModel record in records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return record.ToPost();
        }
    }

    public async Task<Post> UpdateAsync(Post post, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE posts
            SET name             = @Name,
                description      = @Description,
                markdown_content = @MarkdownContent,
                updated_at       = @UpdatedAt
            WHERE post_id = @PostId
            RETURNING post_id, name, description, markdown_content, author_id, created_at, updated_at;
            """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        PostModel model = await connection.QuerySingleAsync<PostModel>(sql, new
        {
            PostId = post.PostId.Value,
            post.Name,
            post.Description,
            post.MarkdownContent,
            post.UpdatedAt,
        });

        return model.ToPost();
    }

    public async Task DeleteAsync(Post post, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM posts WHERE post_id = @PostId;";

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await connection.ExecuteAsync(sql, new { PostId = post.PostId.Value });
    }
}