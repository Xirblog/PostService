using FluentMigrator;

namespace PostService.Infrastructure.Npgsql.Migrations;

[Migration(2)]
public sealed class M2_PostIdGeneratedByDb : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");
        Execute.Sql("ALTER TABLE posts ALTER COLUMN post_id SET DEFAULT gen_random_uuid();");
    }

    public override void Down()
    {
        Execute.Sql("ALTER TABLE posts ALTER COLUMN post_id DROP DEFAULT;");
    }
}

