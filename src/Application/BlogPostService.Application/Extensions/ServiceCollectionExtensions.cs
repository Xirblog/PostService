using BlogPostService.Application.Contracts.Posts;
using Microsoft.Extensions.DependencyInjection;

namespace BlogPostService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPostsService, Services.PostsService>();
        return services;
    }
}