@echo off
echo ========================================
echo  Starting Insurance System with Docker
echo ========================================
echo.

docker-compose up --build

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ========================================
    echo  Error starting Docker Compose
    echo ========================================
    pause
    exit /b %ERRORLEVEL%
)
