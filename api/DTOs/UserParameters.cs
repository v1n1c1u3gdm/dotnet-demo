using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public abstract class UserParameters
{
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
}