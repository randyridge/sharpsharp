# SharpSharp

[![NuGet](https://img.shields.io/nuget/v/SharpSharp)](https://www.nuget.org/packages/SharpSharp)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/randyridge/sharpsharp/release)

This is a .NET Core port of the [Lovell Fuller](https://github.com/lovell)'s lovely high performance Node.js image processor [sharp](https://sharp.pixelplumbing.com/en/stable/) / [sharp github](https://github.com/lovell/sharp/)).

Sharp uses the low-level image processor [libvips](https://libvips.github.io/libvips/) / [libvips github](https://github.com/libvips/libvips).

This port makes use of [Kleis Auke Wolthuizen](https://github.com/kleisauke)'s libvips .NET binding [NetVips](https://kleisauke.github.io/net-vips/) / [NetVips github](https://github.com/kleisauke/net-vips).

**_You will need to provide the appropriate [NetVips.Native](https://github.com/kleisauke/net-vips#install) package for your platform._**

I haven't implemented everything, just the pieces I've needed so far. Maybe at some point I'll add-to and/or refactor it.

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
