import { useState, useEffect } from "react";
import { apiService } from "../services/api";
import type {
  MapGenerationRequest,
  MapGenerationResponse,
  HealthResponse,
  AlgorithmInfo,
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

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "./ui/dialog";
import {
  Activity,
  Map,
  Zap,
  AlertCircle,
  CheckCircle,
  Settings,
  Eye,
  Download,
  Share2,
  Info,
} from "lucide-react";
import { MapVisualizer } from "./MapVisualizer";

export function MapTest() {
  const [healthStatus, setHealthStatus] = useState<HealthResponse | null>(null);
  const [mapResult, setMapResult] = useState<MapGenerationResponse | null>(
    null
  );
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [algorithms, setAlgorithms] = useState<AlgorithmInfo[]>([]);
  const [selectedAlgorithm, setSelectedAlgorithm] = useState<string>("perlin");
  const [algorithmParameters, setAlgorithmParameters] = useState<
    Record<string, unknown>
  >({});

  // Load available algorithms on component mount
  useEffect(() => {
    const loadAlgorithms = async () => {
      try {
        const response = await apiService.getAlgorithms();
        setAlgorithms(response.algorithms);
        if (response.algorithms.length > 0) {
          setSelectedAlgorithm(response.algorithms[0].name);
          setAlgorithmParameters(response.algorithms[0].defaultParameters);
        }
      } catch (err) {
        console.error("Failed to load algorithms:", err);
      }
    };

    loadAlgorithms();
  }, []);

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
        width: 512,
        height: 512,
        algorithm: selectedAlgorithm,
        parameters: algorithmParameters,
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
        width: 512,
        height: 512,
        algorithm: selectedAlgorithm,
        parameters: algorithmParameters,
      };

      const result = await apiService.generateMapWithSeed(request, 12345);
      setMapResult(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error occurred");
    } finally {
      setLoading(false);
    }
  };

  const testHighResolutionGeneration = async () => {
    setLoading(true);
    setError(null);
    try {
      const request: MapGenerationRequest = {
        width: 1024,
        height: 1024,
        algorithm: selectedAlgorithm,
        parameters: algorithmParameters,
      };

      const result = await apiService.generateMap(request);
      setMapResult(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unknown error occurred");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-blue-50">
      {/* Header */}
      <div className="bg-white/80 backdrop-blur-sm border-b sticky top-0 z-10">
        <div className="container mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
                MapGen Viewer
              </h1>
              <p className="text-sm text-muted-foreground">
                Interactive map generation and visualization
              </p>
            </div>
            <div className="flex items-center gap-2">
              {loading ? (
                <Badge variant="outline" className="text-xs">
                  Checking…
                </Badge>
              ) : healthStatus?.status === "healthy" ? (
                <Badge
                  variant="outline"
                  className="text-xs bg-green-100 text-green-700 border-green-200"
                >
                  Healthy
                </Badge>
              ) : (
                <Button variant="outline" size="sm" onClick={testHealthCheck}>
                  <Activity className="h-4 w-4 mr-2" />
                  Check Service
                </Button>
              )}
              <Button variant="outline" size="sm">
                <Settings className="h-4 w-4 mr-2" />
                Settings
              </Button>
              <Button variant="outline" size="sm">
                <Info className="h-4 w-4 mr-2" />
                Docs
              </Button>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="container mx-auto px-6 py-8 space-y-6">
        <div className="grid gap-6 lg:grid-cols-3">
          {/* Controls (left sidebar) */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Settings className="h-5 w-5" />
                Generation Controls
              </CardTitle>
              <CardDescription>
                Configure and generate maps with custom parameters
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-3">
                <div>
                  <label className="text-sm font-medium">Algorithm</label>
                  <select
                    value={selectedAlgorithm}
                    onChange={(e) => {
                      const algorithm = algorithms.find(
                        (a) => a.name === e.target.value
                      );
                      setSelectedAlgorithm(e.target.value);
                      if (algorithm) {
                        setAlgorithmParameters(algorithm.defaultParameters);
                      }
                    }}
                    className="w-full mt-1 p-2 border border-gray-300 rounded-md bg-white"
                  >
                    {algorithms.map((algorithm) => (
                      <option key={algorithm.name} value={algorithm.name}>
                        {algorithm.name.charAt(0).toUpperCase() +
                          algorithm.name.slice(1)}
                      </option>
                    ))}
                  </select>
                  {(() => {
                    const info = algorithms.find(
                      (a) => a.name === selectedAlgorithm
                    );
                    return info?.version ? (
                      <div className="text-xs text-gray-500 mt-1">
                        Version: {info.version}
                      </div>
                    ) : null;
                  })()}
                </div>
              </div>

              <div className="space-y-3">
                <Button
                  onClick={testMapGeneration}
                  disabled={loading}
                  className="w-full"
                  size="lg"
                >
                  {loading ? (
                    <>
                      <Activity className="mr-2 h-5 w-5 animate-spin" />
                      Generating...
                    </>
                  ) : (
                    <>
                      <Map className="mr-2 h-5 w-5" />
                      Generate Random Map
                    </>
                  )}
                </Button>
                <Button
                  onClick={testMapGenerationWithSeed}
                  disabled={loading}
                  variant="secondary"
                  className="w-full"
                  size="lg"
                >
                  {loading ? (
                    <>
                      <Activity className="mr-2 h-5 w-5 animate-spin" />
                      Generating...
                    </>
                  ) : (
                    <>
                      <Zap className="mr-2 h-5 w-5" />
                      Generate with Seed (12345)
                    </>
                  )}
                </Button>
                <Button
                  onClick={testHighResolutionGeneration}
                  disabled={loading}
                  variant="outline"
                  className="w-full"
                  size="lg"
                >
                  {loading ? (
                    <>
                      <Activity className="mr-2 h-5 w-5 animate-spin" />
                      Generating...
                    </>
                  ) : (
                    <>
                      <Eye className="mr-2 h-5 w-5" />
                      Generate High-Res (1024x1024)
                    </>
                  )}
                </Button>

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
                      {(mapResult.algorithm || mapResult.algorithmVersion) && (
                        <div className="flex justify-between">
                          <span>Generator:</span>
                          <span>
                            {mapResult.algorithm ?? selectedAlgorithm}
                            {mapResult.algorithmVersion
                              ? ` (v${mapResult.algorithmVersion})`
                              : ""}
                          </span>
                        </div>
                      )}
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
                    <div className="flex gap-2 mt-4">
                      <Button size="sm" variant="outline">
                        <Download className="h-4 w-4 mr-2" />
                        Download
                      </Button>
                      <Button size="sm" variant="outline">
                        <Share2 className="h-4 w-4 mr-2" />
                        Share
                      </Button>
                    </div>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Map Visualization (right, spans two columns) */}
          <Card className="lg:col-span-2">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Eye className="h-5 w-5" />
                Map Visualization
                {(() => {
                  const info = algorithms.find(
                    (a) => a.name === selectedAlgorithm
                  );
                  return info?.version ? (
                    <span className="ml-2 text-xs font-normal text-muted-foreground">
                      v{info.version}
                    </span>
                  ) : null;
                })()}
              </CardTitle>
              <CardDescription>
                Visual representation of the generated map
              </CardDescription>
            </CardHeader>
            <CardContent>
              {mapResult ? (
                <MapVisualizer
                  mapData={mapResult.data}
                  width={mapResult.format === "png" ? 100 : 50}
                  height={mapResult.format === "png" ? 100 : 50}
                  seed={mapResult.seed}
                  format={mapResult.format}
                />
              ) : (
                <div className="text-center py-12 text-muted-foreground border-2 border-dashed rounded-lg">
                  <Eye className="h-16 w-16 mx-auto mb-4 opacity-50" />
                  <h3 className="text-lg font-semibold mb-2">
                    No Map to Visualize
                  </h3>
                  <p className="text-sm">
                    Generate a map first to see the visualization
                  </p>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Error Display */}
      {error && (
        <Dialog open={!!error} onOpenChange={() => setError(null)}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2 text-destructive">
                <AlertCircle className="h-5 w-5" />
                Error
              </DialogTitle>
              <DialogDescription>{error}</DialogDescription>
            </DialogHeader>
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
}
