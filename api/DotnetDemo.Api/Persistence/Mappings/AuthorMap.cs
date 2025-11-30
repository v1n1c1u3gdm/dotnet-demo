using DotnetDemo.Domain.Entities;
using FluentNHibernate.Mapping;

namespace DotnetDemo.Persistence.Mappings;

public class AuthorMap : ClassMap<Author>
{
    public AuthorMap()
    {
        Table("authors");

        Id(x => x.Id).GeneratedBy.GuidComb();

        Map(x => x.Name).Not.Nullable().Length(255);
        Map(x => x.Birthdate).CustomType("Date").Not.Nullable();
        Map(x => x.PhotoUrl).Column("photo_url").Not.Nullable();
        Map(x => x.PublicKey).Column("public_key").CustomSqlType("text").Not.Nullable();
        Map(x => x.Bio).CustomSqlType("text").Not.Nullable();
        Map(x => x.CreatedAt).Column("created_at").Not.Nullable();
        Map(x => x.UpdatedAt).Column("updated_at").Not.Nullable();

        HasMany(x => x.Socials)
            .KeyColumn("author_id")
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .LazyLoad();

        HasMany(x => x.Articles)
            .KeyColumn("author_id")
            .Inverse()
            .Cascade.AllDeleteOrphan()
            .LazyLoad();
    }
}

