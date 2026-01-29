FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/BuildingBlocks/SharedKernel/SharedKernel.csproj", "src/BuildingBlocks/SharedKernel/"]
COPY ["src/Services/ContractService/ContractService.Domain/ContractService.Domain.csproj", "src/Services/ContractService/ContractService.Domain/"]
COPY ["src/Services/ContractService/ContractService.Application/ContractService.Application.csproj", "src/Services/ContractService/ContractService.Application/"]
COPY ["src/Services/ContractService/ContractService.Infrastructure/ContractService.Infrastructure.csproj", "src/Services/ContractService/ContractService.Infrastructure/"]
COPY ["src/Services/ContractService/ContractService.API/ContractService.API.csproj", "src/Services/ContractService/ContractService.API/"]

# Restore dependencies
RUN dotnet restore "src/Services/ContractService/ContractService.API/ContractService.API.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/src/Services/ContractService/ContractService.API"
RUN dotnet build "ContractService.API.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "ContractService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ContractService.API.dll"]
