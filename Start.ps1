# PowerShell script to start the Insurance System with Docker Compose
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Starting Insurance System with Docker" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    docker-compose up --build
}
catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host " Error starting Docker Compose" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}
