using System;
using System.Collections.Generic;

namespace DotnetDemo.Domain.Entities;

public class Article : BaseEntity
{
    public virtual string Title { get; set; } = string.Empty;
    public virtual string Slug { get; set; } = string.Empty;
    public virtual string PublishedLabel { get; set; } = string.Empty;
    public virtual string PostEntry { get; set; } = string.Empty;
    public virtual IList<string> Tags { get; set; } = new List<string>();

    public virtual Guid AuthorId { get; set; }
    public virtual Author Author { get; set; } = null!;
}

