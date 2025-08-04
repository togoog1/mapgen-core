import { useState } from "react";
import { apiService } from "../services/api";
import type {
  MapGenerationRequest,
  MapGenerationResponse,
  HealthResponse,
} from "../services/api";

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
    <div style={{ padding: "20px", maxWidth: "800px", margin: "0 auto" }}>
      <h2>MapGen API Test</h2>

      <div style={{ marginBottom: "20px" }}>
        <h3>Health Check</h3>
        <button
          onClick={testHealthCheck}
          disabled={loading}
          style={{
            padding: "10px 20px",
            marginRight: "10px",
            backgroundColor: "#007bff",
            color: "white",
            border: "none",
            borderRadius: "4px",
            cursor: loading ? "not-allowed" : "pointer",
          }}
        >
          {loading ? "Testing..." : "Test Health Check"}
        </button>

        {healthStatus && (
          <div
            style={{
              marginTop: "10px",
              padding: "10px",
              backgroundColor: "#d4edda",
              border: "1px solid #c3e6cb",
              borderRadius: "4px",
            }}
          >
            <strong>Health Status:</strong> {healthStatus.status}
            <br />
            <strong>Timestamp:</strong> {healthStatus.timestamp}
          </div>
        )}
      </div>

      <div style={{ marginBottom: "20px" }}>
        <h3>Map Generation</h3>
        <button
          onClick={testMapGeneration}
          disabled={loading}
          style={{
            padding: "10px 20px",
            marginRight: "10px",
            backgroundColor: "#28a745",
            color: "white",
            border: "none",
            borderRadius: "4px",
            cursor: loading ? "not-allowed" : "pointer",
          }}
        >
          {loading ? "Generating..." : "Generate Map"}
        </button>

        <button
          onClick={testMapGenerationWithSeed}
          disabled={loading}
          style={{
            padding: "10px 20px",
            backgroundColor: "#ffc107",
            color: "black",
            border: "none",
            borderRadius: "4px",
            cursor: loading ? "not-allowed" : "pointer",
          }}
        >
          {loading ? "Generating..." : "Generate Map (Seed: 12345)"}
        </button>

        {mapResult && (
          <div
            style={{
              marginTop: "10px",
              padding: "10px",
              backgroundColor: "#d1ecf1",
              border: "1px solid #bee5eb",
              borderRadius: "4px",
            }}
          >
            <strong>Map Generated Successfully!</strong>
            <br />
            <strong>Seed:</strong> {mapResult.seed}
            <br />
            <strong>Format:</strong> {mapResult.format}
            <br />
            <strong>Generated At:</strong> {mapResult.generatedAt}
            <br />
            <strong>Data Size:</strong> {mapResult.data.length} characters
            (base64)
          </div>
        )}
      </div>

      {error && (
        <div
          style={{
            padding: "10px",
            backgroundColor: "#f8d7da",
            border: "1px solid #f5c6cb",
            borderRadius: "4px",
            color: "#721c24",
          }}
        >
          <strong>Error:</strong> {error}
        </div>
      )}
    </div>
  );
}
