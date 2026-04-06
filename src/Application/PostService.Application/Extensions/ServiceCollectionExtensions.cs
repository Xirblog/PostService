using Microsoft.Extensions.DependencyInjection;
using PostService.Application.Contracts.Posts;

namespace PostService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPostsService, Services.PostsService>();
        return services;
    }
}