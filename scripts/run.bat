@echo off
REM MapGen Service Runner Script (Windows)
REM This script runs the MapGen service with hot reload enabled

echo 🚀 Starting MapGen Service...
echo 📁 Project: MapGen.Service
echo 🔥 Hot reload: Enabled
echo.

REM Change to the service directory
cd MapGen.Service

REM Check if the project exists
if not exist "MapGen.Service.csproj" (
    echo ❌ Error: MapGen.Service.csproj not found in MapGen.Service directory
    exit /b 1
)

REM Run the service with hot reload
echo 🔄 Starting dotnet watch run...
echo 📡 Service will be available at: http://localhost:5042
echo 📚 Swagger UI: http://localhost:5042/swagger
echo 🏥 Health check: http://localhost:5042/api/map/health
echo.
echo 💡 Press Ctrl+C to stop the service
echo.

dotnet watch run 