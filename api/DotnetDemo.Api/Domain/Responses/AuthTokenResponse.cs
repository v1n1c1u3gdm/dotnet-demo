using System;

namespace DotnetDemo.Domain.Responses;

public record AuthTokenResponse(string AccessToken, DateTime ExpiresAt, string TokenType = "Bearer");

