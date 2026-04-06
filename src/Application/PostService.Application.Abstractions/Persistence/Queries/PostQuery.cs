using PostService.Application.Models.Posts;
using PostService.Application.Models.Users;
using SourceKit.Generators.Builder.Annotations;
using System;

namespace PostService.Application.Abstractions.Persistence.Queries;

[GenerateBuilder]
public partial record PostQuery(
    PostId[] PostIds,
    string? NameSubstring,
    string? DescriptionSubstring,
    string? MarkdownContentSubstring,
    UserId[] AuthorIds,
    DateTime? CreatedBefore,
    DateTime? CreatedAfter,
    DateTime? UpdatedBefore,
    DateTime? UpdatedAfter);