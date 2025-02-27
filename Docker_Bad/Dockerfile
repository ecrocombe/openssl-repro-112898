# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine@sha256:d5557c9bf579701537a3790a2a0f549e6356a7352e1430717c3e233de1e66173 AS base
USER root
RUN apk upgrade --update-cache --available \
    && apk add openssl

USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Docker_Bad/Docker_Bad.csproj", "Docker_Bad/"]
RUN dotnet restore "./Docker_Bad/Docker_Bad.csproj"
COPY . .
WORKDIR "/src/Docker_Bad"
RUN dotnet build "./Docker_Bad.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Docker_Bad.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker_Bad.dll"]