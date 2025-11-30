using System.Data;
using FluentMigrator;

namespace DotnetDemo.Migrations;

[Migration(20251129233801)]
public class CreateArticlesTable : Migration
{
    public override void Up()
    {
        if (Schema.Table("articles").Exists())
        {
            return;
        }

        Create.Table("articles")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("title").AsString(255).NotNullable()
            .WithColumn("url").AsString(1024).NotNullable()
            .WithColumn("slug").AsString(255).NotNullable()
            .WithColumn("published_label").AsString(255).NotNullable()
            .WithColumn("post_entry").AsCustom("longtext").NotNullable()
            .WithColumn("tags").AsCustom("json").NotNullable()
            .WithColumn("author_id").AsGuid().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        IfDatabase("mysql").Create.ForeignKey("fk_articles_author")
            .FromTable("articles").ForeignColumn("author_id")
            .ToTable("authors").PrimaryColumn("id")
            .OnDeleteOrUpdate(Rule.Cascade);

        Create.Index("idx_articles_slug_unique")
            .OnTable("articles")
            .OnColumn("slug")
            .Unique();

        Create.Index("idx_articles_url_unique")
            .OnTable("articles")
            .OnColumn("url")
            .Unique();
    }

    public override void Down()
    {
        if (Schema.Table("articles").Exists())
        {
            Delete.Table("articles");
        }
    }
}

