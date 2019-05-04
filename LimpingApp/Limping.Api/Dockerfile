FROM  mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS limping-api-prepare
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Limping.Api.sln .
COPY Limping.Api/Limping.Api.csproj Limping.Api/
COPY Limping.Api.Tests/Limping.Api.Tests.csproj Limping.Api.Tests/
RUN dotnet restore

COPY Limping.Api Limping.Api
COPY Limping.Api.Tests Limping.Api.Tests
FROM limping-api-prepare as limping-api-build
RUN dotnet build  \
    && dotnet publish --output dist Limping.Api/Limping.Api.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /app
COPY --from=limping-api-build /app/Limping.Api/dist .
ENTRYPOINT ["dotnet", "Limping.Api.dll"]