#!/bin/bash

# Test Iterations Script for MapGen
# This script runs tests and generates comparison images for iterative improvement

echo "🧪 Running MapGen Test Iterations"
echo "=================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if we're in the right directory
if [ ! -f "Core/MapGen.Core/Class1.cs" ]; then
    print_error "Please run this script from the project root directory"
    exit 1
fi

# Create test output directory
TEST_OUTPUT_DIR="test-output-$(date +%Y%m%d-%H%M%S)"
mkdir -p "$TEST_OUTPUT_DIR"
print_status "Test output directory: $TEST_OUTPUT_DIR"

# Build the Core project
print_status "Building Core project..."
cd Core
dotnet build --configuration Release
if [ $? -ne 0 ]; then
    print_error "Failed to build Core project"
    exit 1
fi
print_success "Core project built successfully"

# Run tests and capture output
print_status "Running organic cavity tests..."
dotnet test MapGen.Core.Tests/MapGen.Core.Tests.csproj --logger "console;verbosity=detailed" --results-directory "../$TEST_OUTPUT_DIR" > "../$TEST_OUTPUT_DIR/test-output.log" 2>&1
TEST_EXIT_CODE=$?

if [ $TEST_EXIT_CODE -eq 0 ]; then
    print_success "Tests completed successfully"
else
    print_warning "Some tests failed - check the log for details"
fi

cd ..

# Copy test images if they exist
if [ -d "Core/MapGen.Core.Tests/bin/Release/net9.0/TestOutput" ]; then
    print_status "Copying test images..."
    cp -r "Core/MapGen.Core.Tests/bin/Release/net9.0/TestOutput"/* "$TEST_OUTPUT_DIR/"
    print_success "Test images copied to $TEST_OUTPUT_DIR"
fi

# Generate summary report
print_status "Generating test summary..."
{
    echo "# MapGen Test Iteration Report"
    echo "Generated: $(date)"
    echo ""
    echo "## Test Results"
    echo "Test exit code: $TEST_EXIT_CODE"
    echo ""
    echo "## Generated Images"
    if [ -d "$TEST_OUTPUT_DIR" ]; then
        find "$TEST_OUTPUT_DIR" -name "*.png" | while read -r file; do
            echo "- $(basename "$file")"
        done
    fi
    echo ""
    echo "## Test Log"
    if [ -f "$TEST_OUTPUT_DIR/test-output.log" ]; then
        cat "$TEST_OUTPUT_DIR/test-output.log"
    fi
} > "$TEST_OUTPUT_DIR/report.md"

print_success "Test summary saved to $TEST_OUTPUT_DIR/report.md"

# Start the service for manual testing
print_status "Starting MapGen service for manual testing..."
cd Service
    if [ -f "../scripts/run.sh" ]; then
      chmod +x ../scripts/run.sh
      ../scripts/run.sh &
    SERVICE_PID=$!
    print_success "Service started with PID: $SERVICE_PID"
    echo "Service PID: $SERVICE_PID" > "../$TEST_OUTPUT_DIR/service-pid.txt"
    else
      print_warning "No run.sh found in scripts directory"
fi

cd ..

# Instructions for next steps
echo ""
echo "🎯 Next Steps:"
echo "1. Check the generated images in: $TEST_OUTPUT_DIR"
echo "2. Compare with the reference image"
echo "3. Adjust parameters in AdvancedOrganicGenerator.cs"
echo "4. Run this script again to test improvements"
echo ""
echo "📊 Test Results:"
echo "- Test output: $TEST_OUTPUT_DIR/test-output.log"
echo "- Test images: $TEST_OUTPUT_DIR/"
echo "- Summary report: $TEST_OUTPUT_DIR/report.md"
echo ""
echo "🔧 To stop the service: kill \$(cat $TEST_OUTPUT_DIR/service-pid.txt)"

print_success "Test iteration completed!" 