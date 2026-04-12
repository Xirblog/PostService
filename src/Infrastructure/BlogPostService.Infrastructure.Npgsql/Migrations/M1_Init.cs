using FluentMigrator;
using System;

namespace BlogPostService.Infrastructure.Npgsql.Migrations;

[Migration(1)]
public sealed class M1_Init : Migration
{
    public override void Up()
    {
        Create.Table("posts")
            .WithColumn("post_id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("description").AsString().NotNullable()
            .WithColumn("markdown_content").AsString().NotNullable()
            .WithColumn("author_id").AsGuid().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("updated_at").AsDateTime().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("posts");
    }
}