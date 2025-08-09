using System;
using System.Collections.Generic;

namespace MapGen.Core;

public class TunnelLatticeGenerator : IMapGenerator
{
    public string AlgorithmName => "tunnel-lattice";
    public string Version => "0.3.0";

    public Dictionary<string, object> DefaultParameters => new()
    {
        { "nodeCount", 24 },
        { "baseRadius", 14.0 },
        { "radiusNoiseAmp", 0.18 },
        { "extraLoops", 12 },
        { "noiseOctaves", 4 },
        { "noiseScale", 1.2 },
        { "curviness", 1.0 },
        { "curveSteps", 20 },
        { "insideBase", (byte)45 },
        { "insideRange", (byte)35 },
        { "outsideBase", (byte)185 },
        { "outsideRange", (byte)45 }
    };

    public byte[] GenerateMap(int width, int height, int seed, Dictionary<string, object> parameters)
    {
        var opt = new Options
        {
            Width = width,
            Height = height,
            Seed = seed,
            NodeCount = parameters.GetParameter("nodeCount", 24),
            BaseRadius = parameters.GetParameter("baseRadius", 14.0),
            RadiusNoiseAmp = parameters.GetParameter("radiusNoiseAmp", 0.18),
            ExtraLoops = parameters.GetParameter("extraLoops", 12),
            NoiseOctaves = parameters.GetParameter("noiseOctaves", 4),
            NoiseScale = parameters.GetParameter("noiseScale", 1.2),
            Curviness = parameters.GetParameter("curviness", 1.0),
            CurveSteps = parameters.GetParameter("curveSteps", 20),
            InsideBase = parameters.GetParameter("insideBase", (byte)45),
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
        public int NodeCount { get; set; } = 24;
        public double BaseRadius { get; set; } = 14.0;
        public double RadiusNoiseAmp { get; set; } = 0.18;
        public int ExtraLoops { get; set; } = 12;
        public int NoiseOctaves { get; set; } = 4;
        public double NoiseScale { get; set; } = 1.2;
        public double Curviness { get; set; } = 1.0;
        public int CurveSteps { get; set; } = 20;
        public byte InsideBase { get; set; } = 45;
        public byte InsideRange { get; set; } = 35;
        public byte OutsideBase { get; set; } = 185;
        public byte OutsideRange { get; set; } = 45;
    }

    public static byte[,] GenerateTileGray(Options opt)
    {
        int W = opt.Width, H = opt.Height;
        if (W <= 1 || H <= 1) throw new ArgumentException("Width/Height must be > 1");

        var rng = new Random(opt.Seed);

        var nodes = PoissonOnTorus(opt.NodeCount, W, H, rng);
        if (nodes.Length == 0) throw new InvalidOperationException("No nodes generated; lower NodeCount.");

        var edges = BuildGraph(nodes, opt.ExtraLoops, W, H);

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

    private static Pt[] PoissonOnTorus(int count, int W, int H, Random rng)
    {
        if (count <= 0) return Array.Empty<Pt>();
        var nodes = new Pt[count];
        int n = 0, attempts = 0;
        double minDist = (W + H) / (2.0 * Math.Max(4, count));
        while (n < count && attempts < count * 800)
        {
            attempts++;
            double x = rng.NextDouble() * W;
            double y = rng.NextDouble() * H;
            bool ok = true;
            for (int i = 0; i < n; i++)
            {
                if (TorusDistance(x, y, nodes[i].x, nodes[i].y, W, H) <= minDist) { ok = false; break; }
            }
            if (ok) nodes[n++] = new Pt(x, y);
        }
        if (n < count) Array.Resize(ref nodes, n);
        return nodes;
    }

    private static (int, int)[] BuildGraph(Pt[] nodes, int extraLoops, int W, int H)
    {
        int N = nodes.Length;
        if (N == 0) return Array.Empty<(int, int)>();
        var remaining = new bool[N]; for (int i = 0; i < N; i++) remaining[i] = true;
        var edgesTemp = new System.Collections.Generic.List<(int, int)>();
        int current = 0; remaining[current] = false;
        int connectedCount = 1;
        while (connectedCount < N)
        {
            double bestD = 1e18; int bestA = -1, bestB = -1;
            for (int a = 0; a < N; a++) if (!remaining[a])
            {
                for (int b = 0; b < N; b++) if (remaining[b])
                {
                    double d = TorusDistance(nodes[a].x, nodes[a].y, nodes[b].x, nodes[b].y, W, H);
                    if (d < bestD) { bestD = d; bestA = a; bestB = b; }
                }
            }
            edgesTemp.Add((bestA, bestB));
            remaining[bestB] = false; connectedCount++;
        }
        var pairs = new System.Collections.Generic.List<(int a, int b, double d)>();
        for (int i = 0; i < N; i++)
            for (int j = i + 1; j < N; j++)
                pairs.Add((i, j, TorusDistance(nodes[i].x, nodes[i].y, nodes[j].x, nodes[j].y, W, H)));
        pairs.Sort((u, v) => u.d.CompareTo(v.d));
        int added = 0;
        foreach (var p in pairs)
        {
            bool exists = false;
            foreach (var e in edgesTemp) if ((e.Item1 == p.a && e.Item2 == p.b) || (e.Item1 == p.b && e.Item2 == p.a)) { exists = true; break; }
            if (!exists) { edgesTemp.Add((p.a, p.b)); if (++added >= extraLoops) break; }
        }
        return edgesTemp.ToArray();
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

        for (int oy = -H; oy <= H; oy += H)
        {
            for (int ox = -W; ox <= W; ox += W)
            {
                double startX = ax + ox, startY = ay + oy;
                for (int y = 0; y < HH; y++)
                {
                    for (int x = 0; x < WW; x++)
                    {
                        double APx = x - startX;
                        double APy = y - startY;
                        double t = (APx * ABx + APy * ABy) / AB2;
                        if (t < 0) t = 0; else if (t > 1) t = 1;
                        double Cx = startX + ABx * t;
                        double Cy = startY + ABy * t;
                        double dx = x - Cx; double dy = y - Cy;
                        double dist = Math.Sqrt(dx * dx + dy * dy);
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