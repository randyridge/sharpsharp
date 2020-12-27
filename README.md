# SharpSharp

[![NuGet](https://img.shields.io/nuget/v/SharpSharp)](https://www.nuget.org/packages/SharpSharp)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/randyridge/sharpsharp/release)

This is a .NET Core port of the [Lovell Fuller](https://github.com/lovell)'s lovely high-performance Node.js image processor [sharp](https://sharp.pixelplumbing.com/en/stable/) / [sharp github](https://github.com/lovell/sharp/)).

Sharp uses the low-level image processor [libvips](https://libvips.github.io/libvips/) / [libvips github](https://github.com/libvips/libvips).

This port makes use of [Kleis Auke Wolthuizen](https://github.com/kleisauke)'s libvips .NET binding [NetVips](https://kleisauke.github.io/net-vips/) / [NetVips github](https://github.com/kleisauke/net-vips).

**_You will need to provide the appropriate [NetVips.Native](https://github.com/kleisauke/net-vips#install) package for your platform._**

I haven't implemented everything, just the pieces I've needed so far. Maybe at some point I'll add-to and/or refactor it.

## Example
``` csharp
using System.Threading.Tasks;
using SharpSharp;

namespace Demo {
    internal static class Program {
        private static async Task Main() =>
            (await ImagePipeline.FromUriAsync("https://www.fillmurray.com/300/300"))
            .Resize(150, 150)
            .Sharpen()
            .Webp()
            .ToFile("murray.webp");
    }
}
```

## Benchmark
```
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.685 (2004/?/20H1)
Intel Core i7-2600K CPU 3.40GHz (Sandy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  Job-AAXGPA : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT

Runtime=.NET Core 5.0

|                     Method |       Mean |     Error |     StdDev |    StdErr |        Min |         Q1 |     Median |         Q3 |        Max |    Op/s | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------------- |-----------:|----------:|-----------:|----------:|-----------:|-----------:|-----------:|-----------:|-----------:|--------:|------:|--------:|---------:|---------:|---------:|----------:|
|                 ImageSharp | 178.158 ms | 3.4904 ms |  5.9270 ms | 0.9744 ms | 169.338 ms | 174.704 ms | 176.968 ms | 181.595 ms | 193.143 ms |   5.613 | 35.69 |    1.58 |        - |        - |        - |  53.44 KB |
|     MagickFromBufferToFile | 410.957 ms | 7.7890 ms |  7.2858 ms | 1.8812 ms | 397.375 ms | 404.889 ms | 412.754 ms | 416.415 ms | 420.528 ms |   2.433 | 81.58 |    2.39 |        - |        - |        - |  18.45 KB |
|       MagickFromFileToFile | 412.873 ms | 8.1176 ms | 11.3797 ms | 2.1900 ms | 384.312 ms | 406.413 ms | 415.170 ms | 420.213 ms | 432.843 ms |   2.422 | 83.05 |    3.76 |        - |        - |        - |  17.31 KB |
|     MagickFromStreamToFile | 413.492 ms | 8.0051 ms | 11.2221 ms | 2.1597 ms | 394.712 ms | 405.396 ms | 411.045 ms | 421.273 ms | 436.576 ms |   2.418 | 83.16 |    3.29 |        - |        - |        - |   30.7 KB |
| SharpSharpFromBufferToFile |   4.996 ms | 0.0990 ms |  0.1708 ms | 0.0277 ms |   4.675 ms |   4.855 ms |   4.972 ms |   5.101 ms |   5.447 ms | 200.166 |  1.00 |    0.00 | 328.1250 | 328.1250 | 328.1250 |  10.69 KB |
|   SharpSharpFromFileToFile |   5.454 ms | 0.1043 ms |  0.1827 ms | 0.0293 ms |   5.171 ms |   5.337 ms |   5.477 ms |   5.570 ms |   5.870 ms | 183.368 |  1.09 |    0.05 |        - |        - |        - |   11.2 KB |
| SharpSharpFromStreamToFile |   5.366 ms | 0.1054 ms |  0.1213 ms | 0.0271 ms |   5.100 ms |   5.277 ms |   5.350 ms |   5.434 ms |   5.586 ms | 186.364 |  1.07 |    0.04 |        - |        - |        - |  13.99 KB |
```

