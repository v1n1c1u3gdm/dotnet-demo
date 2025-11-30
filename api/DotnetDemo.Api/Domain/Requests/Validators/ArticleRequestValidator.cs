using DotnetDemo.Domain.Requests;
using FluentValidation;

namespace DotnetDemo.Domain.Requests.Validators;

public class ArticleRequestValidator : AbstractValidator<ArticleRequest>
{
    public ArticleRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PublishedLabel).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PostEntry).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
        RuleFor(x => x.Tags).NotNull();
    }
}

