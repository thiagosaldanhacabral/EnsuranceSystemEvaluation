FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/BuildingBlocks/SharedKernel/SharedKernel.csproj", "src/BuildingBlocks/SharedKernel/"]
COPY ["src/Services/ProposalService/ProposalService.Domain/ProposalService.Domain.csproj", "src/Services/ProposalService/ProposalService.Domain/"]
COPY ["src/Services/ProposalService/ProposalService.Application/ProposalService.Application.csproj", "src/Services/ProposalService/ProposalService.Application/"]
COPY ["src/Services/ProposalService/ProposalService.Infrastructure/ProposalService.Infrastructure.csproj", "src/Services/ProposalService/ProposalService.Infrastructure/"]
COPY ["src/Services/ProposalService/ProposalService.API/ProposalService.API.csproj", "src/Services/ProposalService/ProposalService.API/"]

# Restore dependencies
RUN dotnet restore "src/Services/ProposalService/ProposalService.API/ProposalService.API.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/src/Services/ProposalService/ProposalService.API"
RUN dotnet build "ProposalService.API.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "ProposalService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ProposalService.API.dll"]
