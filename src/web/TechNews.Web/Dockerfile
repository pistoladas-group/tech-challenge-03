FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/web/TechNews.Web/TechNews.Web.csproj", "src/web/TechNews.Web/"]
RUN dotnet restore "src/web/TechNews.Web/TechNews.Web.csproj"
COPY . .
WORKDIR "/src/src/web/TechNews.Web"
RUN dotnet build "TechNews.Web.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "TechNews.Web.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechNews.Web.dll"]
