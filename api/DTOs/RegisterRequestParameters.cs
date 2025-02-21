using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterRequestParameters
{

    [Required]
    public required string UserName { get; set; }

    [Required/*, RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).*$", ErrorMessage = "Password must meet requirements")*/]
    public required string Password { get; set; }
}