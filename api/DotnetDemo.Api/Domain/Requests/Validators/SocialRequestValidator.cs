using DotnetDemo.Domain.Requests;
using FluentValidation;

namespace DotnetDemo.Domain.Requests.Validators;

public class SocialRequestValidator : AbstractValidator<SocialRequest>
{
    public SocialRequestValidator()
    {
        RuleFor(x => x.ProfileLink).NotEmpty();
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
    }
}

