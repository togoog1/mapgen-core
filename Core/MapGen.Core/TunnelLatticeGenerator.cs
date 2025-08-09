using System;
using System.Collections.Generic;
using System.Linq;

namespace MapGen.Core;

public class TunnelLatticeGenerator : IMapGenerator
{
    public string AlgorithmName => "tunnel-lattice";
    public string Version => "0.8.0";

    public Dictionary<string, object> DefaultParameters => new()
    {
        { "nodeCount", 16 },           // Grid density (4x4 = 16 nodes)
        { "baseRadius", 12.0 },        // Base tunnel width
        { "radiusNoiseAmp", 0.2 },     // Width variation
        { "extraLoops", 6 },           // Additional random connections
        { "curviness", 0.6 },          // How curved the tunnels are
        { "curveSteps", 8 },           // Smoothness of curves
        { "jitterAmount", 0.4 },       // Node position randomness
        { "noiseOctaves", 3 },         // Noise detail
        { "noiseScale", 1.5 },         // Noise frequency
        { "insideBase", (byte)40 },
        { "insideRange", (byte)30 },
        { "outsideBase", (byte)190 },
        { "outsideRange", (byte)40 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var opt = new Options
        {
            Width = width,
            Height = height,
            Seed = seed,
            NodeCount = parameters.GetParameter("nodeCount", 16),
            BaseRadius = parameters.GetParameter("baseRadius", 12.0),
            RadiusNoiseAmp = parameters.GetParameter("radiusNoiseAmp", 0.2),
            ExtraLoops = parameters.GetParameter("extraLoops", 6),
            Curviness = parameters.GetParameter("curviness", 0.6),
            CurveSteps = parameters.GetParameter("curveSteps", 8),
            JitterAmount = parameters.GetParameter("jitterAmount", 0.4),
            NoiseOctaves = parameters.GetParameter("noiseOctaves", 3),
            NoiseScale = parameters.GetParameter("noiseScale", 1.5),
            InsideBase = parameters.GetParameter("insideBase", (byte)40),
            InsideRange = parameters.GetParameter("insideRange", (byte)30),
            OutsideBase = parameters.GetParameter("outsideBase", (byte)190),
            OutsideRange = parameters.GetParameter("outsideRange", (byte)40),
        };

        var grayMap = GenerateTileGray(opt);
        return ConvertGrayToRgba(grayMap);
    }

    private class Options
    {
        public int Width;
        public int Height;
        public int Seed;
        public int NodeCount;
        public double BaseRadius;
        public double RadiusNoiseAmp;
        public int ExtraLoops;
        public double Curviness;
        public int CurveSteps;
        public double JitterAmount;
        public int NoiseOctaves;
        public double NoiseScale;
        public byte InsideBase;
        public byte InsideRange;
        public byte OutsideBase;
        public byte OutsideRange;
    }

    private byte[] GenerateTileGray(Options opt)
    {
        var random = new Random(opt.Seed);
        var gray = new byte[opt.Width * opt.Height];
        
        // Initialize with outside values
        for (int i = 0; i < gray.Length; i++)
        {
            gray[i] = (byte)(opt.OutsideBase + random.Next(opt.OutsideRange + 1));
        }

        // Create lattice grid with jitter
        var nodes = CreateLatticeGrid(opt, random);
        
        // Build lattice graph with smooth connections
        var edges = BuildLatticeGraph(nodes, opt, random);

        // Render curved tunnels
        foreach (var (start, end) in edges)
        {
            RenderCurvedTunnel(gray, opt, start, end, random);
        }

        return gray;
    }

    private List<(double x, double y)> CreateLatticeGrid(Options opt, Random random)
    {
        var nodes = new List<(double x, double y)>();
        var gridSize = (int)Math.Sqrt(opt.NodeCount);
        
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                // Base grid position
                double baseX = (col + 0.5) * opt.Width / gridSize;
                double baseY = (row + 0.5) * opt.Height / gridSize;
                
                // Add organic offset with row staggering
                double offsetX = row % 2 == 1 ? opt.Width / gridSize * 0.5 : 0;
                double jitterX = (random.NextDouble() - 0.5) * opt.JitterAmount * opt.Width / gridSize;
                double jitterY = (random.NextDouble() - 0.5) * opt.JitterAmount * opt.Height / gridSize;
                
                // Apply wave deformation for more organic feel
                double waveY = Math.Sin(col * 0.8) * opt.Width / gridSize * 0.2;
                
                double x = (baseX + offsetX + jitterX) % opt.Width;
                double y = (baseY + jitterY + waveY) % opt.Height;
                
                if (x < 0) x += opt.Width;
                if (y < 0) y += opt.Height;
                
                nodes.Add((x, y));
            }
        }
        
