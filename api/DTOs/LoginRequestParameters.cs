namespace API.DTOs;

public class LoginRequestParameters
{
    public required string UserName { get; set; }   
    public required string Password { get; set; }
}