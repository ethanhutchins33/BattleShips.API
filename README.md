# BattleShips.API

## Overview

BattleShips.API is the backend service for the Battleships trainee project. It exposes
authenticated REST endpoints for player profiles, game creation, joining games, ship
placement, firing shots, lobby polling and game-state polling.

The solution is split into:

- `BattleShips.API` - ASP.NET Core Web API host and controllers.
- `BattleShips.API.Library` - game and player service logic.
- `BattleShips.API.Data` - Entity Framework Core data access, repositories and models.
- `BattleShips.API.Tests` - API/controller tests.
- `BattleShips.API.Library.Tests` - service-layer tests.
- `Build` - Terraform configuration for Azure infrastructure.

## Technical Stack

- .NET 10, targeting `net10.0` across all projects.
- .NET SDK `10.0.0`, pinned by `global.json` with `latestMinor` roll-forward.
- ASP.NET Core Web API with controllers and Newtonsoft.Json.
- Entity Framework Core 10 using SQL Server.
- Microsoft Identity Web with Azure AD B2C JWT bearer authentication.
- Swagger/OpenAPI via Swashbuckle, enabled in Development.
- NUnit, FakeItEasy and coverlet for tests.
- Azure Pipelines and Terraform for packaging and infrastructure deployment.

## Configuration

The API reads configuration from `BattleShips.API/appsettings.json` and environment
overrides.

Required configuration:

- `ConnectionStrings:Db-ConnString` - SQL Server connection string used by
  `BattleShipsContext`.
- `AzureAd` - Azure AD B2C settings used by `AddMicrosoftIdentityWebApi`.

The default local connection string uses SQL Server LocalDB:

```json
"ConnectionStrings": {
  "Db-ConnString": "Data source = (localdb)\\MSSQLLocalDB;Initial Catalog = GameDb;Integrated Security=True;"
}
```

For environment variables, use .NET's double-underscore format, for example:

```bash
ConnectionStrings__Db-ConnString="Server=localhost;Database=GameDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Build and Run

Install the .NET 10 SDK, then restore and build the solution:

```bash
dotnet restore
dotnet build
```

Run the API locally:

```bash
dotnet run --project BattleShips.API
```

The development launch profile uses:

- HTTPS: `https://localhost:7235`
- HTTP: `http://localhost:5235`
- Swagger UI: `https://localhost:7235/swagger`
- Health check: `https://localhost:7235/health`

## Database

The API registers `BattleShipsContext` with SQL Server and exposes repositories for
games, boards, players, ships, ship types and shots.

There are no Entity Framework migrations currently checked into the repository. If the
database schema is not present locally, create or migrate it before running the API.

## Authentication and CORS

All API controllers are protected with `[Authorize]` and expect a valid Azure AD B2C JWT
bearer token.

`PlayerController` also verifies the `battleships.api` scope before returning or creating
the current player profile.

The configured CORS policy allows:

- `https://bsstaticstorage.z6.web.core.windows.net`
- `http://localhost:4200`

## API Routes

Player routes:

- `GET /api/player/get` - get or create the current authenticated player.

Game routes:

- `POST /api/game/create` - create a new game for the authenticated player.
- `POST /api/game/join/{gameCode}` - join a game by game code.
- `POST /api/game/addships` - add ships to a board and ready the player.
- `POST /api/game/fire` - fire at a board coordinate.
- `GET /api/game/ready/{gameId}` - poll lobby ready status.
- `POST /api/game/start/{gameId}` - set the game start time.
- `GET /api/game/state/{gameId}/{hostId}` - get the host and opponent game state.
- `GET /api/game/last/{gameId}` - get the most recent shot for a game.

## Testing

Run all tests with:

```bash
dotnet test
```

The test projects target .NET 10 and use NUnit with FakeItEasy.

## CI/CD and Infrastructure

`azure-pipelines.yml` contains three stages:

- `package` - restores, tests and publishes the API.
- `azure` - runs Terraform from the `Build` directory.
- `publish` - deploys the published package to the `battleships-api` Azure Web App.

Note: the pipeline currently requests the .NET `8.x` SDK, while the projects target
`net10.0`. Update the pipeline SDK version before relying on CI for .NET 10 builds.

## Docker

The repository contains a `dockerfile`, but it still references .NET 7 SDK/runtime images
and an entrypoint that does not match the current API assembly. Update it to .NET 10
before using Docker for this project.
