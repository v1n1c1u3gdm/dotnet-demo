using System;

namespace DotnetDemo.Domain.Entities;

public class Social : BaseEntity
{
    public virtual string ProfileLink { get; set; } = string.Empty;
    public virtual string Slug { get; set; } = string.Empty;
    public virtual string Description { get; set; } = string.Empty;

    public virtual Guid AuthorId { get; set; }
    public virtual Author Author { get; set; } = null!;
}

