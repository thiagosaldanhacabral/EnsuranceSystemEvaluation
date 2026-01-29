# Insurance System - Technical Evaluation Project

A production-ready insurance management system built with microservices architecture, demonstrating hexagonal architecture, CQRS patterns, and modern .NET development practices.

## Overview

This system manages insurance proposals and contracts through two microservices that communicate via HTTP REST (synchronous) and RabbitMQ (asynchronous events). It showcases clean architecture principles, domain-driven design, and enterprise patterns.

### Features

- **ProposalService**: Manage insurance proposals with complete lifecycle (creation, analysis, approval/rejection)
- **ContractService**: Generate contracts from approved proposals with validation and tracking
- **Event-Driven Communication**: RabbitMQ for reliable async messaging between services
- **Resilient HTTP Communication**: Polly-based retry and circuit breaker patterns
- **Comprehensive Testing**: Unit tests with 80%+ code coverage using xUnit and NSubstitute
- **Docker Support**: Full containerization with docker-compose orchestration
- **API Documentation**: Swagger/OpenAPI with detailed endpoint descriptions

## Architecture

### Hexagonal Architecture (Ports & Adapters)

Each service follows hexagonal architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                           │
│  (Minimal APIs, Swagger, Middleware, Health Checks)    │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                 Application Layer                       │
│   (Commands, Queries, Handlers, Validators, DTOs)     │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                  Domain Layer                           │
│  (Entities, Value Objects, Domain Events, Ports)       │
└─────────────────────────────────────────────────────────┘
        │                                          │
┌───────▼──────────────┐           ┌──────────────▼───────┐
│  Infrastructure      │           │  Infrastructure      │
│  (EF Core, SQL)     │           │  (RabbitMQ, HTTP)    │
└──────────────────────┘           └──────────────────────┘
```

### Technology Stack

- **.NET 10**: Latest framework with Minimal APIs and performance improvements
- **SQL Server 2022**: Relational database with Entity Framework Core 10
- **RabbitMQ 3.12**: Message broker with MassTransit abstraction
- **Docker & Docker Compose**: Containerization and orchestration
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **xUnit + NSubstitute**: Unit testing with mocking
- **FluentValidation**: Request validation with MediatR pipeline
- **FluentAssertions**: Readable test assertions

## Project Structure

```
EnsuranceSystemEvaluation/
├── src/
│   ├── BuildingBlocks/
│   │   └── SharedKernel/                    # Shared base classes, value objects
│   └── Services/
│       ├── ProposalService/
│       │   ├── ProposalService.Domain/      # Entities, value objects, ports
│       │   ├── ProposalService.Application/ # CQRS handlers, validators
│       │   ├── ProposalService.Infrastructure/ # EF Core, RabbitMQ adapters
│       │   └── ProposalService.API/         # Minimal APIs, middleware
│       └── ContractService/
│           ├── ContractService.Domain/
│           ├── ContractService.Application/
│           ├── ContractService.Infrastructure/
│           └── ContractService.API/
├── tests/
│   ├── ProposalService.UnitTests/           # Domain & application tests
│   ├── ProposalService.IntegrationTests/    # API & database tests
│   ├── ContractService.UnitTests/
│   └── ContractService.IntegrationTests/
├── docker-compose.yml                        # Container orchestration
├── docker-compose.override.yml              # Development overrides
├── Start.bat / Start.ps1                     # Quick start scripts
├── Stop.bat / Stop.ps1                       # Quick stop scripts
├── Status.bat / Status.ps1                   # Check services status
└── Directory.Build.props                     # Shared MSBuild configuration
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (10.0.102 or later)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (4.25+ recommended)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.9+ for best experience) OR [VS Code](https://code.visualstudio.com/)
- SQL Server Management Studio (optional, for database inspection)

### Quick Start with Docker Compose

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EnsuranceSystemEvaluation
   ```

2. **Start all services**
   
   **Option A - Using Shortcut Scripts (Recommended for Windows):**
   
   Simply double-click one of the following files in Windows Explorer:
   - `Start.bat` (for Command Prompt)
   - `Start.ps1` (for PowerShell)
   
   Or run from terminal:
   ```cmd
   # Command Prompt
   Start.bat
   ```
   ```powershell
   # PowerShell
   .\Start.ps1
   ```
   
   **Option B - Using Docker Compose directly:**
   ```bash
   docker-compose up --build
   ```

   This will start:
   - SQL Server on port `1433`
   - RabbitMQ on ports `5672` (AMQP) and `15672` (Management UI)
   - ProposalService API on port `5001`
   - ContractService API on port `5002`

3. **Access the APIs**
   - ProposalService Swagger: http://localhost:5001/swagger
   - ContractService Swagger: http://localhost:5002/swagger
   - RabbitMQ Management: http://localhost:15672 (guest/guest)

4. **Stop all services**
   
   **Option A - Using Shortcut Scripts:**
   - Double-click `Stop.bat` or `Stop.ps1`
   - Or run: `Stop.bat` / `.\Stop.ps1`
   
   **Option B - Using Docker Compose:**
   ```bash
   docker-compose down
   ```

   To also remove volumes:
   ```bash
   docker-compose down -v
   ```

5. **Check services status** (Optional)
   
   To verify all services are running correctly:
   - Double-click `Status.bat` or `Status.ps1`
   - Or run: `Status.bat` / `.\Status.ps1`
   
   This will show:
   - Docker container status
   - Health check results for all APIs
   - Direct access URLs for Swagger and RabbitMQ

### Visual Studio Integration

1. Open `EnsuranceSystemEvaluation.sln` in Visual Studio 2022+
2. Set `docker-compose` as the startup project
3. Press **F5** to start debugging
   - All containers start automatically
   - Debugger attaches to the API containers
   - Swagger UI opens automatically
4. Press **Shift+F5** or click Stop to stop all containers

### Local Development (Without Docker)

1. **Start SQL Server**
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
   ```

2. **Start RabbitMQ**
   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management
   ```

3. **Run migrations** (automatically applied on startup in Development mode)

4. **Start ProposalService**
   ```bash
   cd src/Services/ProposalService/ProposalService.API
   dotnet run
   ```

   API available at: http://localhost:5001

5. **Start ContractService** (in another terminal)
   ```bash
   cd src/Services/ContractService/ContractService.API
   dotnet run
   ```

   API available at: http://localhost:5002

## API Usage Examples

### Create a Proposal

```bash
curl -X POST http://localhost:5001/api/proposals \
  -H "Content-Type: application/json" \
  -d '{
    "customerCPF": "11144477735",
    "customerName": "John Doe",
    "insuranceValue": 50000
  }'
