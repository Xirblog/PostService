using BlogPostService.Presentation.Grpc.Services;
using Microsoft.AspNetCore.Builder;

namespace BlogPostService.Presentation.Grpc.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<GrpcPostsService>();
        return app;
    }
}