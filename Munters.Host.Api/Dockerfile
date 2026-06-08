FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=DEBUG
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Munters.Host.Api/Munters.Host.Api.csproj", "Munters.Host.Api/"]
COPY ["Munters.Giphy/Munters.Giphy.csproj", "Munters.Giphy/"]

RUN dotnet restore "Munters.Host.Api/Munters.Host.Api.csproj"
COPY . .
WORKDIR "/src/Munters.Host.Api"
RUN dotnet build "./Munters.Host.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=$BUILD_CONFIGURATION
RUN dotnet publish "./Munters.Host.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "Munters.Host.Api.dll"]
