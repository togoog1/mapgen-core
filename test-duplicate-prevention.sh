#!/bin/bash

# Test script to demonstrate duplicate prevention
echo "🧪 Testing MapGen duplicate prevention..."

echo ""
echo "1. Starting services for the first time..."
./start-all.sh

echo ""
echo "2. Waiting 10 seconds..."
sleep 10

echo ""
echo "3. Starting services again (should kill existing and restart)..."
./start-all.sh

echo ""
echo "✅ Test complete! Check that services were restarted cleanly." 