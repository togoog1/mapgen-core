using System;
using System.Collections.Generic;
using System.Linq;

namespace MapGen.Core;

public class TunnelLatticeGenerator : IMapGenerator
{
    public string AlgorithmName => "tunnel-lattice";
    public string Version => "0.6.0";

    public Dictionary<string, object> DefaultParameters => new()
    {
        { "nodeCount", 16 },
        { "baseRadius", 10.0 },
        { "radiusNoiseAmp", 0.25 },
        { "extraLoops", 8 },
        { "noiseOctaves", 3 },
        { "noiseScale", 2.0 },
        { "curviness", 0.8 },
        { "curveSteps", 10 },
        { "jitterAmount", 0.6 },
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
            NodeCount = parameters.GetParameter("nodeCount", 25),
            BaseRadius = parameters.GetParameter("baseRadius", 12.0),
            RadiusNoiseAmp = parameters.GetParameter("radiusNoiseAmp", 0.1),
            ExtraLoops = parameters.GetParameter("extraLoops", 4),
            NoiseOctaves = parameters.GetParameter("noiseOctaves", 2),
            NoiseScale = parameters.GetParameter("noiseScale", 1.5),
            Curviness = parameters.GetParameter("curviness", 0.8),
            CurveSteps = parameters.GetParameter("curveSteps", 10),
            JitterAmount = parameters.GetParameter("jitterAmount", 0.6),
            InsideBase = parameters.GetParameter("insideBase", (byte)40),
            InsideRange = parameters.GetParameter("insideRange", (byte)35),
            OutsideBase = parameters.GetParameter("outsideBase", (byte)185),
            OutsideRange = parameters.GetParameter("outsideRange", (byte)45),
        };

