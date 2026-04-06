using Grpc.Core;
using PostService.Application.Abstractions.Integrations;
using PostService.Application.Models.Users;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserService.Presentation.Grpc.Protos;

namespace PostService.Infrastructure.Grpc.Gateway;

public class UserGateway : IUserGateway
{
    private readonly UserService.Presentation.Grpc.Protos.UserService.UserServiceClient _userServiceClient;

    public UserGateway(UserService.Presentation.Grpc.Protos.UserService.UserServiceClient userServiceClient)
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