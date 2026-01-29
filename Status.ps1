# PowerShell script to check Insurance System status
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Insurance System - Service Status" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Checking Docker containers..." -ForegroundColor Yellow
Write-Host ""
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Testing Health Endpoints" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    Write-Host "[ProposalService] Testing http://localhost:5001/health..." -ForegroundColor Yellow
    $response1 = Invoke-WebRequest -Uri "http://localhost:5001/health" -UseBasicParsing
    Write-Host "✓ Status: $($response1.StatusCode) - $($response1.Content)" -ForegroundColor Green
}
catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

try {
    Write-Host "[ContractService] Testing http://localhost:5002/health..." -ForegroundColor Yellow
    $response2 = Invoke-WebRequest -Uri "http://localhost:5002/health" -UseBasicParsing
    Write-Host "✓ Status: $($response2.StatusCode) - $($response2.Content)" -ForegroundColor Green
}
catch {
    Write-Host "✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Access URLs" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "ProposalService Swagger: " -NoNewline
Write-Host "http://localhost:5001/swagger" -ForegroundColor Blue
Write-Host "ContractService Swagger: " -NoNewline
Write-Host "http://localhost:5002/swagger" -ForegroundColor Blue
Write-Host "RabbitMQ Management:     " -NoNewline
Write-Host "http://localhost:15672" -ForegroundColor Blue -NoNewline
Write-Host " (guest/guest)" -ForegroundColor Gray
Write-Host ""

Read-Host "Press Enter to exit"
