using DotnetDemo.Domain.Entities;
using FluentNHibernate.Mapping;

namespace DotnetDemo.Persistence.Mappings;

public class SocialMap : ClassMap<Social>
{
    public SocialMap()
    {
        Table("socials");

        Id(x => x.Id).GeneratedBy.GuidComb();

        Map(x => x.ProfileLink).Column("profile_link").Not.Nullable();
        Map(x => x.Slug).Not.Nullable().Unique().Length(255);
        Map(x => x.Description).CustomSqlType("text").Not.Nullable();
        Map(x => x.CreatedAt).Column("created_at").Not.Nullable();
        Map(x => x.UpdatedAt).Column("updated_at").Not.Nullable();

        References(x => x.Author)
            .Column("author_id")
            .Not.Nullable()
            .Cascade.None();
    }
}

