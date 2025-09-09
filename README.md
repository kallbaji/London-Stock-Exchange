# LSEProject Solution

## Overview

**LSEProject** is a modular .NET 8 solution for managing and querying London Stock Exchange (LSE) trade data. It demonstrates:
- ASP.NET Core Web API
- Entity Framework Core with PostgreSQL
- Distributed caching with Redis
- JWT authentication (with user-secrets support)
- OAuth2.0 ready structure
- Unit testing with xUnit and Moq

---

## Solution Structure

- **LSEProject**: Main API for posting trades and advanced operations.
- **LSEGETALLAPI**: API for querying all trades and stock values.
- **LSEGETSUBSETAPI**: API for querying a subset of trades.
- **LSEGETTOKENAPI**: API for authentication and JWT token generation.
- **LSEDAL**: Data access layer (EF Core models and context).
- **LSEAuth**: Shared authentication configuration (JWT).
- **LSEProject.Tests**: Unit tests for controllers and business logic.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- [Redis](https://redis.io/)
- [Node.js](https://nodejs.org/) (optional, for frontend)
- [Docker](https://www.docker.com/) (optional, for running Redis/Postgres)

---

## Setup

### 1. Clone the repository

```sh
git clone <your-repo-url>
cd LSEProject
```

### 2. Database

- Update your PostgreSQL connection string in `LSEDAL/LSEDAL.cs` or `appsettings.json`.
- Run database migrations if needed.

### 3. Redis

- **On Mac:**  
  ```sh
  brew install redis
  brew services start redis
  ```
- **Or with Docker:**  
  ```sh
  docker run -p 6379:6379 redis
  ```

### 4. User-Secrets for JWT (Development)

In each API project directory (with a `<UserSecretsId>` in `.csproj`):

```sh
dotnet user-secrets set "Jwt:Issuer" "LSEProjectAPI"
dotnet user-secrets set "Jwt:Audience" "LSEProjectClient"
dotnet user-secrets set "Jwt:Key" "A_Very_Long_Random_Secret_Key_1234567890"
```

### 5. Build and Run

```sh
dotnet build
dotnet run --project LSEGETALLAPI
dotnet run --project LSEProject
# ...run other APIs as needed
```

APIs will be available at their configured ports (see `launchSettings.json`).

---

## Authentication

### JWT

- Uses JWT Bearer authentication.
- Token generation endpoint: `POST /api/auth/token` (see `LSEGETTOKENAPI`).
- Protect endpoints with `[Authorize]` attribute.
- Token validation parameters are loaded from user-secrets or configuration.



## API Endpoints

### LSEGETALLAPI

- `GET /api/trades/stocks/values`  
  Returns all stock values, supports distributed caching.

### LSEProject

- `POST /api/trades`  
  Adds a new trade, invalidates relevant cache entries.  
  Requires JWT Bearer token.

### LSEGETTOKENAPI

- `POST /api/auth/token`  
  Returns a JWT token for valid credentials.

---

## Caching

- Uses Redis for distributed caching.
- Cache keys like `AllStockValues`, `StockValue_{TickerSymbol}`, and `ValuesByTickers_*` are managed and invalidated on data changes.

---

## Testing

Run all unit tests:

```sh
dotnet test
```

---

## Development Tips

- Use [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local JWT secrets.
- Set `ASPNETCORE_ENVIRONMENT=Development` to enable user-secrets.
- Use VS Code launch configurations or `launchSettings.json` for environment setup.

---

## Contributing

1. Fork the repo
2. Create a feature branch
3. Commit your changes
4. Open a pull request

---

## Authors

- Gauri Shankar Pandey

---

## Notes

- For pattern-based cache invalidation, StackExchange.Redis is used directly.
- For any issues, please open an issue or discussion on