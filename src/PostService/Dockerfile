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
COPY ["src/PostService/PostService.csproj", "src/PostService/"]
COPY ["src/Application/PostService.Application/PostService.Application.csproj", "src/Application/PostService.Application/"]
COPY ["src/Application/PostService.Application.Abstractions/PostService.Application.Abstractions.csproj", "src/Application/PostService.Application.Abstractions/"]
COPY ["src/Application/PostService.Application.Models/PostService.Application.Models.csproj", "src/Application/PostService.Application.Models/"]
COPY ["src/Application/PostService.Application.Contracts/PostService.Application.Contracts.csproj", "src/Application/PostService.Application.Contracts/"]
COPY ["src/Infrastructure/PostService.Infrastructure.Npgsql/PostService.Infrastructure.Npgsql.csproj", "src/Infrastructure/PostService.Infrastructure.Npgsql/"]
COPY ["src/Presentation/PostService.Presentation.Grpc/PostService.Presentation.Grpc.csproj", "src/Presentation/PostService.Presentation.Grpc/"]
RUN dotnet restore "src/PostService/PostService.csproj"
COPY . .
WORKDIR "/src/src/PostService"
RUN dotnet build "./PostService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PostService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostService.dll"]
