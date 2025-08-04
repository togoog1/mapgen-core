# MapGen Service Runner Script (PowerShell)
# This script runs the MapGen service with hot reload enabled

Write-Host "🚀 Starting MapGen Service..." -ForegroundColor Green
Write-Host "📁 Project: MapGen.Service" -ForegroundColor Cyan
Write-Host "🔥 Hot reload: Enabled" -ForegroundColor Yellow
Write-Host ""

# Change to the service directory
Set-Location MapGen.Service

# Check if the project exists
if (-not (Test-Path "MapGen.Service.csproj")) {
    Write-Host "❌ Error: MapGen.Service.csproj not found in MapGen.Service directory" -ForegroundColor Red
    exit 1
}

# Run the service with hot reload
Write-Host "🔄 Starting dotnet watch run..." -ForegroundColor Green
Write-Host "📡 Service will be available at: http://localhost:5042" -ForegroundColor Cyan
Write-Host "📚 Swagger UI: http://localhost:5042/swagger" -ForegroundColor Cyan
Write-Host "🏥 Health check: http://localhost:5042/api/map/health" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Press Ctrl+C to stop the service" -ForegroundColor Yellow
Write-Host ""

dotnet watch run 