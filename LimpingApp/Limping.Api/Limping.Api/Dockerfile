FROM  mcr.microsoft.com/dotnet/core/sdk:2.1.603-stretch AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Limping.Api/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY Limping.Api/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.1.10-stretch-slim
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Limping.Api.dll"]