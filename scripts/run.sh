#!/bin/bash

# MapGen Service Runner Script
# This script runs the MapGen service with hot reload enabled

set -e

echo "🚀 Starting MapGen Service..."
echo "📁 Project: MapGen.Service"
echo "🔥 Hot reload: Enabled"
echo ""

# Change to the service directory
cd MapGen.Service

# Check if the project exists
if [ ! -f "MapGen.Service.csproj" ]; then
    echo "❌ Error: MapGen.Service.csproj not found in MapGen.Service directory"
    exit 1
fi

# Run the service with hot reload
echo "🔄 Starting dotnet watch run..."
echo "📡 Service will be available at: http://localhost:5042"
echo "📚 Swagger UI: http://localhost:5042/swagger"
echo "🏥 Health check: http://localhost:5042/api/map/health"
echo ""
echo "💡 Press Ctrl+C to stop the service"
echo ""

dotnet watch run 