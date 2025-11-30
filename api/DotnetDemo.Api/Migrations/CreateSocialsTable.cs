using System.Data;
using FluentMigrator;

namespace DotnetDemo.Migrations;

[Migration(20251129233758)]
public class CreateSocialsTable : Migration
{
    public override void Up()
    {
        if (Schema.Table("socials").Exists())
        {
            return;
        }

        Create.Table("socials")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("profile_link").AsString(2048).NotNullable()
            .WithColumn("slug").AsString(255).NotNullable()
            .WithColumn("description").AsCustom("text").NotNullable()
            .WithColumn("author_id").AsGuid().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        IfDatabase("mysql").Create.ForeignKey("fk_socials_author")
            .FromTable("socials").ForeignColumn("author_id")
            .ToTable("authors").PrimaryColumn("id")
            .OnDeleteOrUpdate(Rule.Cascade);

        Create.Index("idx_socials_slug_unique")
            .OnTable("socials")
            .OnColumn("slug")
            .Unique();
    }

    public override void Down()
    {
        if (Schema.Table("socials").Exists())
        {
            Delete.Table("socials");
        }
    }
}

