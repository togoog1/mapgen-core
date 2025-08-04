import { useState } from "react";
import { apiService } from "../services/api";
import type {
  MapGenerationRequest,
  MapGenerationResponse,
  HealthResponse,
} from "../services/api";
import { Button } from "./ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "./ui/card";
import { Badge } from "./ui/badge";
import { Activity, Map, Zap, AlertCircle, CheckCircle } from "lucide-react";
import { MapVisualizer } from "./MapVisualizer";

export function MapTest() {
  const [healthStatus, setHealthStatus] = useState<HealthResponse | null>(null);
  const [mapResult, setMapResult] = useState<MapGenerationResponse | null>(
    null
  );
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const testHealthCheck = async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await apiService.healthCheck();
      setHealthStatus(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error occurred");
    } finally {
      setLoading(false);
    }
  };

  const testMapGeneration = async () => {
    setLoading(true);
    setError(null);
    try {
      const request: MapGenerationRequest = {
        width: 100,
        height: 100,
        algorithm: "perlin",
        parameters: {
          scale: 0.1,
          octaves: 4,
        },
      };

      const result = await apiService.generateMap(request);
      setMapResult(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error occurred");
    } finally {
      setLoading(false);
    }
  };

  const testMapGenerationWithSeed = async () => {
    setLoading(true);
    setError(null);
    try {
      const request: MapGenerationRequest = {
        width: 50,
        height: 50,
        algorithm: "perlin",
        parameters: {
          scale: 0.2,
          octaves: 2,
        },
      };

      const result = await apiService.generateMapWithSeed(request, 12345);
      setMapResult(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error occurred");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto p-6 max-w-4xl">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-center mb-2">MapGen API Test</h1>
        <p className="text-muted-foreground text-center">
          Test the integration between Viewer, Service, and Core components
        </p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        {/* Health Check Card */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Activity className="h-5 w-5" />
              Health Check
            </CardTitle>
            <CardDescription>
              Verify the Service is running and accessible
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <Button
              onClick={testHealthCheck}
              disabled={loading}
              className="w-full"
              variant="outline"
            >
              {loading ? (
                <>
                  <Activity className="mr-2 h-4 w-4 animate-spin" />
                  Testing...
                </>
              ) : (
                <>
                  <Zap className="mr-2 h-4 w-4" />
                  Test Health Check
                </>
              )}
            </Button>

            {healthStatus && (
              <div className="p-4 bg-green-50 border border-green-200 rounded-lg">
                <div className="flex items-center gap-2 mb-2">
                  <CheckCircle className="h-4 w-4 text-green-600" />
                  <span className="font-medium text-green-800">
                    Service Healthy
                  </span>
                </div>
                <div className="text-sm text-green-700">
                  <div>
                    <strong>Status:</strong> {healthStatus.status}
                  </div>
                  <div>
                    <strong>Timestamp:</strong>{" "}
                    {new Date(healthStatus.timestamp).toLocaleString()}
                  </div>
                </div>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Map Generation Card */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Map className="h-5 w-5" />
              Map Generation
            </CardTitle>
            <CardDescription>
              Generate maps using the Core library via Service
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex gap-2">
              <Button
                onClick={testMapGeneration}
                disabled={loading}
                className="flex-1"
              >
                {loading ? (
                  <>
                    <Activity className="mr-2 h-4 w-4 animate-spin" />
                    Generating...
                  </>
                ) : (
                  <>
                    <Map className="mr-2 h-4 w-4" />
                    Generate Map
                  </>
                )}
              </Button>
              <Button
                onClick={testMapGenerationWithSeed}
                disabled={loading}
                variant="secondary"
                className="flex-1"
              >
                {loading ? (
                  <>
                    <Activity className="mr-2 h-4 w-4 animate-spin" />
                    Generating...
                  </>
                ) : (
                  <>
                    <Zap className="mr-2 h-4 w-4" />
                    Seed: 12345
                  </>
                )}
              </Button>
            </div>

            {mapResult && (
              <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg">
                <div className="flex items-center gap-2 mb-3">
                  <CheckCircle className="h-4 w-4 text-blue-600" />
                  <span className="font-medium text-blue-800">
                    Map Generated Successfully!
                  </span>
                </div>
                <div className="space-y-2 text-sm text-blue-700">
                  <div className="flex justify-between">
                    <span>Seed:</span>
                    <Badge variant="outline">{mapResult.seed}</Badge>
                  </div>
                  <div className="flex justify-between">
                    <span>Format:</span>
                    <Badge variant="secondary">{mapResult.format}</Badge>
                  </div>
                  <div className="flex justify-between">
                    <span>Generated:</span>
                    <span>
                      {new Date(mapResult.generatedAt).toLocaleString()}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span>Data Size:</span>
                    <Badge variant="outline">
                      {mapResult.data.length} chars
                    </Badge>
                  </div>
                </div>
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Map Visualization */}
      {mapResult && (
        <div className="mt-6">
          <MapVisualizer
            mapData={mapResult.data}
            width={mapResult.format === "png" ? 100 : 50}
            height={mapResult.format === "png" ? 100 : 50}
            seed={mapResult.seed}
            format={mapResult.format}
          />
        </div>
      )}

      {error && (
        <Card className="mt-6 border-destructive">
          <CardContent className="pt-6">
            <div className="flex items-center gap-2 text-destructive">
              <AlertCircle className="h-4 w-4" />
              <span className="font-medium">Error</span>
            </div>
            <p className="mt-2 text-sm text-destructive">{error}</p>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