        return nodes;
    }

    private List<((double x, double y), (double x, double y))> BuildLatticeGraph(
        List<(double x, double y)> nodes, Options opt, Random random)
    {
        var edges = new List<((double x, double y), (double x, double y))>();
        var gridSize = (int)Math.Sqrt(opt.NodeCount);

        // Primary lattice connections (grid-based)
        for (int i = 0; i < nodes.Count; i++)
        {
            int row = i / gridSize;
            int col = i % gridSize;
            
            // Right neighbor
            if (col < gridSize - 1)
            {
                edges.Add((nodes[i], nodes[i + 1]));
            }
            else
            {
                // Wrap to left edge
                edges.Add((nodes[i], nodes[row * gridSize]));
            }
            
            // Down neighbor
            if (row < gridSize - 1)
            {
                edges.Add((nodes[i], nodes[i + gridSize]));
            }
            else
            {
                // Wrap to top edge
                edges.Add((nodes[i], nodes[col]));
            }
        }

        // Add extra random connections for organic variation
        for (int i = 0; i < opt.ExtraLoops; i++)
        {
            var node1 = nodes[random.Next(nodes.Count)];
            
            // Find nearby nodes for more natural connections
            var nearbyNodes = nodes.Where(n => 
                Distance(node1, n) < opt.Width / gridSize * 2.5 && 
                !nodes.Where(existing => existing == n).Any()
            ).ToList();
            
            if (nearbyNodes.Count > 0)
            {
                var node2 = nearbyNodes[random.Next(nearbyNodes.Count)];
                edges.Add((node1, node2));
            }
        }

        return edges;
    }

    private void RenderCurvedTunnel(byte[] gray, Options opt, 
        (double x, double y) start, (double x, double y) end, Random random)
    {
        var path = CreateCurvedPath(start, end, opt, random);
        
        foreach (var (x, y) in path)
        {
            // Render tunnel with noise-based width variation
            double noiseValue = NoiseToroidal(x / opt.Width, y / opt.Height, opt.NoiseOctaves, opt.NoiseScale);
            double radiusMultiplier = 1.0 + opt.RadiusNoiseAmp * noiseValue;
            double radius = opt.BaseRadius * radiusMultiplier;
            
            RenderTunnelSegment(gray, opt, x, y, radius, random);
        }
    }

    private List<(double x, double y)> CreateCurvedPath(
        (double x, double y) start, (double x, double y) end, Options opt, Random random)
    {
        var path = new List<(double x, double y)>();
        
        // Calculate path with torus wrapping
        var (dx, dy) = GetTorusVector(start, end, opt.Width, opt.Height);
        var distance = Math.Sqrt(dx * dx + dy * dy);
        
        if (distance < 1.0) return path;
        
        // Generate control points for smooth curve
        double midX = start.x + dx * 0.5;
        double midY = start.y + dy * 0.5;
        
        // Add curvature perpendicular to the main direction
        double perpX = -dy / distance;
        double perpY = dx / distance;
        double curveOffset = opt.Curviness * distance * 0.3 * (random.NextDouble() - 0.5);
        
        midX += perpX * curveOffset;
        midY += perpY * curveOffset;
        
        // Ensure midpoint wraps correctly on torus
        midX = ((midX % opt.Width) + opt.Width) % opt.Width;
        midY = ((midY % opt.Height) + opt.Height) % opt.Height;
        
        // Generate smooth curve
        for (int i = 0; i <= opt.CurveSteps; i++)
        {
            double t = (double)i / opt.CurveSteps;
            
            // Quadratic Bezier interpolation
            double x = (1-t)*(1-t)*start.x + 2*(1-t)*t*midX + t*t*end.x;
            double y = (1-t)*(1-t)*start.y + 2*(1-t)*t*midY + t*t*end.y;
            
            // Wrap coordinates
            x = ((x % opt.Width) + opt.Width) % opt.Width;
            y = ((y % opt.Height) + opt.Height) % opt.Height;
            
            path.Add((x, y));
        }
        
        return path;
    }

    private void RenderTunnelSegment(byte[] gray, Options opt, double centerX, double centerY, 
        double radius, Random random)
    {
        int minX = (int)Math.Max(0, centerX - radius - 2);
        int maxX = (int)Math.Min(opt.Width - 1, centerX + radius + 2);
        int minY = (int)Math.Max(0, centerY - radius - 2);
        int maxY = (int)Math.Min(opt.Height - 1, centerY + radius + 2);

        for (int py = minY; py <= maxY; py++)
        {
            for (int px = minX; px <= maxX; px++)
            {
                double dist = Distance((px, py), (centerX, centerY));
                
                if (dist <= radius)
                {
                    int idx = py * opt.Width + px;
                    gray[idx] = (byte)(opt.InsideBase + random.Next(opt.InsideRange + 1));
                }
            }
        }
    }

    private (double dx, double dy) GetTorusVector((double x, double y) from, (double x, double y) to, 
        int width, int height)
    {
        double dx = to.x - from.x;
        double dy = to.y - from.y;
        
        // Handle torus wrapping
        if (Math.Abs(dx) > width * 0.5)
        {
            dx = dx > 0 ? dx - width : dx + width;
        }
        if (Math.Abs(dy) > height * 0.5)
        {
            dy = dy > 0 ? dy - height : dy + height;
        }
        
        return (dx, dy);
    }

    private double Distance((double x, double y) a, (double x, double y) b)
    {
        double dx = a.x - b.x;
        double dy = a.y - b.y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private double NoiseToroidal(double x, double y, int octaves, double scale)
    {
        double total = 0.0;
        double frequency = scale;
        double amplitude = 1.0;
        double maxValue = 0.0;

        for (int i = 0; i < octaves; i++)
        {
            total += ValueNoisePeriodic(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= 0.5;
            frequency *= 2.0;
        }

        return total / maxValue;
    }

    private double ValueNoisePeriodic(double x, double y)
    {
        // Convert to unit torus coordinates
        double fx = x - Math.Floor(x);
        double fy = y - Math.Floor(y);
        
        // Get integer coordinates
        int ix = (int)Math.Floor(x) & 255;
        int iy = (int)Math.Floor(y) & 255;
        
        // Sample noise at corners
        double a = Hash22(ix, iy);
        double b = Hash22(ix + 1, iy);
        double c = Hash22(ix, iy + 1);
        double d = Hash22(ix + 1, iy + 1);
        
        // Smooth interpolation
        double u = Fade(fx);
        double v = Fade(fy);
        
        return Lerp(Lerp(a, b, u), Lerp(c, d, u), v);
    }

    private double Hash22(int x, int y)
    {
        int n = x * 374761393 + y * 668265263;
        n = (n << 13) ^ n;
        return ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / (double)0x7fffffff;
    }

    private double Fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private double Lerp(double a, double b, double t)
    {
        return a + t * (b - a);
    }

    private byte[] ConvertGrayToRgba(byte[] grayMap)
    {
        var rgba = new byte[grayMap.Length * 4];
        for (int i = 0; i < grayMap.Length; i++)
        {
            rgba[i * 4] = grayMap[i];     // R
            rgba[i * 4 + 1] = grayMap[i]; // G
            rgba[i * 4 + 2] = grayMap[i]; // B
            rgba[i * 4 + 3] = 255;        // A
        }
        return rgba;
    }
} 