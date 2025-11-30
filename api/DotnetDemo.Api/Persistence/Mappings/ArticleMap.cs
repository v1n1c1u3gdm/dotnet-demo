using DotnetDemo.Domain.Entities;
using DotnetDemo.Persistence.Types;
using FluentNHibernate.Mapping;

namespace DotnetDemo.Persistence.Mappings;

public class ArticleMap : ClassMap<Article>
{
    public ArticleMap()
    {
        Table("articles");

        Id(x => x.Id).GeneratedBy.GuidComb();

        Map(x => x.Title).Not.Nullable().Length(255);
        Map(x => x.Slug).Not.Nullable().Unique().Length(255);
        Map(x => x.PublishedLabel).Column("published_label").Not.Nullable().Length(255);
        Map(x => x.PostEntry).Column("post_entry").CustomSqlType("longtext").Not.Nullable();
        Map(x => x.Tags).CustomType<StringListJsonType>().CustomSqlType("json").Not.Nullable();
        Map(x => x.CreatedAt).Column("created_at").Not.Nullable();
        Map(x => x.UpdatedAt).Column("updated_at").Not.Nullable();

        References(x => x.Author)
            .Column("author_id")
            .Not.Nullable()
            .Cascade.None();
    }
}

