using Microsoft.Extensions.DependencyInjection;
using PostService.Application.Abstractions.Integrations;
using PostService.Infrastructure.Grpc.Gateway;
using PostService.Infrastructure.Grpc.Proto;

namespace PostService.Infrastructure.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        services.AddGrpcClient<UserService.UserServiceClient>();
        services.AddScoped<IUserGateway, UserGateway>();
        return services;
    }
}