```

Response:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "proposalNumber": "PROP-20260129-00001",
  "customerName": "John Doe",
  "customerCPF": "11144477735",
  "insuranceValue": 50000.00,
  "status": "InAnalysis",
  "createdAt": "2026-01-29T10:30:00Z"
}
```

### Approve a Proposal

```bash
curl -X PATCH http://localhost:5001/api/proposals/3fa85f64-5717-4562-b3fc-2c963f66afa6/status \
  -H "Content-Type: application/json" \
  -d '{
    "newStatus": "Approved"
  }'
```

### Create a Contract from Approved Proposal

```bash
curl -X POST http://localhost:5002/api/contracts \
  -H "Content-Type: application/json" \
  -d '{
    "proposalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "durationMonths": 12
  }'
```

Response:
```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "proposalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "contractNumber": "CONT-20260129-00001",
  "customerName": "John Doe",
  "customerCPF": "11144477735",
  "premium": 1000.00,
  "contractDate": "2026-01-29T10:35:00Z",
  "startDate": "2026-01-30",
  "endDate": "2027-01-30"
}
```

### List All Contracts (Paginated)

```bash
curl "http://localhost:5002/api/contracts?pageNumber=1&pageSize=20"
```

## Running Tests

### Unit Tests

Run all unit tests:
```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```

Run tests for specific service:
```bash
dotnet test tests/ProposalService.UnitTests
dotnet test tests/ContractService.UnitTests
```

### Integration Tests (with Testcontainers)

```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Code Coverage

Generate coverage report:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=80

# Install report generator (once)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:TestResults/Report -reporttypes:Html

# Open report
start TestResults/Report/index.html
```

## Database Migrations

### Create a new migration

ProposalService:
```bash
cd src/Services/ProposalService/ProposalService.Infrastructure
dotnet ef migrations add MigrationName --context ProposalDbContext
```

ContractService:
```bash
cd src/Services/ContractService/ContractService.Infrastructure
dotnet ef migrations add MigrationName --context ContractDbContext
```

### Apply migrations manually

```bash
dotnet ef database update --context ProposalDbContext
dotnet ef database update --context ContractDbContext
```

Note: Migrations are automatically applied on application startup in Development and Docker environments.

## Key Design Patterns

### CQRS (Command Query Responsibility Segregation)

Separate read and write operations for better scalability and maintainability:

