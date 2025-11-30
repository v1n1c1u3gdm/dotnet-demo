using System;
using System.Collections.Generic;

namespace DotnetDemo.Domain.Requests;

public record ArticleRequest(
    string Title,
    string Slug,
    string PublishedLabel,
    string PostEntry,
    IList<string> Tags,
    Guid AuthorId);

