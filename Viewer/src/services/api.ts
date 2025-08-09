const API_BASE_URL = "http://localhost:5023/api";

export interface MapGenerationRequest {
  width: number;
  height: number;
  algorithm: string;
  parameters: Record<string, unknown>;
}

export interface MapGenerationResponse {
  success: boolean;
  seed: number;
  format: string;
  generatedAt: string;
  data: string;
  algorithm?: string;
  algorithmVersion?: string;
}

export interface HealthResponse {
  status: string;
  timestamp: string;
}

export interface AlgorithmInfo {
  name: string;
  version?: string;
  defaultParameters: Record<string, unknown>;
}

export interface AlgorithmsResponse {
  algorithms: AlgorithmInfo[];
}

export class ApiService {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  async healthCheck(): Promise<HealthResponse> {
    const response = await fetch(`${this.baseUrl}/map/health`);
    if (!response.ok) {
      throw new Error(`Health check failed: ${response.statusText}`);
    }
    return response.json();
  }

  async getAlgorithms(): Promise<AlgorithmsResponse> {
    const response = await fetch(`${this.baseUrl}/map/algorithms`);
    if (!response.ok) {
      throw new Error(`Failed to get algorithms: ${response.statusText}`);
    }
    return response.json();
  }

  async generateMap(
    request: MapGenerationRequest
  ): Promise<MapGenerationResponse> {
    const response = await fetch(`${this.baseUrl}/map/generate`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(
        `Map generation failed: ${errorData.error || response.statusText}`
      );
    }

    return response.json();
  }

  async generateMapWithSeed(
    request: MapGenerationRequest,
    seed: number
  ): Promise<MapGenerationResponse> {
    const response = await fetch(`${this.baseUrl}/map/generate/${seed}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(
        `Map generation failed: ${errorData.error || response.statusText}`
      );
    }

    return response.json();
  }
}

export const apiService = new ApiService();
