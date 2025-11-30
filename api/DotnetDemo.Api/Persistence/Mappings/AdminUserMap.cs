using DotnetDemo.Domain.Entities;
using FluentNHibernate.Mapping;

namespace DotnetDemo.Persistence.Mappings;

public class AdminUserMap : ClassMap<AdminUser>
{
    public AdminUserMap()
    {
        Table("admin_users");

        Id(x => x.Id).GeneratedBy.GuidComb();

        Map(x => x.Username).Not.Nullable().Length(255).Unique();
        Map(x => x.DisplayName).Column("display_name").Not.Nullable().Length(255);
        Map(x => x.PasswordHash).Column("password_hash").Not.Nullable().Length(512);
        Map(x => x.PasswordSalt).Column("password_salt").Not.Nullable().Length(512);
        Map(x => x.PasswordHashIterations).Column("password_hash_iterations").Not.Nullable();
        Map(x => x.LastLoginAt).Column("last_login_at").Nullable();
        Map(x => x.CreatedAt).Column("created_at").Not.Nullable();
        Map(x => x.UpdatedAt).Column("updated_at").Not.Nullable();
    }
}

