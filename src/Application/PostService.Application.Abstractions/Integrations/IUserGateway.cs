using PostService.Application.Models.Users;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostService.Application.Abstractions.Integrations;

public interface IUserGateway
{
    Task<User?> FindUserById(Guid userId, CancellationToken cancellationToken);
}