using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostService.Application.Abstractions.Integrations;
using PostService.Infrastructure.Grpc.Gateway;
using PostService.Infrastructure.Grpc.Options;
using PostService.Infrastructure.Grpc.Proto;
using System;

namespace PostService.Infrastructure.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<UserService.UserServiceClient>(o =>
        {
            GrpcClientOptions options = configuration
                .GetRequiredSection("GrpcClients:UserService")
                .Get<GrpcClientOptions>()!;

            o.Address = new Uri(options.Address);
        });

        services.AddScoped<IUserGateway, UserGateway>();
        return services;
    }
}