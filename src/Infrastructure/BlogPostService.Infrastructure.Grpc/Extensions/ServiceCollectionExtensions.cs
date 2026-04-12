using BlogPostService.Application.Abstractions.Integrations;
using BlogPostService.Infrastructure.Grpc.Gateway;
using BlogPostService.Infrastructure.Grpc.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlogPostService.Infrastructure.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<UserService.Presentation.Grpc.Protos.UserService.UserServiceClient>(o =>
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