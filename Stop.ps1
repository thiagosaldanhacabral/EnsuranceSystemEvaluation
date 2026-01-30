# PowerShell script to stop the Insurance System
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Stopping Insurance System" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    docker-compose stop
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host " System stopped successfully" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host " Error stopping Docker Compose" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Read-Host "Press Enter to exit"
