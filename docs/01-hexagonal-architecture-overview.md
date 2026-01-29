# Hexagonal Architecture Overview - Insurance System

## System Architecture Diagram

```mermaid
graph TB
    subgraph "External World"
        HTTP[HTTP Clients<br/>Swagger/Postman]
        RMQ[RabbitMQ<br/>Message Broker]
        DB[(SQL Server<br/>Database)]
    end

    subgraph "ProposalService Hexagon"
        direction TB

        subgraph "Primary Adapters (Input)"
            API1[ProposalService.API<br/>Minimal API Endpoints]
        end

        subgraph "Application Layer"
            APP1[ProposalService.Application<br/>Commands & Queries<br/>MediatR Handlers<br/>FluentValidation]
        end

        subgraph "Domain Layer (Core Hexagon)"
            DOM1[ProposalService.Domain<br/>Entities: Proposal<br/>Value Objects: CPF, Money<br/>Ports: IProposalRepository<br/>Domain Events]
        end

        subgraph "Secondary Adapters (Output)"
            INFRA1[ProposalService.Infrastructure<br/>EF Core Repository<br/>RabbitMQ Publisher]
        end

        API1 --> APP1
        APP1 --> DOM1
        DOM1 -.defines ports.-> INFRA1
        INFRA1 -.implements ports.-> DOM1
    end

    subgraph "ContractService Hexagon"
        direction TB

        subgraph "Primary Adapters (Input)"
            API2[ContractService.API<br/>Minimal API Endpoints]
        end

        subgraph "Application Layer"
            APP2[ContractService.Application<br/>Commands & Queries<br/>MediatR Handlers<br/>FluentValidation]
        end

        subgraph "Domain Layer (Core Hexagon)"
            DOM2[ContractService.Domain<br/>Entities: Contract<br/>Value Objects: CustomerInfo, Money<br/>Ports: IContractRepository<br/>IProposalServiceClient]
        end

        subgraph "Secondary Adapters (Output)"
            INFRA2[ContractService.Infrastructure<br/>EF Core Repository<br/>HTTP Client with Polly<br/>RabbitMQ Consumer]
        end

        API2 --> APP2
        APP2 --> DOM2
        DOM2 -.defines ports.-> INFRA2
        INFRA2 -.implements ports.-> DOM2
    end

    subgraph "Shared Building Blocks"
        SHARED[SharedKernel<br/>Base Entity<br/>Value Object<br/>Domain Event<br/>Common Exceptions]
    end

    %% External connections
    HTTP --> API1
    HTTP --> API2

    INFRA1 --> DB
    INFRA2 --> DB

    INFRA1 --> RMQ
    INFRA2 --> RMQ

    %% Service to service communication
    INFRA2 -.HTTP REST<br/>with Polly Retry.-> API1

    %% Shared dependencies
    DOM1 -.uses.-> SHARED
    DOM2 -.uses.-> SHARED

    %% Styling
    classDef primaryAdapter fill:#4CAF50,stroke:#2E7D32,stroke-width:3px,color:#fff
    classDef application fill:#2196F3,stroke:#1565C0,stroke-width:3px,color:#fff
    classDef domain fill:#FF9800,stroke:#E65100,stroke-width:4px,color:#fff
    classDef secondaryAdapter fill:#9C27B0,stroke:#6A1B9A,stroke-width:3px,color:#fff
    classDef external fill:#607D8B,stroke:#37474F,stroke-width:2px,color:#fff
    classDef shared fill:#FFC107,stroke:#F57F17,stroke-width:2px,color:#000

    class API1,API2 primaryAdapter
    class APP1,APP2 application
    class DOM1,DOM2 domain
    class INFRA1,INFRA2 secondaryAdapter
    class HTTP,RMQ,DB external
    class SHARED shared
```

## Key Concepts

### Hexagonal Architecture (Ports & Adapters)

**Core Principles:**
- **Domain (Orange)**: Business logic at the center, independent of frameworks
- **Ports**: Interfaces defined by the domain (dashed lines)
- **Adapters**: Implementations of ports (Primary = Input, Secondary = Output)
- **Dependency Rule**: All dependencies point inward toward the domain

### ProposalService Hexagon
- **Primary Adapter**: Minimal API receives HTTP requests
- **Application**: MediatR orchestrates use cases (CQRS pattern)
- **Domain**: Proposal entity with business rules (Approve, Reject)
- **Secondary Adapters**: EF Core (persistence), RabbitMQ (events)

### ContractService Hexagon
- **Primary Adapter**: Minimal API receives HTTP requests
- **Application**: MediatR orchestrates use cases
- **Domain**: Contract entity, validates proposals via HTTP
- **Secondary Adapters**: EF Core, HTTP Client (Polly resilience), RabbitMQ

### Communication Patterns
- **Synchronous**: HTTP REST with Polly retry/circuit breaker
- **Asynchronous**: RabbitMQ for domain events
- **Shared Kernel**: Common base classes reused across services

### Benefits
✅ **Testability**: Mock ports for unit tests (no database needed)
✅ **Flexibility**: Swap SQL Server for MongoDB without changing domain
✅ **Maintainability**: Clear separation of concerns
✅ **Independence**: Domain has zero external dependencies

---

**Created for:** Technical Interview - Insurance System
**Architecture:** Hexagonal (Ports & Adapters)
**Pattern:** CQRS with MediatR
**Technology:** .NET 10, SQL Server, RabbitMQ, Docker
