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
          ImagePipeline.
            .FromFile("test.jpg")
            .Resize(720, 588)
            .ToJpeg("murray.webp");
    }
}


#### Input from URL
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

<!---
### Results
|                         Method |    Op/s |  Ratio |     Gen 0 |     Gen 1 |     Gen 2 |  Allocated |
|------------------------------- |--------:|-------:|----------:|----------:|----------:|-----------:|
|  'SharpSharp Buffer to Buffer' | 251.561 |   1.00 |  328.1250 |  328.1250 |  328.1250 |   10.04 KB |
|      'SharpSharp File to File' | 188.622 |   1.33 |         - |         - |         - |   11.38 KB |
|   'SkiaSharp Buffer to Buffer' |   9.593 |  26.12 |  200.0000 |         - |         - | 1023.89 KB |
|       'SkiaSharp File to File' |   9.378 |  26.68 |  200.0000 |         - |         - |  964.06 KB |
|  'ImageSharp Buffer to Buffer' |   5.111 |  49.64 |         - |         - |         - |  368.17 KB |
|      'ImageSharp File to File' |   5.071 |  49.49 |         - |         - |         - |   53.59 KB |
|   'FreeImage Buffer to Buffer' |   4.695 |  53.44 | 1000.0000 | 1000.0000 | 1000.0000 |  189.62 KB |
|       'FreeImage File to File' |   4.343 |  57.62 | 1000.0000 | 1000.0000 | 1000.0000 |   12.82 KB |
|     'ImageMagick File to File' |   2.444 | 103.06 |         - |         - |         - |   17.31 KB |
| 'ImageMagick Buffer to Buffer' |   2.428 | 103.59 |         - |         - |         - |  342.49 KB |
-->