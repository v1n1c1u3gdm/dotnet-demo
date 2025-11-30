using System;
using System.Collections.Generic;

namespace DotnetDemo.Domain.Entities;

public class Author : BaseEntity
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual DateTime Birthdate { get; set; }
    public virtual string PhotoUrl { get; set; } = string.Empty;
    public virtual string PublicKey { get; set; } = string.Empty;
    public virtual string Bio { get; set; } = string.Empty;

    public virtual IList<Social> Socials { get; set; } = new List<Social>();
    public virtual IList<Article> Articles { get; set; } = new List<Article>();
}

