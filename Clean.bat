@echo off
echo ========================================
echo  Cleaning Docker Containers and Volumes
echo ========================================
echo.

echo Stopping all containers...
docker-compose down

echo.
echo Removing all project containers (if any)...
docker rm -f contract-api proposal-api rabbitmq sqlserver 2>nul

echo.
echo Removing all project images...
docker rmi -f contract-api:latest contract-api:dev proposal-api:latest proposal-api:dev 2>nul

echo.
echo Cleaning Docker system...
docker system prune -f

echo.
echo ========================================
echo  Cleanup Complete!
echo ========================================
echo.
echo You can now run Start.bat or press F5 in Visual Studio
echo.
pause
