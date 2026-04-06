using Grpc.Core;
using PostService.Application.Abstractions.Integrations;
using PostService.Application.Models.Users;
using PostService.Infrastructure.Grpc.Proto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostService.Infrastructure.Grpc.Gateway;

public class UserGateway : IUserGateway
{
    private readonly UserService.UserServiceClient _userServiceClient;

    public UserGateway(UserService.UserServiceClient userServiceClient)
    {
        _userServiceClient = userServiceClient;
    }

    public async Task<User?> FindUserById(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            FindUserByIdResponse response = await _userServiceClient.FindUserByIdAsync(
                new FindUserByIdRequest { UserId = userId.ToString() },
                cancellationToken: cancellationToken);
            return new User(new UserId(Guid.Parse(response.UserId)));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}