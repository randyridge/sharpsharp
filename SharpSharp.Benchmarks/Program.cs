using System;
using System.IO;
using BenchmarkDotNet.Running;
using NetVips;
using SharpSharp.Tests;

namespace SharpSharp.Benchmarks {
    internal static class Program {
        private static void Main() {
            //var buffer = File.ReadAllBytes(@"D:\tsv_20191029-src.jpg");

            //ImagePipeline
            //    .FromBuffer(buffer)
            //    .Resize(1170, 487)
            //    .Jpeg(new JpegOptions(95, false, false, false, false, false, true, 0))
            //    .ToFile(@"d:\tsv-same-qxh.jpg");

            //ImagePipeline
            //    .FromBuffer(buffer)
            //    .Resize(1170, 487)
            //    .Jpeg(new JpegOptions())
            //    .ToFile(@"d:\tsv-qxh.jpg");

            var left = Image.NewFromFile(@"D:\tsv_20191029.jpg");
            var right = Image.NewFromFile(@"D:\tsv_20191029.jpg");
            var leftFingerprint = ImageUtilities.Fingerprint(left);
            var rightFingerprint = ImageUtilities.Fingerprint(right);
            var mcd = ImageUtilities.MaxColorDistance(left, right);
            var mse = ImageUtilities.MeanSquaredError(left, right);
            var psnr = ImageUtilities.PeakSignalToNoiseRatio(left, right);
            var ssim = ImageUtilities.StructuralSimilarity(left, right);
            Console.WriteLine($"Max color distance = {mcd}");
            Console.WriteLine($"Mean squared error = {mse}");
            Console.WriteLine($"Peak signal to noise ratio = {psnr}");
            Console.WriteLine($"SSIM = {ssim}");
            Console.WriteLine($"Left Fingerprint = {leftFingerprint}");
            Console.WriteLine($"Right Fingerprint = {rightFingerprint}");
        }
    }
    //        private static void Main(string[] args = null) =>
    //            BenchmarkSwitcher
    //                .FromAssembly(typeof(Program).Assembly)
    //#if DEBUG
    //                .Run(args, new BenchmarkDotNet.Configs.DebugInProcessConfig());
    //#else
    //                .Run(args);
    //#endif
    //    }
}
