using FluentMigrator;

namespace DotnetDemo.Migrations;

[Migration(20251129233752)]
public class CreateAuthorsTable : Migration
{
    public override void Up()
    {
        if (Schema.Table("authors").Exists())
        {
            return;
        }

        Create.Table("authors")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("birthdate").AsDate().NotNullable()
            .WithColumn("photo_url").AsString(1024).NotNullable()
            .WithColumn("public_key").AsCustom("text").NotNullable()
            .WithColumn("bio").AsCustom("text").NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Index("idx_authors_name_unique")
            .OnTable("authors")
            .OnColumn("name")
            .Unique();
    }

    public override void Down()
    {
        if (Schema.Table("authors").Exists())
        {
            Delete.Table("authors");
        }
    }
}

