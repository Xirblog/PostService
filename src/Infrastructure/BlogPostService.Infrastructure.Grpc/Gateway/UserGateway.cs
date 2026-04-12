using BlogPostService.Application.Abstractions.Integrations;
using BlogPostService.Application.Models.Users;
using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserService.Presentation.Grpc.Protos;

namespace BlogPostService.Infrastructure.Grpc.Gateway;

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