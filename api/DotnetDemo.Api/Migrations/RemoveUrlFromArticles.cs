using FluentMigrator;

namespace DotnetDemo.Migrations;

[Migration(20251130235900)]
public class RemoveUrlFromArticles : Migration
{
    public override void Up()
    {
        if (!Schema.Table("articles").Exists())
        {
            return;
        }

        if (Schema.Table("articles").Column("url").Exists())
        {
            Delete.Index("idx_articles_url_unique").OnTable("articles").OnColumn("url");
            Delete.Column("url").FromTable("articles");
        }
    }

    public override void Down()
    {
        if (!Schema.Table("articles").Exists())
        {
            return;
        }

        if (!Schema.Table("articles").Column("url").Exists())
        {
            Alter.Table("articles")
                .AddColumn("url").AsString(1024).Nullable();
        }
    }
}

