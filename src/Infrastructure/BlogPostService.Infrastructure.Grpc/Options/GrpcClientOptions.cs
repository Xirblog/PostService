namespace BlogPostService.Infrastructure.Grpc.Options;

public sealed class GrpcClientOptions
{
    public required string Address { get; init; }
}