        var gray = GenerateTileGray(opt);
        return ConvertGrayToRgba(gray);
    }

    private static byte[] ConvertGrayToRgba(byte[,] gray)
    {
        int h = gray.GetLength(0), w = gray.GetLength(1);
        var pixels = new byte[w * h * 4];
        int i = 0;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                byte v = gray[y, x];
                pixels[i++] = v;
                pixels[i++] = v;
                pixels[i++] = v;
                pixels[i++] = 255;
            }
        }
        return pixels;
    }

    // ==== Port of the standalone generator (adapted to class) ====
    public sealed class Options
    {
        public int Width { get; set; } = 512;
        public int Height { get; set; } = 512;
        public int Seed { get; set; } = 42;
        public int NodeCount { get; set; } = 25;
        public double BaseRadius { get; set; } = 12.0;
        public double RadiusNoiseAmp { get; set; } = 0.1;
        public int ExtraLoops { get; set; } = 4;
        public int NoiseOctaves { get; set; } = 2;
        public double NoiseScale { get; set; } = 1.5;
        public double Curviness { get; set; } = 0.8;
        public int CurveSteps { get; set; } = 10;
        public double JitterAmount { get; set; } = 0.6;
        public byte InsideBase { get; set; } = 40;
        public byte InsideRange { get; set; } = 35;
        public byte OutsideBase { get; set; } = 185;
        public byte OutsideRange { get; set; } = 45;
    }

    public static byte[,] GenerateTileGray(Options opt)
    {
        int W = opt.Width, H = opt.Height;
        if (W <= 1 || H <= 1) throw new ArgumentException("Width/Height must be > 1");

        var rng = new Random(opt.Seed);

        // Create a structured grid lattice instead of random Poisson points
        double jitterAmount = opt.JitterAmount;
        var nodes = CreateGridLattice(opt.NodeCount, W, H, rng, jitterAmount);
        if (nodes.Length == 0) throw new InvalidOperationException("No nodes generated; lower NodeCount.");

        var edges = BuildLatticeGraph(nodes, opt.ExtraLoops, W, H);

        var sdf = new float[H, W];
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                sdf[y, x] = 1e9f;

        foreach (var e in edges)
        {
            var a = nodes[e.Item1];
            var b = nodes[e.Item2];
            
            // Create curvy path instead of straight line
            var curvyPath = MakeCurvyPath(a, b, opt.CurveSteps, opt.Curviness, W, H, rng);
            UnionPolylineSdfTorus(sdf, curvyPath, opt.BaseRadius, opt.RadiusNoiseAmp, W, H, opt.NoiseOctaves, opt.NoiseScale);
        }

        var img = new byte[H, W];
        double invBase = 1.0 / Math.Max(1e-6, opt.BaseRadius);
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                double d = sdf[y, x];
                int shade;
                if (d < 0)
                {
                    shade = opt.InsideBase + (int)(opt.InsideRange * (-d * invBase));
                }
                else
                {
                    shade = opt.OutsideBase + (int)(opt.OutsideRange * Math.Min(d * invBase, 1.0));
                }
                if (shade < 0) shade = 0; if (shade > 255) shade = 255;
                img[y, x] = (byte)shade;
            }
        }

        return img;
    }

    private struct Pt { public double x, y; public Pt(double x, double y) { this.x = x; this.y = y; } }

    private static Pt[] CreateGridLattice(int count, int W, int H, Random rng, double jitterAmount = 0.6)
    {
        // Create a loosely structured lattice with organic deformation
        int gridSize = (int)Math.Ceiling(Math.Sqrt(count));
        double cellW = (double)W / gridSize;
        double cellH = (double)H / gridSize;
        
        var nodes = new List<Pt>();
        
        for (int gy = 0; gy < gridSize; gy++)
        {
            for (int gx = 0; gx < gridSize; gx++)
            {
                if (nodes.Count >= count) break;
                
                // Base grid position with offset pattern for more organic feel
                double offsetX = (gy % 2) * cellW * 0.25; // Offset every other row
                double baseX = (gx + 0.5) * cellW + offsetX;
                double baseY = (gy + 0.5) * cellH;
                
                // Add organic deformation using multiple noise scales
                double bigJitterX = (rng.NextDouble() - 0.5) * cellW * jitterAmount;
                double bigJitterY = (rng.NextDouble() - 0.5) * cellH * jitterAmount;
                
                // Add some wave-like deformation across the grid
                double waveX = Math.Sin(gy * 0.7) * cellW * 0.15;
                double waveY = Math.Cos(gx * 0.8) * cellH * 0.15;
                
                double x = baseX + bigJitterX + waveX;
                double y = baseY + bigJitterY + waveY;
                
                // Wrap to ensure torus boundary conditions
                x = ((x % W) + W) % W;
                y = ((y % H) + H) % H;
                
                nodes.Add(new Pt(x, y));
            }
            if (nodes.Count >= count) break;
        }
        
        return nodes.Take(count).ToArray();
    }

    private static (int, int)[] BuildLatticeGraph(Pt[] nodes, int extraLoops, int W, int H)
    {
        int N = nodes.Length;
        if (N == 0) return Array.Empty<(int, int)>();
        
        var edges = new List<(int, int)>();
        int gridSize = (int)Math.Ceiling(Math.Sqrt(N));
        
        // Build structured grid connections (lattice pattern)
        for (int i = 0; i < N; i++)
        {
            int gx = i % gridSize;
            int gy = i / gridSize;
            
            // Connect to right neighbor
            if (gx < gridSize - 1)
            {
                int rightIdx = i + 1;
                if (rightIdx < N) edges.Add((i, rightIdx));
            }
            
            // Connect to bottom neighbor  
            if (gy < gridSize - 1)
            {
                int bottomIdx = i + gridSize;
                if (bottomIdx < N) edges.Add((i, bottomIdx));
            }
            
            // Torus wrapping connections
            // Right edge wraps to left
            if (gx == gridSize - 1)
            {
                int wrapRightIdx = gy * gridSize; // leftmost node in same row
                if (wrapRightIdx < N && wrapRightIdx != i) edges.Add((i, wrapRightIdx));
            }
            
            // Bottom edge wraps to top
            if (gy == gridSize - 1)
            {
                int wrapBottomIdx = gx; // topmost node in same column
                if (wrapBottomIdx < N && wrapBottomIdx != i) edges.Add((i, wrapBottomIdx));
            }
        }
        
        // Add more organic connections for yarn-like intertwining
        var connectionCandidates = new List<(int a, int b, double dist)>();
        
        for (int i = 0; i < N; i++)
        {
            for (int j = i + 1; j < N; j++)
            {
                double dist = TorusDistance(nodes[i].x, nodes[i].y, nodes[j].x, nodes[j].y, W, H);
                if (dist < Math.Min(W, H) * 0.3) // Only consider nearby connections
                {
                    connectionCandidates.Add((i, j, dist));
                }
            }
        }
        
        // Sort by distance and add shortest connections that create interesting patterns
        connectionCandidates.Sort((a, b) => a.dist.CompareTo(b.dist));
        
        int added = 0;
        foreach (var candidate in connectionCandidates)
        {
            if (added >= extraLoops) break;
            
            // Check if this edge already exists
            bool exists = false;
            foreach (var existing in edges)
            {
                if ((existing.Item1 == candidate.a && existing.Item2 == candidate.b) ||
                    (existing.Item1 == candidate.b && existing.Item2 == candidate.a))
                {
                    exists = true;
                    break;
                }
            }
            
            if (!exists)
            {
                edges.Add((candidate.a, candidate.b));
                added++;
            }
        }
        
        return edges.ToArray();
    }

    private static (double dx, double dy) TorusVector(double ax, double ay, double bx, double by, int W, int H)
    {
        double dx = bx - ax; double dy = by - ay;
        if (dx > W / 2.0) dx -= W; else if (dx < -W / 2.0) dx += W;
        if (dy > H / 2.0) dy -= H; else if (dy < -H / 2.0) dy += H;
        return (dx, dy);
    }

    private static double TorusDistance(double ax, double ay, double bx, double by, int W, int H)
    {
        double dx = Math.Abs(ax - bx); if (dx > W / 2.0) dx = W - dx;
        double dy = Math.Abs(ay - by); if (dy > H / 2.0) dy = H - dy;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static double Hash22(double x, double y)
    {
        const double TWO_PI = Math.PI * 2.0;
        double n = Math.Sin((x * 127.1 + y * 311.7) * TWO_PI);
        double f = n - Math.Floor(n);
        return f;
    }

    private static double ValueNoisePeriodic(double u, double v, int period)
    {
        double x = u * period, y = v * period;
        int x0 = (int)Math.Floor(x) % period; if (x0 < 0) x0 += period;
        int y0 = (int)Math.Floor(y) % period; if (y0 < 0) y0 += period;
        int x1 = (x0 + 1) % period, y1 = (y0 + 1) % period;
        double fx = x - Math.Floor(x), fy = y - Math.Floor(y);
        double sx = fx * fx * (3 - 2 * fx);
        double sy = fy * fy * (3 - 2 * fy);
        double a = Hash22(x0, y0), b = Hash22(x1, y0), c = Hash22(x0, y1), d = Hash22(x1, y1);
        double i1 = a + (b - a) * sx;
        double i2 = c + (d - c) * sx;
        return (i1 + (i2 - i1) * sy);
    }

    private static double NoiseToroidal(double u, double v, int octaves = 3, double scale = 1.0)
    {
        const int P = 256;
        double f = 0.0, amp = 0.6, freq = scale;
        for (int o = 0; o < Math.Max(1, octaves); o++)
        {
            double uu = (u * freq) % 1.0; if (uu < 0) uu += 1.0;
            double vv = (v * freq) % 1.0; if (vv < 0) vv += 1.0;
            f += amp * ValueNoisePeriodic(uu, vv, P);
            amp *= 0.5; freq *= 2.0;
        }
        return f * 2.0 - 1.0;
    }

    private static void SegmentSdfMinTorusByVector(float[,] sdf, double ax, double ay, double dvx, double dvy,
                                                   double baseR, double amp, int W, int H, int noiseOctaves, double noiseScale)
    {
        int HH = sdf.GetLength(0), WW = sdf.GetLength(1);
        double ABx = dvx, ABy = dvy;
        double AB2 = ABx * ABx + ABy * ABy + 1e-12;

        // Calculate bounding box to limit pixel checking
        double minX = Math.Min(ax, ax + ABx) - baseR * (1.0 + amp);
        double maxX = Math.Max(ax, ax + ABx) + baseR * (1.0 + amp);
        double minY = Math.Min(ay, ay + ABy) - baseR * (1.0 + amp);
        double maxY = Math.Max(ay, ay + ABy) + baseR * (1.0 + amp);

        int startX = Math.Max(0, (int)Math.Floor(minX));
        int endX = Math.Min(WW - 1, (int)Math.Ceiling(maxX));
        int startY = Math.Max(0, (int)Math.Floor(minY));
        int endY = Math.Min(HH - 1, (int)Math.Ceiling(maxY));

        // Only process single copy first, add torus wrapping only if needed
        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                double APx = x - ax;
                double APy = y - ay;
                double t = (APx * ABx + APy * ABy) / AB2;
                if (t < 0) t = 0; else if (t > 1) t = 1;
                double Cx = ax + ABx * t;
                double Cy = ay + ABy * t;
                double dx = x - Cx; double dy = y - Cy;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                
                // Simplified noise calculation 
                double u = (Cx / W) % 1.0; if (u < 0) u += 1.0;
                double v = (Cy / H) % 1.0; if (v < 0) v += 1.0;
                double r = baseR * (1.0 + amp * NoiseToroidal(u, v, noiseOctaves, noiseScale));
                float val = (float)(dist - r);
                if (val < sdf[y, x]) sdf[y, x] = val;
            }
        }

        // Handle torus wrapping only for segments near edges
        if (minX < baseR || maxX > W - baseR || minY < baseR || maxY > H - baseR)
        {
            for (int oy = -H; oy <= H; oy += H)
            {
                for (int ox = -W; ox <= W; ox += W)
                {
                    if (ox == 0 && oy == 0) continue; // Already processed above
                    
                    double startX_wrap = ax + ox, startY_wrap = ay + oy;
                    for (int y = 0; y < HH; y++)
                    {
                        for (int x = 0; x < WW; x++)
                        {
                            double APx = x - startX_wrap;
                            double APy = y - startY_wrap;
                            double t = (APx * ABx + APy * ABy) / AB2;
                            if (t < 0) t = 0; else if (t > 1) t = 1;
                            double Cx = startX_wrap + ABx * t;
                            double Cy = startY_wrap + ABy * t;
                            double dx = x - Cx; double dy = y - Cy;
                            double dist = Math.Sqrt(dx * dx + dy * dy);
                            
                            if (dist < baseR * 2) // Early exit for distant pixels
                            {
                                double u = Mod(Cx, W) / W;
                                double v = Mod(Cy, H) / H;
                                double r = baseR * (1.0 + amp * NoiseToroidal(u, v, noiseOctaves, noiseScale));
                                float val = (float)(dist - r);
                                if (val < sdf[y, x]) sdf[y, x] = val;
                            }
                        }
                    }
                }
            }
        }
    }

    private static double Mod(double a, int m)
    {
        double r = a % m; if (r < 0) r += m; return r;
    }

    // Phase 2: Curvy polylines implementation
    private static (double x, double y) CurlField(double u, double v)
    {
        // Simple curl noise - derivative of potential field
        double scale = 4.0;
        double dx = ValueNoisePeriodic(u, v + 0.01, 256) - ValueNoisePeriodic(u, v - 0.01, 256);
        double dy = ValueNoisePeriodic(u + 0.01, v, 256) - ValueNoisePeriodic(u - 0.01, v, 256);
        return (dy * scale, -dx * scale); // Perpendicular for curl
    }

    private static System.Collections.Generic.List<(double x, double y)> MakeCurvyPath(
        Pt a, Pt b, int steps, double curviness, int W, int H, Random rng)
    {
        var path = new System.Collections.Generic.List<(double x, double y)>();
        path.Add((a.x, a.y));

        if (steps <= 1)
        {
            path.Add((b.x, b.y));
            return path;
        }

        // Get the direct vector from a to b accounting for torus wrapping
        var directVec = TorusVector(a.x, a.y, b.x, b.y, W, H);
        double totalDist = Math.Sqrt(directVec.dx * directVec.dx + directVec.dy * directVec.dy);
        double stepLen = totalDist / steps;

        double currentX = a.x, currentY = a.y;
        
        for (int i = 1; i < steps; i++)
        {
            // Progress toward target
            double t = (double)i / steps;
            double targetX = a.x + directVec.dx * t;
            double targetY = a.y + directVec.dy * t;
            
            // Add curl noise deviation
            double u = Mod(currentX, W) / W;
            double v = Mod(currentY, H) / H;
            var curl = CurlField(u, v);
            
            // Blend toward target with curl deviation
            double deviation = curviness * stepLen * 0.5;
            currentX = targetX + curl.x * deviation;
            currentY = targetY + curl.y * deviation;
            
            // Wrap coordinates
            currentX = Mod(currentX, W);
            currentY = Mod(currentY, H);
            
            path.Add((currentX, currentY));
        }

        path.Add((b.x, b.y));
        return path;
    }

    private static void UnionPolylineSdfTorus(float[,] sdf, System.Collections.Generic.List<(double x, double y)> path,
        double baseR, double amp, int W, int H, int noiseOctaves, double noiseScale)
    {
        if (path.Count < 2) return;

        // Draw capsules between consecutive points
        for (int i = 0; i < path.Count - 1; i++)
        {
            var a = path[i];
            var b = path[i + 1];
            var dv = TorusVector(a.x, a.y, b.x, b.y, W, H);
            SegmentSdfMinTorusByVector(sdf, a.x, a.y, dv.dx, dv.dy, baseR, amp, W, H, noiseOctaves, noiseScale);
        }
    }
} 