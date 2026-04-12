FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "src/Directory.Build.props"]
COPY ["Directory.Packages.props", "src/Directory.Packages.props"]
COPY ["src/BlogPostService/BlogPostService.csproj", "src/BlogPostService/"]
COPY ["src/Application/BlogPostService.Application/BlogPostService.Application.csproj", "src/Application/BlogPostService.Application/"]
COPY ["src/Application/BlogPostService.Application.Abstractions/BlogPostService.Application.Abstractions.csproj", "src/Application/BlogPostService.Application.Abstractions/"]
COPY ["src/Application/BlogPostService.Application.Models/BlogPostService.Application.Models.csproj", "src/Application/BlogPostService.Application.Models/"]
COPY ["src/Application/BlogPostService.Application.Contracts/BlogPostService.Application.Contracts.csproj", "src/Application/BlogPostService.Application.Contracts/"]
COPY ["src/Infrastructure/BlogPostService.Infrastructure.Grpc/BlogPostService.Infrastructure.Grpc.csproj", "src/Infrastructure/BlogPostService.Infrastructure.Grpc/"]
COPY ["src/Infrastructure/BlogPostService.Infrastructure.Npgsql/BlogPostService.Infrastructure.Npgsql.csproj", "src/Infrastructure/BlogPostService.Infrastructure.Npgsql/"]
COPY ["src/Presentation/BlogPostService.Presentation.Grpc/BlogPostService.Presentation.Grpc.csproj", "src/Presentation/BlogPostService.Presentation.Grpc/"]
RUN dotnet restore "src/BlogPostService/BlogPostService.csproj"
COPY . .
WORKDIR "/src/src/BlogPostService"
RUN dotnet build "./BlogPostService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlogPostService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogPostService.dll"]
