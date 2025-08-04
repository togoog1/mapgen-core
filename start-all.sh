#!/bin/bash

# MapGen Playground - Start All Services
# This script starts all three parts of the MapGen project:
# 1. Core (tests with watch)
# 2. Service (API with hot reload)
# 3. Viewer (React dev server)

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[MapGen]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[MapGen]${NC} $1"
}

print_error() {
    echo -e "${RED}[MapGen]${NC} $1"
}

print_info() {
    echo -e "${BLUE}[MapGen]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
    print_status "Checking prerequisites..."
    
    # Check .NET SDK
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK not found. Please install .NET 9 SDK."
        exit 1
    fi
    
    # Check Node.js
    if ! command -v node &> /dev/null; then
        print_error "Node.js not found. Please install Node.js 20 LTS."
        exit 1
    fi
    
    # Check npm
    if ! command -v npm &> /dev/null; then
        print_error "npm not found. Please install npm."
        exit 1
    fi
    
    print_status "Prerequisites check passed ✓"
}

# Setup dependencies
setup_dependencies() {
    print_status "Setting up dependencies..."
    
    # Install npm dependencies for Viewer
    if [ -d "Viewer" ] && [ -f "Viewer/package.json" ]; then
        print_info "Installing Viewer dependencies..."
        cd Viewer
        npm install
        cd ..
        print_status "Viewer dependencies installed ✓"
    else
        print_warning "Viewer directory or package.json not found"
    fi
    
    # Restore .NET dependencies
    if [ -d "Core" ]; then
        print_info "Restoring Core dependencies..."
        cd Core
        dotnet restore
        cd ..
        print_status "Core dependencies restored ✓"
    fi
    
    if [ -d "Service" ]; then
        print_info "Restoring Service dependencies..."
        cd Service
        dotnet restore
        cd ..
        print_status "Service dependencies restored ✓"
    fi
}

# Start services based on platform and terminal
start_services() {
    print_status "Starting MapGen services..."
    
    # Detect platform and terminal
    PLATFORM=$(uname)
    
    if [[ "$PLATFORM" == "Darwin" ]]; then
        # macOS
        start_services_macos
    elif [[ "$PLATFORM" == "Linux" ]]; then
        # Linux
        start_services_linux
    else
        # Fallback - just show manual commands
        print_warning "Platform not detected. Please run these commands manually in separate terminals:"
        show_manual_commands
    fi
}

start_services_macos() {
    print_info "Starting services on macOS..."
    
    # Terminal 1: Core tests
    if [ -d "Core" ]; then
        osascript -e "tell application \"Terminal\" to do script \"cd '$(pwd)/Core' && echo 'Starting Core tests with watch...' && dotnet watch test\""
        print_status "Core tests started in new Terminal tab"
    fi
    
    # Terminal 2: Service API
    if [ -d "Service" ]; then
        sleep 2
        osascript -e "tell application \"Terminal\" to do script \"cd '$(pwd)/Service/MapGen.Service' && echo 'Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023\""
        print_status "Service API started in new Terminal tab"
    fi
    
    # Terminal 3: Viewer
    if [ -d "Viewer" ]; then
        sleep 4
        osascript -e "tell application \"Terminal\" to do script \"cd '$(pwd)/Viewer' && echo 'Starting Viewer development server...' && npm run dev\""
        print_status "Viewer started in new Terminal tab"
    fi
}

start_services_linux() {
    print_info "Starting services on Linux..."
    
    # Check if we're in a graphical environment
    if [ -n "$DISPLAY" ]; then
        # Try different terminal emulators
        if command -v gnome-terminal &> /dev/null; then
            start_with_gnome_terminal
        elif command -v xterm &> /dev/null; then
            start_with_xterm
        elif command -v konsole &> /dev/null; then
            start_with_konsole
        else
            print_warning "No supported terminal emulator found. Please run commands manually:"
            show_manual_commands
        fi
    else
        print_warning "No display detected. Please run these commands manually in separate terminals:"
        show_manual_commands
    fi
}

