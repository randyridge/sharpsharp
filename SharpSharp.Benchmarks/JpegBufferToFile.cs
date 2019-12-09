using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using ImageMagick;
using SharpSharp.Tests;

namespace SharpSharp.Benchmarks {
    [Config(typeof(DefaultConfig))]
    public class JpegBufferToFile {
        private readonly byte[] buffer;
        private int height = 588;
        private int width = 720;

        public JpegBufferToFile() {
            NetVips.NetVips.ConcurrencySet(Environment.ProcessorCount);
            buffer = File.ReadAllBytes(TestImageData.InputJpg);
        }

        [Benchmark(Baseline = true)]
        public void SharpSharp() {
            ImagePipeline
                .FromBuffer(buffer)
                .Resize(width, height)
                .ToFile(TestImageData.OutputJpg);
        }

        [Benchmark]
        public void Magick() {
            var image = new MagickImage(buffer) {
                FilterType = FilterType.Lanczos,
                Quality = 80
            };
            image.Resize(width, height);
            image.Write(TestImageData.OutputJpg, MagickFormat.Jpg);
            image.Dispose();
        }
    }
}
