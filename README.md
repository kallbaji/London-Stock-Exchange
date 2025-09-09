# LSEProject

## Overview

**LSEProject** is a .NET 8 solution that provides APIs for managing and querying London Stock Exchange (LSE) trade data. It demonstrates:
- ASP.NET Core Web API
- Entity Framework Core with PostgreSQL
- Distributed caching with Redis
- Unit testing with xUnit and Moq

---

## Projects

- **LSEGETALLAPI**: API for querying all trades and stock values.
- **LSEProject**: API for posting trades and advanced operations.
- **LSEDAL**: Data access layer (EF Core models and context).
- **LSEProject.Tests**: Unit tests for controllers and business logic.
- **LSEGETAPI**: API for querying  stock value by ticker Symbol.
- **LSEGETSUBSETAPI**: API for querying stock values by list of ticker symbol .
---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- [Redis](https://redis.io/)
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


### 4. Build and Run

```sh
dotnet build
dotnet run --project LSEGETALLAPI
dotnet run --project LSEGETAPI
dotnet run --project LSEGETSUBSETAPI
dotnet run --project LSEProject
```

APIs will be available at `https://localhost:5015` or `http://localhost:5019` for GETAPPAPI (see `launchSettings.json` for other project).

---

## Testing

Run all unit tests:

```sh
dotnet test
```

---

## API Endpoints

### LSEGETALLAPI

- `GET /api/trades/stocks/values`  
  Returns all stock values, supports distributed caching.

### LSESUBSETAPI

- `GET  /api/trades/stocks/values-by-tickers` 
   Returns all stock values that are in list

### LSEGETAPI

- `GET  /api/trades/stocks/value/<TickerSymbol>` 
   Returns all stock values that are in list

### LSEProject

- `POST /api/trades`  
  Adds a new trade, invalidates relevant cache entries.

---

## Caching

- Uses Redis for distributed caching.
- Cache keys like `AllStockValues`, `StockValue_{TickerSymbol}`, and `ValuesByTickers_*` are managed and invalidated on data changes.



---



## Authors

- Gauri Shankar Pandey

---

## Notes

- For pattern-based cache invalidation, StackExchange.Redis is used directly.
