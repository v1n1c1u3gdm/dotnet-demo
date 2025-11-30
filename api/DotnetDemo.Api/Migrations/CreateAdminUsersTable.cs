using FluentMigrator;

namespace DotnetDemo.Migrations;

[Migration(20251201000100)]
public class CreateAdminUsersTable : Migration
{
    public override void Up()
    {
        if (Schema.Table("admin_users").Exists())
        {
            return;
        }

        Create.Table("admin_users")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("username").AsString(255).NotNullable()
            .WithColumn("display_name").AsString(255).NotNullable()
            .WithColumn("password_hash").AsString(512).NotNullable()
            .WithColumn("password_salt").AsString(512).NotNullable()
            .WithColumn("password_hash_iterations").AsInt32().NotNullable()
            .WithColumn("last_login_at").AsDateTime().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Index("idx_admin_users_username_unique")
            .OnTable("admin_users")
            .OnColumn("username")
            .Unique();
    }

    public override void Down()
    {
        if (Schema.Table("admin_users").Exists())
        {
            Delete.Table("admin_users");
        }
    }
}

