FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/api/TechNews.Auth.Api/TechNews.Auth.Api.csproj", "src/api/TechNews.Auth.Api/"]
RUN dotnet restore "src/api/TechNews.Auth.Api/TechNews.Auth.Api.csproj"
COPY . .
WORKDIR "/src/src/api/TechNews.Auth.Api"
RUN dotnet build "TechNews.Auth.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "TechNews.Auth.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechNews.Auth.Api.dll"]