- **Commands**: Modify state (CreateProposal, UpdateProposalStatus, ContractProposal)
- **Queries**: Read data (GetProposalById, ListProposals, ListContracts)
- **MediatR**: Mediates between API and handlers with validation pipeline

### Repository Pattern

Abstracts data access layer:
```csharp
public interface IProposalRepository
{
    Task<Proposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Proposal>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default);
    Task UpdateAsync(Proposal proposal, CancellationToken cancellationToken = default);
}
```

### Domain Events

Publish domain events for cross-service communication:
```csharp
public sealed record ProposalApprovedEvent : DomainEvent
{
    public Guid ProposalId { get; init; }
    public string ProposalNumber { get; init; }
    public DateTime ApprovedAt { get; init; }
}
```

### Value Objects

Encapsulate validation and behavior:
```csharp
public sealed class CPF : ValueObject
{
    public string Number { get; }

    public static CPF Create(string cpf)
    {
        // Validates CPF format and check digits
        // Returns immutable instance
    }

    public string Formatted => // Returns ###.###.###-##
}
```

### Resilience Patterns (Polly)

HTTP communication with retry and circuit breaker:
```csharp
services.AddHttpClient<IProposalServiceClient, ProposalServiceClient>()
    .AddPolicyHandler(GetRetryPolicy())        // 3 retries with exponential backoff
    .AddPolicyHandler(GetCircuitBreakerPolicy()); // Opens after 5 consecutive failures
```

## Performance Optimizations

### Database
- **Indexes**: Strategic indexes on frequently queried columns (ProposalNumber, CustomerCPF, Status)
- **AsNoTracking()**: Used for read-only queries to improve performance
- **Pagination**: Default page size 20, max 100 to limit memory usage
- **Connection pooling**: Configured automatically by EF Core

### API
- **Response Compression**: Gzip enabled for all responses
- **Minimal APIs**: Lower overhead compared to traditional controllers
- **Async/await**: All I/O operations are asynchronous

### EF Core
- **Compiled queries**: For frequently executed queries
- **Query splitting**: For complex queries with includes
- **No-tracking queries**: Default for read operations

## Security Considerations

### Implemented
- **Input validation**: FluentValidation on all commands/queries
- **SQL injection prevention**: Parameterized queries via EF Core
- **CORS configuration**: Restrictive CORS in production (currently permissive for demo)
- **Health checks**: Monitor service and database health

### For Production (Not Implemented in Demo)
- Authentication/Authorization (JWT, OAuth2)
- API rate limiting
- Sensitive data encryption
- Secrets management (Azure Key Vault, AWS Secrets Manager)
- HTTPS enforcement

## Monitoring & Observability

### Logging

Structured logging with Serilog:
```csharp
Log.Information("Processing proposal {ProposalId} for customer {CustomerName}",
    proposalId, customerName);
```

Logs are written to console in JSON format for easy parsing by log aggregators (ELK, Datadog, etc.)

### Health Checks

Available endpoints:
- `/health` - Overall system health
- `/health/ready` - Kubernetes readiness probe

Checks include:
- Database connectivity
- RabbitMQ connectivity (in full implementation)

### Metrics (Future Enhancement)

Recommended additions:
- Application Insights / Prometheus metrics
- Distributed tracing (OpenTelemetry)
- Performance counters

## Troubleshooting

### Docker containers won't start

1. Check Docker Desktop is running
2. Verify ports are not in use:
   ```bash
   netstat -ano | findstr ":5001"
   netstat -ano | findstr ":5002"
   netstat -ano | findstr ":1433"
   ```
3. Remove existing containers and volumes:
   ```bash
   docker-compose down -v
   docker-compose up --build
   ```

### Database migration errors

Reset database:
```bash
docker-compose down -v
docker-compose up --build
```

Migrations will auto-apply on startup.

### RabbitMQ connection issues

1. Check RabbitMQ is running: http://localhost:15672
2. Verify credentials in appsettings.Docker.json (guest/guest)
3. Restart RabbitMQ container:
   ```bash
   docker restart rabbitmq
   ```

### Tests failing with CPF validation errors

The system validates Brazilian CPF numbers using check digits. Use valid CPFs in tests:
- Valid CPF 1: `11144477735`
- Valid CPF 2: `52998224725`

## Contributing

This is a technical evaluation project. Contributions are not expected, but feedback is welcome.

## License

This project is for educational and evaluation purposes only.

## Contact

For questions about this implementation:
- Create an issue in the repository
- Contact the development team

---

**Built using .NET 10, Clean Architecture, and Domain-Driven Design principles**
