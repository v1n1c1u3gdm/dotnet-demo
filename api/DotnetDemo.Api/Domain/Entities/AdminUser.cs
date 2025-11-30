using System;

namespace DotnetDemo.Domain.Entities;

public class AdminUser : BaseEntity
{
    public virtual string Username { get; set; } = string.Empty;
    public virtual string DisplayName { get; set; } = string.Empty;
    public virtual string PasswordHash { get; set; } = string.Empty;
    public virtual string PasswordSalt { get; set; } = string.Empty;
    public virtual int PasswordHashIterations { get; set; }
    public virtual DateTime? LastLoginAt { get; set; }
}

