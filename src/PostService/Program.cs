using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PostService.Application.Extensions;
using PostService.Infrastructure.Grpc.Extensions;
using PostService.Infrastructure.Npgsql.Extensions;
using PostService.Presentation.Grpc.Extensions;

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