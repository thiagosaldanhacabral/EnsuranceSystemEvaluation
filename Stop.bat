@echo off
echo ========================================
echo  Stopping Insurance System
echo ========================================
echo.

docker-compose stop

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ========================================
    echo  Error stopping Docker Compose
    echo ========================================
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================
echo  System stopped successfully
echo ========================================
pause
