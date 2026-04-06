using Microsoft.AspNetCore.Builder;
using PostService.Presentation.Grpc.Services;

namespace PostService.Presentation.Grpc.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<GrpcPostsService>();
        return app;
    }
}