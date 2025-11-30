using DotnetDemo.Domain.Requests;
using FluentValidation;

namespace DotnetDemo.Domain.Requests.Validators;

public class AuthorRequestValidator : AbstractValidator<AuthorRequest>
{
    public AuthorRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Birthdate).LessThan(DateTime.UtcNow.AddDays(1));
        RuleFor(x => x.PhotoUrl).NotEmpty();
        RuleFor(x => x.PublicKey).NotEmpty();
        RuleFor(x => x.Bio).NotEmpty();
    }
}

