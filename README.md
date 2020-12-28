# SharpSharp

[![NuGet](https://img.shields.io/nuget/v/SharpSharp)](https://www.nuget.org/packages/SharpSharp)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/randyridge/sharpsharp/release)

The typical use case for this high-speed .NET Core library is to convert large images in common formats to smaller, web-friendly JPEG, PNG, WebP, and AVIF images of varying dimensions.

This is a .NET Core port of the [Lovell Fuller](https://github.com/lovell)'s lovely high-performance Node.js image processor [sharp](https://sharp.pixelplumbing.com/en/stable/) / [sharp github](https://github.com/lovell/sharp/).

Sharp uses the low-level image processor [libvips](https://libvips.github.io/libvips/) / [libvips github](https://github.com/libvips/libvips).

This port makes use of [Kleis Auke Wolthuizen](https://github.com/kleisauke)'s libvips .NET binding [NetVips](https://kleisauke.github.io/net-vips/) / [NetVips github](https://github.com/kleisauke/net-vips).

**_You will need to provide the appropriate [NetVips.Native](https://github.com/kleisauke/net-vips#install) package for your platform._**

I haven't implemented everything, just the pieces I've needed so far. Maybe at some point I'll add-to and/or refactor it.

## Install
```sh
Install-Package SharpSharp -Version 0.6.0-alpha1
```

``` csharp
using SharpSharp;
```

## Examples
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

## Benchmark Results ([sharp's benchmark](https://sharp.pixelplumbing.com/performance))

### Task
Decompress a 2725x2225 JPEG image,
resize to 720x588 using Lanczos 3 resampling (where available),
then compress to JPEG at a "quality" setting of 80.

### Environment
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.685 (2004/?/20H1)
Intel Core i7-2600K CPU 3.40GHz (Sandy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  Job-YJAIQO : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT

Runtime=.NET Core 5.0

### Contenders

* [FreeImage-dotnet-core](https://github.com/matgr1/FreeImage-dotnet-core) v4.3.6
* [Magick.NET-Q8-AnyCPU](https://github.com/dlemstra/Magick.NET) v7.22.2.2
* [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) v1.0.2
* [SkiaSharp](https://github.com/mono/SkiaSharp) v2.80.2
* [SharpSharp](https://github.com/randyridge/sharpsharp) v0.6.0-alpha1
  
### Results
|                         Method |       Mean |     Error |     StdDev |    StdErr |        Min |         Q1 |     Median |         Q3 |        Max |    Op/s |  Ratio | RatioSD |     Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|------------------------------- |-----------:|----------:|-----------:|----------:|-----------:|-----------:|-----------:|-----------:|-----------:|--------:|-------:|--------:|----------:|----------:|----------:|-----------:|
|  'SharpSharp Buffer to Buffer' |   3.975 ms | 0.0786 ms |  0.1356 ms | 0.0220 ms |   3.734 ms |   3.867 ms |   3.960 ms |   4.086 ms |   4.324 ms | 251.561 |   1.00 |    0.00 |  328.1250 |  328.1250 |  328.1250 |   10.04 KB |
|      'SharpSharp File to File' |   5.302 ms | 0.1036 ms |  0.1518 ms | 0.0282 ms |   4.983 ms |   5.185 ms |   5.298 ms |   5.381 ms |   5.576 ms | 188.622 |   1.33 |    0.06 |         - |         - |         - |   11.38 KB |
|   'SkiaSharp Buffer to Buffer' | 104.248 ms | 2.0808 ms |  2.1369 ms | 0.5183 ms | 101.145 ms | 102.711 ms | 104.248 ms | 105.266 ms | 108.725 ms |   9.593 |  26.12 |    1.05 |  200.0000 |         - |         - | 1023.89 KB |
|       'SkiaSharp File to File' | 106.636 ms | 2.0920 ms |  2.4092 ms | 0.5387 ms | 102.911 ms | 104.962 ms | 106.163 ms | 108.947 ms | 110.617 ms |   9.378 |  26.68 |    1.18 |  200.0000 |         - |         - |  964.06 KB |
|  'ImageSharp Buffer to Buffer' | 195.648 ms | 3.8943 ms |  9.3305 ms | 1.1315 ms | 177.560 ms | 188.953 ms | 193.276 ms | 201.672 ms | 219.222 ms |   5.111 |  49.64 |    3.18 |         - |         - |         - |  368.17 KB |
|      'ImageSharp File to File' | 197.205 ms | 3.9118 ms |  7.9908 ms | 1.1189 ms | 183.202 ms | 192.083 ms | 196.480 ms | 202.130 ms | 214.361 ms |   5.071 |  49.49 |    2.69 |         - |         - |         - |   53.59 KB |
|   'FreeImage Buffer to Buffer' | 212.982 ms | 4.2326 ms |  5.3528 ms | 1.1161 ms | 205.365 ms | 209.224 ms | 212.683 ms | 216.850 ms | 225.572 ms |   4.695 |  53.44 |    2.54 | 1000.0000 | 1000.0000 | 1000.0000 |  189.62 KB |
|       'FreeImage File to File' | 230.281 ms | 4.5688 ms |  5.2614 ms | 1.1765 ms | 222.607 ms | 226.276 ms | 230.792 ms | 233.663 ms | 241.642 ms |   4.343 |  57.62 |    2.78 | 1000.0000 | 1000.0000 | 1000.0000 |   12.82 KB |
|     'ImageMagick File to File' | 409.113 ms | 7.6457 ms | 13.5903 ms | 2.1488 ms | 384.667 ms | 400.364 ms | 408.498 ms | 417.691 ms | 436.796 ms |   2.444 | 103.06 |    4.07 |         - |         - |         - |   17.31 KB |
| 'ImageMagick Buffer to Buffer' | 411.799 ms | 8.1147 ms | 12.6336 ms | 2.2333 ms | 386.371 ms | 403.606 ms | 411.966 ms | 420.014 ms | 439.007 ms |   2.428 | 103.59 |    4.87 |         - |         - |         - |  342.49 KB |
