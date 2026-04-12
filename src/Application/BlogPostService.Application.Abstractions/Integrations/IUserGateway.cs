using BlogPostService.Application.Models.Users;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlogPostService.Application.Abstractions.Integrations;

public interface IUserGateway
{
    Task<User?> FindUserById(Guid userId, CancellationToken cancellationToken);
}