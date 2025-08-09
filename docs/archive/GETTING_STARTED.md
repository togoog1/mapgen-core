# MapGen: High-Resolution Organic Cavity Generation

## 🎯 Goal

Generate procedural world depth maps that show complex organic cavities and structures similar to natural porous materials, with high-resolution output and advanced pattern generation.

## 🚀 Quick Start

### 1. Run Test Iterations

```bash
./test-iterations.sh
```

This script will:

- Build the Core project
- Run comprehensive tests
- Generate comparison images
- Start the service for manual testing
- Create a detailed report

### 2. View Generated Images

Check the `test-output-YYYYMMDD-HHMMSS/` directory for:

- Test images with different parameters
- Performance benchmarks
- Quality analysis reports

### 3. Manual Testing via Web Interface

1. Start the service: `cd Service && ./run.sh`
2. Open the viewer: `cd Viewer && npm run dev`
3. Navigate to the Map Generator tab
4. Try the "Generate High-Res (1024x1024)" button

## 🧬 Advanced Organic Generator

The new `AdvancedOrganicGenerator` creates complex cavity patterns using:

### Multi-Layer Approach

1. **Base Cavities**: Initial cavity seeding with organic clustering
2. **Organic Growth**: Biological-inspired expansion patterns
3. **Fractal Noise**: Multi-octave detail enhancement
4. **Voronoi Structure**: Natural cell-based boundaries
5. **Surface Texture**: Granular detail addition
6. **Smoothing**: Natural edge processing

### Key Parameters

```json
{
  "cavityDensity": 0.4, // Percentage of cavity area
  "cavityDepth": 0.8, // How deep cavities appear
  "wallThickness": 0.15, // Thickness of wall structures
  "organicVariation": 0.6, // Natural variation in growth
  "branchingFactor": 0.7, // How much cavities branch
  "textureDetail": 0.5, // Surface granularity
  "smoothingPasses": 4 // Edge smoothing iterations
}
```

## 📊 Quality Metrics

The test suite evaluates:

### Visual Quality

- **Cavity Percentage**: 25-65% for natural patterns
- **Depth Variance**: >2000 for good contrast
- **Average Depth**: Balanced distribution around middle gray
- **Edge Quality**: Smooth, organic transitions

### Technical Quality

- **Resolution Support**: Up to 2048x2048
- **Performance**: <30 seconds for 1024x1024
- **Seed Consistency**: Identical results with same seed
- **Parameter Sensitivity**: Responsive to adjustments

## 🔧 Iterative Improvement

### 1. Run Tests

```bash
./test-iterations.sh
```

### 2. Analyze Results

- Check generated images in test output directory
- Review quality metrics in the report
- Compare with reference image characteristics

### 3. Adjust Parameters

Edit `Core/MapGen.Core/AdvancedOrganicGenerator.cs`:

- Modify default parameters
- Adjust algorithm behavior
- Fine-tune quality settings

### 4. Test Again

```bash
./test-iterations.sh
```

## 🎨 Reference Image Characteristics

The target pattern should exhibit:

- **Organic Cavities**: Irregular, amoeba-like dark shapes
- **Sinuous Walls**: Light, undulating structures
- **Depth Gradients**: Smooth transitions from deep to shallow
- **Granular Texture**: Fine surface detail
- **Natural Complexity**: Non-geometric, biological appearance

## 📈 Performance Targets

| Resolution | Target Time | Memory Usage |
| ---------- | ----------- | ------------ |
| 512x512    | <5 seconds  | <50MB        |
| 1024x1024  | <30 seconds | <200MB       |
| 2048x2048  | <2 minutes  | <800MB       |

## 🔬 Testing Framework

### Automated Tests

- **Parameter Sweep**: Test different parameter combinations
- **Seed Consistency**: Verify deterministic generation
- **Performance Benchmarking**: Measure generation speed
- **Image Analysis**: Calculate quality metrics
- **Resolution Scaling**: Test different sizes

### Manual Testing

- **Visual Comparison**: Side-by-side with reference
- **Parameter Tuning**: Real-time adjustment
- **Quality Assessment**: Subjective evaluation

## 🛠️ Development Workflow

1. **Implement Changes**: Modify algorithms in Core project
2. **Run Tests**: Execute test suite for validation
3. **Analyze Results**: Review generated images and metrics
4. **Iterate**: Adjust parameters and algorithms
5. **Validate**: Ensure improvements meet targets

## 📁 Project Structure

```
mapgen-core/
├── Core/                          # Core algorithms
│   ├── MapGen.Core/
│   │   ├── Class1.cs             # Basic generators
│   │   └── AdvancedOrganicGenerator.cs  # New advanced generator
│   └── MapGen.Core.Tests/
│       ├── OrganicCavityTests.cs # Comprehensive tests
│       └── ImageComparisonTests.cs # Quality analysis
├── Service/                       # API service
│   └── MapGen.Service/
│       └── Controllers/
│           └── MapController.cs   # High-res endpoints
├── Viewer/                        # Web interface
│   └── src/
│       └── components/
│           └── MapTest.tsx        # Enhanced UI
└── test-iterations.sh            # Test automation script
```

## 🎯 Success Criteria

### Visual Match

- [ ] Cavities match reference organic shapes
- [ ] Wall structures are sinuous and natural
- [ ] Gradients are smooth and realistic
- [ ] Texture detail matches reference granularity

### Technical Requirements

- [ ] Generate 2048x2048 resolution maps
- [ ] Maintain <30 second generation time
- [ ] Support real-time parameter adjustment
- [ ] Export multiple format types

### Quality Standards

- [ ] No geometric artifacts
- [ ] Natural edge transitions
- [ ] Consistent depth mapping
- [ ] Organic complexity patterns

## 🚀 Next Steps

1. **Run Initial Tests**: `./test-iterations.sh`
2. **Review Generated Images**: Check test output directory
3. **Compare with Reference**: Evaluate visual similarity
4. **Adjust Parameters**: Fine-tune algorithm settings
5. **Iterate**: Repeat until quality targets are met

The goal is to create procedural cavity maps that can be used for:

- **3D modeling and displacement**
- **Game world generation**
- **Scientific visualization**
- **Artistic texture creation**
