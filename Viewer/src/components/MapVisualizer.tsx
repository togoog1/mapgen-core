import { useMemo } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "./ui/card";
import { Badge } from "./ui/badge";
import { Image } from "lucide-react";

interface MapVisualizerProps {
  mapData: string | null;
  width: number;
  height: number;
  seed: number;
  format: string;
}

export function MapVisualizer({
  mapData,
  width,
  height,
  seed,
  format,
}: MapVisualizerProps) {
  const canvasRef = useMemo(() => {
    if (!mapData) return null;

    // Convert base64 to Uint8Array
    const binaryString = atob(mapData);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }

    return bytes;
  }, [mapData]);

  const renderMap = () => {
    if (!canvasRef || !mapData) return null;

    // Create a canvas element to render the map
    const canvas = document.createElement("canvas");
    canvas.width = width;
    canvas.height = height;
    const ctx = canvas.getContext("2d");

    if (!ctx) return null;

    // For now, we'll create a simple visualization from the raw data
    // In a real implementation, you'd decode the actual image format
    const imageData = ctx.createImageData(width, height);

    // Convert the raw bytes to a grayscale visualization
    for (
      let i = 0;
      i < Math.min(canvasRef.length, width * height * 4);
      i += 4
    ) {
      const value = canvasRef[i] || 0;
      imageData.data[i] = value; // R
      imageData.data[i + 1] = value; // G
      imageData.data[i + 2] = value; // B
      imageData.data[i + 3] = 255; // A
    }

    ctx.putImageData(imageData, 0, 0);
    return canvas.toDataURL();
  };

  if (!mapData) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Image className="h-5 w-5" />
            Map Visualization
          </CardTitle>
          <CardDescription>No map data available</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center h-32 bg-muted rounded-lg">
            <p className="text-muted-foreground">
              Generate a map to see visualization
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  const dataUrl = renderMap();

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Image className="h-5 w-5" />
          Map Visualization
        </CardTitle>
        <CardDescription>Generated map from Core library</CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex gap-2 flex-wrap">
          <Badge variant="outline">
            Size: {width}×{height}
          </Badge>
          <Badge variant="secondary">Seed: {seed}</Badge>
          <Badge variant="outline">Format: {format}</Badge>
        </div>

        {dataUrl && (
          <div className="border rounded-lg overflow-hidden">
            <img
              src={dataUrl}
              alt={`Generated map (${width}×${height})`}
              className="w-full h-auto max-h-96 object-contain"
            />
          </div>
        )}

        <div className="text-xs text-muted-foreground">
          <p>Raw data size: {mapData.length} characters (base64)</p>
          <p>
            This is a placeholder visualization. In a real implementation, you
            would decode the actual image format.
          </p>
        </div>
      </CardContent>
    </Card>
  );
}
