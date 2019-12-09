# SharpSharp

![NuGet](https://img.shields.io/nuget/v/SharpSharp)](https://www.nuget.org/packages/SharpSharp)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/randyridge/sharpsharp/release)

This is a terrible .Net Core port of the lovely [sharp](https://github.com/lovell/sharp/) using [NetVips](https://github.com/kleisauke/net-vips).

**_You will need to provide the appropriate NetVips.Native package for your platform._**

I haven't implemented everything, just the pieces I've needed so far. The giant mutable state pipeline baton stuff that they do in sharp makes me uneasy. Maybe at some point I'll refactor it.

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
            .ToFile(@"c:\murray.webp");
    }
}
```