start_with_gnome_terminal() {
    # Core tests
    if [ -d "Core" ]; then
        gnome-terminal --tab --title="MapGen Core Tests" -- bash -c "cd '$(pwd)/Core' && echo 'Starting Core tests with watch...' && dotnet watch test; exec bash"
    fi
    
    # Service API
    if [ -d "Service" ]; then
        gnome-terminal --tab --title="MapGen Service" -- bash -c "cd '$(pwd)/Service/MapGen.Service' && echo 'Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023; exec bash"
    fi
    
    # Viewer
    if [ -d "Viewer" ]; then
        gnome-terminal --tab --title="MapGen Viewer" -- bash -c "cd '$(pwd)/Viewer' && echo 'Starting Viewer development server...' && npm run dev; exec bash"
    fi
}

start_with_xterm() {
    # Core tests
    if [ -d "Core" ]; then
        xterm -title "MapGen Core Tests" -e "cd '$(pwd)/Core' && echo 'Starting Core tests with watch...' && dotnet watch test; bash" &
    fi
    
    # Service API
    if [ -d "Service" ]; then
        xterm -title "MapGen Service" -e "cd '$(pwd)/Service/MapGen.Service' && echo 'Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023; bash" &
    fi
    
    # Viewer
    if [ -d "Viewer" ]; then
        xterm -title "MapGen Viewer" -e "cd '$(pwd)/Viewer' && echo 'Starting Viewer development server...' && npm run dev; bash" &
    fi
}

start_with_konsole() {
    # Core tests
    if [ -d "Core" ]; then
        konsole --new-tab --title "MapGen Core Tests" -e bash -c "cd '$(pwd)/Core' && echo 'Starting Core tests with watch...' && dotnet watch test; exec bash" &
    fi
    
    # Service API
    if [ -d "Service" ]; then
        konsole --new-tab --title "MapGen Service" -e bash -c "cd '$(pwd)/Service/MapGen.Service' && echo 'Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023; exec bash" &
    fi
    
    # Viewer
    if [ -d "Viewer" ]; then
        konsole --new-tab --title "MapGen Viewer" -e bash -c "cd '$(pwd)/Viewer' && echo 'Starting Viewer development server...' && npm run dev; exec bash" &
    fi
}

show_manual_commands() {
    echo ""
    print_info "=== MANUAL STARTUP COMMANDS ==="
    echo ""
    print_info "Terminal 1 - Core Tests:"
    echo "cd Core && dotnet watch test"
    echo ""
    print_info "Terminal 2 - Service API:"
    echo "cd Service/MapGen.Service && dotnet watch run --urls=http://localhost:5023"
    echo ""
    print_info "Terminal 3 - Viewer:"
    echo "cd Viewer && npm run dev"
    echo ""
}

show_urls() {
    echo ""
    print_status "=== MAPGEN SERVICES ==="
    echo ""
    print_info "🧪 Core Tests: Running in watch mode"
    print_info "🚀 Service API: http://localhost:5023"
    print_info "📱 Viewer: http://localhost:5173"
    print_info "📚 API Docs: http://localhost:5023/swagger"
    echo ""
    print_status "=== DEVELOPMENT WORKFLOW ==="
    echo "1. Edit any C# file in Core → tests rerun, service rebuilds"
    echo "2. Edit any React file in Viewer → browser hot reloads"
    echo "3. Visit Viewer to test your generators interactively"
    echo ""
    print_warning "Press Ctrl+C in each terminal to stop the services"
}

# Main execution
main() {
    echo ""
    print_status "🎮 MapGen Playground Launcher"
    echo ""
    
    check_prerequisites
    setup_dependencies
    start_services
    
    sleep 6  # Give services time to start
    show_urls
}

# Handle script arguments
case "${1:-}" in
    --help|-h)
        echo "MapGen Playground Launcher"
        echo ""
        echo "Usage: $0 [OPTIONS]"
        echo ""
        echo "Options:"
        echo "  --help, -h    Show this help message"
        echo "  --manual, -m  Show manual startup commands only"
        echo ""
        echo "This script starts all three MapGen services:"
        echo "  • Core (tests with watch)"
        echo "  • Service (API with hot reload)"
        echo "  • Viewer (React dev server)"
        exit 0
        ;;
    --manual|-m)
        show_manual_commands
        exit 0
        ;;
    *)
        main
        ;;
esac 