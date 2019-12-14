# SharpSharp

[![NuGet](https://img.shields.io/nuget/v/SharpSharp)](https://www.nuget.org/packages/SharpSharp)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/randyridge/sharpsharp/release)

This is a .NET Core port of Lovell Fuller's lovely NodeJS image processor sharp [github](https://github.com/lovell/sharp/)/[docs](https://sharp.pixelplumbing.com/en/stable/).
It makes use of Kleis Auke Wolthuizen's .NET binding NetVips [github](https://github.com/kleisauke/net-vips)/[docs](https://kleisauke.github.io/net-vips/) of the low-level image processor libvips [github](https://github.com/libvips/libvips)/[docs](https://libvips.github.io/libvips/).

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
