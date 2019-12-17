using System;
using System.Collections;
using RandyRidge.Common;

namespace SharpSharp {
    /// <summary>
    ///     Provides a difference gradient hash.
    /// </summary>
    public sealed class DifferenceHash {
        private readonly byte[] buffer;
        private readonly Lazy<ImagePipeline> lazyImagePipeline;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="buffer"></param>
        public DifferenceHash(byte[] buffer) {
            Guard.NotNullOrEmpty(buffer, nameof(buffer));
            this.buffer = buffer;
            lazyImagePipeline = new Lazy<ImagePipeline>(CreatePipeline);
        }

        public BitArray ComputeLong() {
            const int ExpectedBits = 64;
            const int Width = 9;
            const int Height = 8;

            var test = NormalizeImage(Width, Height);
            var bitArray = new BitArray(ExpectedBits);
            var bitIndex = 0;
            for(var x = 0; x < Width - 1; x++) {
                for(var y = 0; y < Height; y++) {
                    bitArray.Set(bitIndex, test[y * Height + x] < test[y * Height + x + 1]);
                    bitIndex++;
                }
            }

            return bitArray;
        }

        public BitArray ComputeShort() {
            const int ExpectedBits = 16;
            const int Width = 6;
            const int Height = 4;

            var test = NormalizeImage(Width, Height);
            var bitArray = new BitArray(ExpectedBits);
            var bitIndex = 0;
            for(var x = 0; x < Width - 1; x++) {
                for(var y = 0; y < Height; y++) {
                    if((x == 0 || x == Width - 1) && (y == 0 || y == Height)) {
                        continue;
                    }

                    bitArray.Set(bitIndex, test[y * Height + x] < test[y * Height + x + 1]);
                    bitIndex++;
                }
            }

            return bitArray;
        }

        private ImagePipeline CreatePipeline() =>
            ImagePipeline.FromBuffer(buffer)
                .Grayscale()
                .Normalize()
                .Raw();

        private byte[] NormalizeImage(int width, int height) {
            lazyImagePipeline
                .Value
                .Resize(new ResizeOptions(width, height, Fit.Fill))
                .ToBuffer(out var test);
            return test;
        }
    }
}
