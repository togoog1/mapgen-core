# MapGen Test Iteration Report
Generated: Mon Aug  4 00:08:48 CDT 2025

## Test Results
Test exit code: 1

## Generated Images

## Test Log
  Determining projects to restore...
  All projects are up-to-date for restore.
  MapGen.Core -> /Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core/bin/Debug/net9.0/MapGen.Core.dll
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/ImageComparisonTests.cs(258,21): warning CA1416: This call site is reachable on all platforms. 'Bitmap.SetPixel(int, int, Color)' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/ImageComparisonTests.cs(265,13): warning CA1416: This call site is reachable on all platforms. 'Image.Save(string, ImageFormat)' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/ImageComparisonTests.cs(249,32): warning CA1416: This call site is reachable on all platforms. 'Bitmap' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/ImageComparisonTests.cs(265,35): warning CA1416: This call site is reachable on all platforms. 'ImageFormat.Png' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/OrganicCavityTests.cs(212,13): warning CA1416: This call site is reachable on all platforms. 'Image.Save(string, ImageFormat)' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/OrganicCavityTests.cs(196,32): warning CA1416: This call site is reachable on all platforms. 'Bitmap' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/OrganicCavityTests.cs(212,35): warning CA1416: This call site is reachable on all platforms. 'ImageFormat.Png' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/OrganicCavityTests.cs(205,21): warning CA1416: This call site is reachable on all platforms. 'Bitmap.SetPixel(int, int, Color)' is only supported on: 'windows' 6.1 and later. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) [/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/MapGen.Core.Tests.csproj]
  MapGen.Core.Tests -> /Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/bin/Debug/net9.0/MapGen.Core.Tests.dll
Test run for /Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/bin/Debug/net9.0/MapGen.Core.Tests.dll (.NETCoreApp,Version=v9.0)
VSTest version 17.14.1 (arm64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
/Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/bin/Debug/net9.0/MapGen.Core.Tests.dll
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.8.2+699d445a1a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.04]   Discovering: MapGen.Core.Tests
[xUnit.net 00:00:00.05]   Discovered:  MapGen.Core.Tests
[xUnit.net 00:00:00.05]   Starting:    MapGen.Core.Tests
  Passed MapGen.Core.Tests.UnitTest1.Test1 [1 ms]
Warning: Could not save test image comparison_resolution_256.png: The type initializer for 'Gdip' threw an exception.
Resolution 256x256:
  Generation time: 1430ms
  Cavity Percentage: 100.000%
  Average Depth: 71.0
  Depth Variance: 0.0
[xUnit.net 00:00:01.54]     MapGen.Core.Tests.ImageComparisonTests.TestImageComparison_ResolutionScaling [FAIL]
[xUnit.net 00:00:01.54]       Generation time 1430ms should be reasonable for 256x256
[xUnit.net 00:00:01.54]       Stack Trace:
[xUnit.net 00:00:01.54]         /Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/ImageComparisonTests.cs(177,0): at MapGen.Core.Tests.ImageComparisonTests.TestImageComparison_ResolutionScaling()
[xUnit.net 00:00:01.54]            at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
[xUnit.net 00:00:01.54]            at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
  Failed MapGen.Core.Tests.ImageComparisonTests.TestImageComparison_ResolutionScaling [1 s]
  Error Message:
   Generation time 1430ms should be reasonable for 256x256
  Stack Trace:
     at MapGen.Core.Tests.ImageComparisonTests.TestImageComparison_ResolutionScaling() in /Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/ImageComparisonTests.cs:line 177
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
Density: 0.3, Depth: 0.7, Variation: 0.5, Score: 0.397
Density: 0.3, Depth: 0.7, Variation: 0.6, Score: 0.397
Density: 0.3, Depth: 0.7, Variation: 0.7, Score: 0.397
Density: 0.3, Depth: 0.7, Variation: 0.8, Score: 0.397
[xUnit.net 00:00:08.50]     MapGen.Core.Tests.OrganicCavityTests.TestAdvancedOrganicGenerator_ImageAnalysis [FAIL]
[xUnit.net 00:00:08.50]       Cavity percentage 100.000% should be between 10% and 90%
[xUnit.net 00:00:08.50]       Stack Trace:
[xUnit.net 00:00:08.50]         /Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/OrganicCavityTests.cs(174,0): at MapGen.Core.Tests.OrganicCavityTests.TestAdvancedOrganicGenerator_ImageAnalysis()
[xUnit.net 00:00:08.50]            at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
[xUnit.net 00:00:08.50]            at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
  Failed MapGen.Core.Tests.OrganicCavityTests.TestAdvancedOrganicGenerator_ImageAnalysis [8 s]
  Error Message:
   Cavity percentage 100.000% should be between 10% and 90%
  Stack Trace:
     at MapGen.Core.Tests.OrganicCavityTests.TestAdvancedOrganicGenerator_ImageAnalysis() in /Users/nathanallen/Documents/GitHub/Playground/mapgen-core/Core/MapGen.Core.Tests/OrganicCavityTests.cs:line 174
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
Density: 0.3, Depth: 0.8, Variation: 0.5, Score: 0.336
Density: 0.3, Depth: 0.8, Variation: 0.6, Score: 0.336
Warning: Could not save test image test_advanced_organic_seed_consistency.png: The type initializer for 'Gdip' threw an exception.
  Passed MapGen.Core.Tests.OrganicCavityTests.TestAdvancedOrganicGenerator_SeedConsistency [3 s]
Density: 0.3, Depth: 0.8, Variation: 0.7, Score: 0.336
Density: 0.3, Depth: 0.8, Variation: 0.8, Score: 0.336
Density: 0.3, Depth: 0.9, Variation: 0.5, Score: 0.276
Density: 0.3, Depth: 0.9, Variation: 0.6, Score: 0.276
Density: 0.3, Depth: 0.9, Variation: 0.7, Score: 0.276
Density: 0.3, Depth: 0.9, Variation: 0.8, Score: 0.276
Density: 0.4, Depth: 0.7, Variation: 0.5, Score: 0.397
Density: 0.4, Depth: 0.7, Variation: 0.6, Score: 0.397
Density: 0.4, Depth: 0.7, Variation: 0.7, Score: 0.397
Density: 0.4, Depth: 0.7, Variation: 0.8, Score: 0.397
Density: 0.4, Depth: 0.8, Variation: 0.5, Score: 0.336
Density: 0.4, Depth: 0.8, Variation: 0.6, Score: 0.336
Density: 0.4, Depth: 0.8, Variation: 0.7, Score: 0.336
Density: 0.4, Depth: 0.8, Variation: 0.8, Score: 0.336
Density: 0.4, Depth: 0.9, Variation: 0.5, Score: 0.276
Density: 0.4, Depth: 0.9, Variation: 0.6, Score: 0.276
Density: 0.4, Depth: 0.9, Variation: 0.7, Score: 0.276
Density: 0.4, Depth: 0.9, Variation: 0.8, Score: 0.276
