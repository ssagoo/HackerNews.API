#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HackerNews.API.Host/HackerNews.API.Host.csproj", "HackerNews.API.Host/"]
COPY ["HackerNews.API.Application/HackerNews.API.Application.csproj", "HackerNews.API.Application/"]
RUN dotnet restore "./HackerNews.API.Host/./HackerNews.API.Host.csproj"
COPY . .
WORKDIR "/src/HackerNews.API.Host"
RUN dotnet build "./HackerNews.API.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HackerNews.API.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

# Default values for known public environment variables
ENV API_PORT=8090
ENV API_ALT_PORT=8100
ENV SERVER_NAME=localhost
ENV API_HOST_PORT=8090

EXPOSE $API_PORT
EXPOSE $API_ALT_PORT

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HackerNews.API.Host.dll"]