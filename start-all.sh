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

print_url() {
    echo -e "${GREEN}[MapGen]${NC} 🌐 $1"
}

# Check for existing processes and kill them
check_and_kill_existing() {
    print_status "Checking for existing MapGen processes..."
    
    # Kill existing dotnet watch processes
    if pgrep -f "dotnet watch" > /dev/null; then
        print_warning "Found existing dotnet watch processes. Stopping them..."
        pkill -f "dotnet watch"
        sleep 2
    fi
    
    # Kill existing npm dev processes
    if pgrep -f "npm run dev" > /dev/null; then
        print_warning "Found existing npm dev processes. Stopping them..."
        pkill -f "npm run dev"
        sleep 2
    fi
    
    # Kill processes on specific ports
    if lsof -ti:5023 > /dev/null 2>&1; then
        print_warning "Port 5023 is in use. Stopping process..."
        lsof -ti:5023 | xargs kill -9
        sleep 1
    fi
    
    if lsof -ti:5173 > /dev/null 2>&1; then
        print_warning "Port 5173 is in use. Stopping process..."
        lsof -ti:5173 | xargs kill -9
        sleep 1
    fi
    
    print_status "Existing processes cleaned up ✓"
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
        osascript -e "tell application \"Terminal\" to do script \"cd '$(pwd)/Core' && echo '🧪 Starting Core tests with watch...' && dotnet watch test\""
        print_status "Core tests started in new Terminal window"
    fi
    
    # Terminal 2: Service API
    if [ -d "Service" ]; then
        sleep 2
        osascript -e "tell application \"Terminal\" to do script \"cd '$(pwd)/Service/MapGen.Service' && echo '🚀 Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023\""
        print_status "Service API started in new Terminal window"
        print_url "Service API: http://localhost:5023"
        print_url "API Docs: http://localhost:5023/swagger"
    fi
    
    # Terminal 3: Viewer
    if [ -d "Viewer" ]; then
        sleep 4
        osascript -e "tell application \"Terminal\" to do script \"cd '$(pwd)/Viewer' && echo '📱 Starting Viewer development server...' && npm run dev\""
        print_status "Viewer started in new Terminal window"
        print_url "Viewer: http://localhost:5173"
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
        gnome-terminal --window --title="MapGen Core Tests" -- bash -c "cd '$(pwd)/Core' && echo '🧪 Starting Core tests with watch...' && dotnet watch test; exec bash"
        print_status "Core tests started in new Terminal window"
    fi
    
    # Service API
    if [ -d "Service" ]; then
        sleep 2
        gnome-terminal --window --title="MapGen Service" -- bash -c "cd '$(pwd)/Service/MapGen.Service' && echo '🚀 Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023; exec bash"
        print_status "Service API started in new Terminal window"
        print_url "Service API: http://localhost:5023"
        print_url "API Docs: http://localhost:5023/swagger"
    fi
    
    # Viewer
    if [ -d "Viewer" ]; then
        sleep 4
        gnome-terminal --window --title="MapGen Viewer" -- bash -c "cd '$(pwd)/Viewer' && echo '📱 Starting Viewer development server...' && npm run dev; exec bash"
        print_status "Viewer started in new Terminal window"
        print_url "Viewer: http://localhost:5173"
    fi
}

start_with_xterm() {
    # Core tests
    if [ -d "Core" ]; then
        xterm -title "MapGen Core Tests" -e "cd '$(pwd)/Core' && echo '🧪 Starting Core tests with watch...' && dotnet watch test; bash" &
        print_status "Core tests started in new Terminal window"
    fi
    
    # Service API
    if [ -d "Service" ]; then
        sleep 2
        xterm -title "MapGen Service" -e "cd '$(pwd)/Service/MapGen.Service' && echo '🚀 Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023; bash" &
        print_status "Service API started in new Terminal window"
        print_url "Service API: http://localhost:5023"
        print_url "API Docs: http://localhost:5023/swagger"
    fi
    
    # Viewer
    if [ -d "Viewer" ]; then
        sleep 4
        xterm -title "MapGen Viewer" -e "cd '$(pwd)/Viewer' && echo '📱 Starting Viewer development server...' && npm run dev; bash" &
        print_status "Viewer started in new Terminal window"
        print_url "Viewer: http://localhost:5173"
    fi
}

start_with_konsole() {
    # Core tests
    if [ -d "Core" ]; then
        konsole --new-window --title "MapGen Core Tests" -e bash -c "cd '$(pwd)/Core' && echo '🧪 Starting Core tests with watch...' && dotnet watch test; exec bash" &
        print_status "Core tests started in new Terminal window"
    fi
    
    # Service API
    if [ -d "Service" ]; then
        sleep 2
        konsole --new-window --title "MapGen Service" -e bash -c "cd '$(pwd)/Service/MapGen.Service' && echo '🚀 Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023; exec bash" &
        print_status "Service API started in new Terminal window"
        print_url "Service API: http://localhost:5023"
        print_url "API Docs: http://localhost:5023/swagger"
    fi
    
    # Viewer
    if [ -d "Viewer" ]; then
        sleep 4
        konsole --new-window --title "MapGen Viewer" -e bash -c "cd '$(pwd)/Viewer' && echo '📱 Starting Viewer development server...' && npm run dev; exec bash" &
        print_status "Viewer started in new Terminal window"
        print_url "Viewer: http://localhost:5173"
    fi
}

show_manual_commands() {
    echo ""
    print_info "=== MANUAL STARTUP COMMANDS ==="
    echo ""
    print_info "Terminal 1 - Core Tests:"
    echo "cd Core && echo '🧪 Starting Core tests with watch...' && dotnet watch test"
    echo ""
    print_info "Terminal 2 - Service API:"
    echo "cd Service/MapGen.Service && echo '🚀 Starting Service API with hot reload...' && dotnet watch run --urls=http://localhost:5023"
    echo ""
    print_info "Terminal 3 - Viewer:"
    echo "cd Viewer && echo '📱 Starting Viewer development server...' && npm run dev"
    echo ""
}

show_urls() {
    echo ""
    print_status "=== MAPGEN SERVICES ==="
    echo ""
    print_info "🧪 Core Tests: Running in watch mode"
    print_url "🚀 Service API: http://localhost:5023"
    print_url "📚 API Docs: http://localhost:5023/swagger"
    print_url "📱 Viewer: http://localhost:5173"
    echo ""
    print_status "=== DEVELOPMENT WORKFLOW ==="
    echo "1. Edit any C# file in Core → tests rerun, service rebuilds"
    echo "2. Edit any React file in Viewer → browser hot reloads"
    echo "3. Visit Viewer to test your generators interactively"
    echo ""
    print_warning "Press Ctrl+C in each terminal to stop the services"
    echo ""
    print_info "💡 Tip: Run './start-all.sh' again to restart all services"
}

# Main execution
main() {
    echo ""
    print_status "🎮 MapGen Playground Launcher"
    echo ""
    
    check_prerequisites
    check_and_kill_existing
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