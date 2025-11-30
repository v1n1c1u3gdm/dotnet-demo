using System;

namespace DotnetDemo.Domain.Requests;

public record AuthorRequest(
    string Name,
    DateTime Birthdate,
    string PhotoUrl,
    string PublicKey,
    string Bio);

