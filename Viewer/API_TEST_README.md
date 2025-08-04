# MapGen Viewer API Test

This document explains how to test the integration between the Viewer and the Service.

## Prerequisites

1. Make sure the Service is running on `http://localhost:5042`
2. Make sure the Viewer is running on `http://localhost:5173` (default Vite port)

## How to Test

### 1. Start the Service

Navigate to the Service directory and run:

```bash
cd Service
dotnet run --project MapGen.Service
```

The service should start on `http://localhost:5042`

### 2. Start the Viewer

Navigate to the Viewer directory and run:

```bash
cd Viewer
npm run dev
```

The viewer should start on `http://localhost:5173`

### 3. Test the Integration

1. Open your browser and go to `http://localhost:5173`
2. Scroll down to the "MapGen API Test" section
3. Click "Test Health Check" to verify the service is running
4. Click "Generate Map" to test map generation
5. Click "Generate Map (Seed: 12345)" to test map generation with a specific seed

## What the Test Does

The test component includes:

- **Health Check**: Tests the `/api/map/health` endpoint to verify the service is running
- **Map Generation**: Tests the `/api/map/generate` endpoint with a 100x100 Perlin noise map
- **Map Generation with Seed**: Tests the `/api/map/generate/{seed}` endpoint with a 50x50 map using seed 12345

## Expected Results

- **Health Check**: Should return status "healthy" with a timestamp
- **Map Generation**: Should return a successful response with map data in base64 format
- **Error Handling**: If the service is not running, you'll see error messages

## Troubleshooting

1. **CORS Errors**: Make sure the Service has CORS enabled (already configured in Program.cs)
2. **Connection Refused**: Make sure the Service is running on port 5042
3. **404 Errors**: Make sure the API endpoints are properly configured in the Service

## API Endpoints

- `GET /api/map/health` - Health check endpoint
- `POST /api/map/generate` - Generate map with random seed
- `POST /api/map/generate/{seed}` - Generate map with specific seed

## Next Steps

Once the basic integration is working, you can:

1. Add more sophisticated map generation parameters
2. Implement map visualization using the returned data
3. Add more test scenarios for different algorithms
4. Implement error handling and retry logic
