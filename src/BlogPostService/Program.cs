using BlogPostService.Application.Extensions;
using BlogPostService.Infrastructure.Grpc.Extensions;
using BlogPostService.Infrastructure.Npgsql.Extensions;
using BlogPostService.Presentation.Grpc.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddNpgsql(builder.Configuration)
    .AddMigrator(builder.Configuration)
    .AddGrpcClients(builder.Configuration)
    .AddGrpcApi();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcServices();
app.MigrateUp();

app.Run();