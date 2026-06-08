# Munters Giphy Integration API

A .NET 10 Web API that integrates with the Giphy API to fetch trending GIFs and search for GIFs by term. The application includes a caching mechanism using Redis to optimize performance and reduce redundant API calls.

## Overview

This project was developed as part of a Back-End C# Developer Exam. It demonstrates:
- Integration with external APIs (Giphy).
- Caching strategies using Redis.
- Clean Architecture principles and SOLID design.
- Asynchronous programming and safe concurrent operations.
- Containerization with Docker and Docker Compose.
- API documentation with Scalar.

## Stack

- **Language:** C# 14.0
- **Framework:** ASP.NET Core 10.0
- **Database/Cache:** Redis
- **Package Manager:** NuGet (Central Package Management)
- **Containerization:** Docker, Docker Compose
- **API Documentation:** Scalar / OpenAPI

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for containerized execution and Redis)
- Giphy API Key (a default one is provided in `appsettings.json` for testing)

## Setup & Run

### Running with Docker Compose (Recommended)

The easiest way to run the application along with its Redis dependency:

```powershell
docker compose up --build
```

The API will be available at:
- HTTP: `http://localhost:8080`
- HTTPS: `https://localhost:8081`
- API Documentation (Scalar): `http://localhost:8080/scalar/v1`

### Running Locally

1. **Start Redis:**
   You can start only the Redis service using Docker:
   ```powershell
   docker compose up redis -d
   ```

2. **Configure Connection String:**
   Ensure `RedisCacheOptions:ConnectionString` in `Munters.Host.Api/appsettings.json` points to your local Redis instance (default: `localhost:6379`).

3. **Run the API:**
   ```powershell
   dotnet run --project Munters.Host.Api/Munters.Host.Api.csproj
   ```

## Scripts & Commands

- **Build Solution:** `dotnet build`
- **Run Tests:** `dotnet test`
- **Publish:** `dotnet publish -c Release`
- **Docker Build:** `docker build -t munters-giphy -f Munters.Host.Api/Dockerfile .`

## API Endpoints

- `GET /trending`: Fetches URLs of the trending GIFs of the day.
- `GET /search/{text}`: Searches for GIFs given a search term and returns their URLs.

## Environment Variables & Configuration

Configuration is managed via `appsettings.json` and can be overridden by environment variables.

| Key | Description | Default |
|-----|-------------|---------|
| `GiphyApiClientOptions:ApiKey` | API Key for Giphy | `Emzgikb7...` |
| `GiphyApiClientOptions:BaseUrl` | Giphy API Base URL | `https://api.giphy.com` |
| `RedisCacheOptions:ConnectionString` | Redis connection string | `https://localhost:6379` |
| `RedisCacheOptions:InstanceName` | Redis instance name | `Munters.Host.Api` |

## Tests

The solution includes a test project `Munters.Giphy.Tests`.
Run tests using:
```powershell
dotnet test
```

## Project Structure

- `Munters.Host.Api`: The entry point project (ASP.NET Core Web API).
- `Munters.Giphy`: Core logic, handlers, and external API client.
- `Munters.Giphy.Tests`: Unit and integration tests.
- `Directory.Packages.props`: Central Package Management configuration.
- `compose.yaml`: Docker Compose orchestration.

## TODOs
- [ ] Implement UI for displaying GIFs (Bonus requirement).
- [ ] Add more comprehensive integration tests.
- [ ] Set up CI/CD pipeline.

## License

TODO: Add license information.
