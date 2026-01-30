# PowerShell script to restart failed containers
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Restarting Failed Containers" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get containers that are not running
$failedContainers = docker ps -a --filter "status=created" --filter "status=exited" --format "{{.Names}}"

if ($failedContainers) {
    Write-Host "Found failed containers:" -ForegroundColor Yellow
    $failedContainers | ForEach-Object {
        Write-Host "  - $_" -ForegroundColor Yellow
    }
    Write-Host ""
    
    foreach ($container in $failedContainers) {
        Write-Host "Restarting $container..." -ForegroundColor Cyan
        docker start $container
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ $container restarted successfully" -ForegroundColor Green
        } else {
            Write-Host "✗ Failed to restart $container" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host " Current Status" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    docker ps --format "table {{.Names}}\t{{.Status}}"
    
} else {
    Write-Host "✓ No failed containers found. All services are running!" -ForegroundColor Green
    Write-Host ""
    docker ps --format "table {{.Names}}\t{{.Status}}"
}

Write-Host ""
Read-Host "Press Enter to exit"
