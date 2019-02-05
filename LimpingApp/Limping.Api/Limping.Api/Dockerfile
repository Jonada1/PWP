FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 57307
EXPOSE 44334

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY Limping.Api/Limping.Api.csproj Limping.Api/
RUN dotnet restore Limping.Api/Limping.Api.csproj
COPY . .
WORKDIR /src/Limping.Api
RUN dotnet build Limping.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Limping.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Limping.Api.dll"]
