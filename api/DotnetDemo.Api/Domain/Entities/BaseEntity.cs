using System;

namespace DotnetDemo.Domain.Entities;

public abstract class BaseEntity
{
    public virtual Guid Id { get; protected set; }
    public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

