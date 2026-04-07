using SourceKit.Generators.Builder.Annotations;
using System;

namespace PostService.Application.Contracts.Posts.Operations;

[GenerateBuilder]
public partial record PostDtoQuery(
    Guid[] PostIds,
    string? NameSubstring,
    string? DescriptionSubstring,
    string? MarkdownContentSubstring,
    Guid[] AuthorIds,
    DateTime? CreatedBefore,
    DateTime? CreatedAfter,
    DateTime? UpdatedBefore,
    DateTime? UpdatedAfter);