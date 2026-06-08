# Munters Giphy Integration API

A .NET 10 Web API that integrates with the Giphy API to fetch trending GIFs and search for GIFs by term. The application
includes a caching mechanism using Redis to optimize performance and reduce redundant API calls.

## Overview

This project was developed as part of a Back-End C# Developer Exam. It demonstrates:

- Integration with external APIs (Giphy).
- Caching strategies using Redis.
- Clean Architecture principles and SOLID design.
- Asynchronous programming and safe concurrent operations.
- Containerization with Docker and Docker Compose.
- API documentation with Scalar.
- Health Monitoring.

## Stack

- **Language:** C# 14.0
- **Framework:** ASP.NET Core 10.0
- **Database/Cache:** Redis 7.0 (Alpine)
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
- HTTPS: `https://localhost:8081` (Note: requires valid dev certificates)
- API Documentation (Scalar): `http://localhost:8080/scalar/v1`
- Health Checks: `http://localhost:8080/hc`

### Running Locally

1. **Start Redis:**
   You can start only the Redis service using Docker:
   ```powershell
   docker compose up redis -d
   ```

2. **Configure Connection String:**
   Ensure `RedisCacheOptions:ConnectionString` in `Munters.Host.Api/appsettings.json` points to your local Redis
   instance (default: `127.0.0.1:6379,abortConnect=false`).

3. **Run the API:**
   ```powershell
   dotnet run --project Munters.Host.Api/Munters.Host.Api.csproj
   ```

## Scripts & Commands

- **Build Solution:** `dotnet build`
- **Run Tests:** `dotnet test`
- **Clean Solution:** `dotnet clean`
- **Publish:** `dotnet publish -c Release`
- **Docker Build:** `docker build -t munters-giphy -f Munters.Host.Api/Dockerfile .`

## API Endpoints

- `GET /trending`: Returns an array of URLs for today's trending GIFs.
- `GET /search/{text}`: Returns an array of URLs for GIFs matching the search term.
- `GET /hc`: Health check endpoint providing status of the application and Redis connection.
- `GET /scalar/v1`: Interactive API documentation.

## Environment Variables & Configuration

Configuration is managed via `appsettings.json` and can be overridden by environment variables (e.g., using `__` as a
separator for nested keys).

| Key                                                  | Description                    | Default                             |
|------------------------------------------------------|--------------------------------|-------------------------------------|
| `GiphyApiClientOptions:ApiKey`                       | API Key for Giphy              | `Emzgikb7...`                       |
| `GiphyApiClientOptions:BaseUrl`                      | Giphy API Base URL             | `https://api.giphy.com`             |
| `GiphyApiClientOptions:SearchExpirationsInHours`     | Cache TTL for search results   | `24`                                |
| `GiphyApiClientOptions:TrendingExpirationsInMinutes` | Cache TTL for trending results | `15`                                |
| `RedisCacheOptions:ConnectionString`                 | Redis connection string        | `127.0.0.1:6379,abortConnect=false` |
| `RedisCacheOptions:InstanceName`                     | Redis instance name            | `Munters.Host.Api`                  |

*Note: When running via Docker Compose, `RedisCacheOptions__ConnectionString` is used to override the Redis connection
to use the container name `redis`.*

## Tests

The solution includes a test project `Munters.Giphy.Tests` with unit and integration tests.
Run tests using:

```powershell
dotnet test
```

## Project Structure

- `Munters.Giphy`: Core library containing logic, handlers, models, and the Giphy API client.
- `Munters.Host.Api`: ASP.NET Core Web API (Entry Point).
- `Munters.Giphy.Tests`: xUnit test project for unit and integration testing.
- `Directory.Packages.props`: Central Package Management for NuGet packages.
- `compose.yaml`: Docker Compose configuration for local development.

## TODOs

- [ ] Implement UI for displaying GIFs (Bonus requirement).
- [ ] Improve error handling for edge cases in API client.
- [ ] Add more comprehensive integration tests.
- [ ] Set up CI/CD pipeline.
- [ ] (TODO: add LICENSE file)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details (TODO: add LICENSE file).
