@echo off
echo ========================================
echo  Insurance System - Service Status
echo ========================================
echo.

echo Checking Docker containers...
echo.
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

echo.
echo ========================================
echo  Testing Health Endpoints
echo ========================================
echo.

echo [ProposalService] Testing http://localhost:5001/health...
curl -s http://localhost:5001/health
echo.

echo [ContractService] Testing http://localhost:5002/health...
curl -s http://localhost:5002/health
echo.

echo.
echo ========================================
echo  Access URLs
echo ========================================
echo.
echo ProposalService Swagger: http://localhost:5001/swagger
echo ContractService Swagger: http://localhost:5002/swagger
echo RabbitMQ Management:     http://localhost:15672 (guest/guest)
echo.

pause
