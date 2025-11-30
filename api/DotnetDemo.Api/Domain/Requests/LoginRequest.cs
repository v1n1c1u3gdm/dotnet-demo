using System.ComponentModel.DataAnnotations;

namespace DotnetDemo.Domain.Requests;

public record LoginRequest(
    [property: Required] string Username,
    [property: Required] string Password);

