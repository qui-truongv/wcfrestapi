# QMS RESTful API

Queue Management System RESTful API built with .NET 8

## Prerequisites

- .NET 8 SDK
- SQL Server 2019+
- Visual Studio 2022 / VS Code / Rider

## Getting Started

### 1. Clone the repository
```bash
git clone <repository-url>
cd QMS.RestfulAPI
```

### 2. Update Connection String

Update `appsettings.Development.json` with your SQL Server connection string

### 3. Run Database Migrations
```bash
cd src/QMS.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../QMS.API
dotnet ef database update --startup-project ../QMS.API
```

### 4. Run the API
```bash
cd ../QMS.API
dotnet run
```

API will be available at: `https://localhost:7xxx`
Swagger UI: `https://localhost:7xxx/swagger`

## Project Structure
```
QMS.RestfulAPI/
+-- src/
¦   +-- QMS.API/              # API Layer
¦   +-- QMS.Core/             # Domain Layer
¦   +-- QMS.Application/      # Application Layer
¦   +-- QMS.Infrastructure/   # Infrastructure Layer
¦   +-- QMS.Shared/           # Shared Utilities
+-- tests/
    +-- QMS.UnitTests/
    +-- QMS.IntegrationTests/
```

## API Versioning

API supports versioning via URL segments:
- V1: `/api/v1/[controller]`
- V2: `/api/v2/[controller]`

## Authentication

API uses JWT Bearer token authentication.

## Features

- ? Clean Architecture
- ? API Versioning
- ? JWT Authentication
- ? Swagger Documentation
- ? Health Checks
- ? Response Compression
- ? CORS Support
- ? Structured Logging (Serilog)
- ? Exception Handling Middleware
- ? SignalR Support

## License

MIT