using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp {
    public static class ImageUtilities {
        public static string Fingerprint(Image image) {
            Guard.ArgumentNotNull(image, nameof(image));
            var fingerPrint = string.Empty;
            ImagePipeline
                .FromImage(image)
                .Grayscale()
                .Normalize()
                .Resize(new ResizeOptions(9, 8, Fit.Fill))
                .Raw()
                .ToBuffer(new ToBufferOptions(data => {
                    var builder = new StringBuilder(64);
                    for(var x = 0; x < 8; x++) {
                        for(var y = 0; y < 8; y++) {
                            var left = data[y * 8 + x];
                            var right = data[y * 8 + x + 1];
                            builder.Append(left < right ? '1' : '0');
                        }
                    }

                    fingerPrint = builder.ToString();
                }));
            return fingerPrint;
        }

        public static double MaxColorDistance(Image left, Image right) {
            GuardImages(left, right);

            if(left.HasAlpha()) {
                left = left.Premultiply().ExtractBand(1, left.Bands - 1);
            }

            if(right.HasAlpha()) {
                right = right.Premultiply().ExtractBand(1, right.Bands - 1);
            }

            return left.DE00(right).Max();
        }

        public static double MeanSquaredError(Image left, Image right) {
            GuardImages(left, right);

            var leftBuffer = ImagePipeline
                .FromImage(left)
                .Raw()
                .ToBuffer();

            var rightBuffer = ImagePipeline
                .FromImage(right)
                .Raw()
                .ToBuffer();

            if(leftBuffer == null || rightBuffer == null) {
                throw new SharpSharpException("Failed to load buffers.");
            }

            var sumSquared = 0;
            for(var y = 0; y < left.Height; y++) {
                for(var x = 0; x < left.Width; x++) {
                    var p1 = leftBuffer[y * left.Height + x];
                    var p2 = rightBuffer[y * left.Height + x];
                    var error = p2 - p1;
                    sumSquared += error * error;
                }
            }

            return sumSquared / (double) (left.Height * left.Width);
        }

        public static double PeakSignalToNoiseRatio(Image left, Image right) {
            GuardImages(left, right);
            var leftBuffer = ImagePipeline
                .FromImage(left)
                .Raw()
                .ToBuffer();

            var rightBuffer = ImagePipeline
                .FromImage(right)
                .Raw()
                .ToBuffer();

            if(leftBuffer == null || rightBuffer == null) {
                throw new SharpSharpException("Failed to load buffers.");
            }

            double se = 0;
            for(var y = 0; y < left.Height; y++) {
                for(var x = 0; x < left.Width; x++) {
                    for(var channel = 0; channel < left.Bands; channel++) {
                        double e = leftBuffer[y * left.Height + x + channel] - rightBuffer[y * left.Height + x + channel];
                        se += e * e;
                    }
                }
            }

            var mse = se / (left.Width * left.Height * left.Bands);
            return 10 * Math.Log10(255 * 255 / mse);
        }

        public static double StructuralSimilarity(Image left, Image right) {
            GuardImages(left, right);
            var leftBuffer = ImagePipeline
                .FromImage(left)
                .Raw()
                .ToBuffer();

            var rightBuffer = ImagePipeline
                .FromImage(right)
                .Raw()
                .ToBuffer();

            if(leftBuffer == null || rightBuffer == null) {
                throw new SharpSharpException("Failed to load buffers.");
            }

            var k1 = 0.01;
            var k2 = 0.03;
            var c1 = k1 * 255 * (k1 * 255);
            var c2 = k2 * 255 * (k2 * 255);
            var c3 = c2 / 2;

            double valSum1 = 0;
            double valSum2 = 0;
            for(var y = 0; y < left.Height; y++) {
                for(var x = 0; x < left.Width; x++) {
                    for(var channel = 0; channel < left.Bands; channel++) {
                        valSum1 += leftBuffer[y * left.Height + x + channel];
                        valSum2 += rightBuffer[y * left.Height + x + channel];
                    }
                }
            }

            var miu1 = valSum1 / (left.Width * left.Height * left.Bands);
            var miu2 = valSum2 / (left.Width * left.Height * left.Bands);

            double varSum1 = 0;
            double varSum2 = 0;
            double covSum = 0;
            for(var y = 0; y < left.Height; y++) {
                for(var x = 0; x < left.Width; x++) {
                    for(var channel = 0; channel < left.Bands; channel++) {
                        var e1 = leftBuffer[y * left.Height + x + channel] - miu1;
                        var e2 = rightBuffer[y * left.Height + x + channel] - miu2;
                        varSum1 += e1 * e1;
                        varSum2 += e2 * e2;
                        covSum += e1 * e2;
                    }
                }
            }

            var sigma1 = Math.Sqrt(varSum1 / (left.Width * left.Height * left.Bands - 1));
            var sigma2 = Math.Sqrt(varSum2 / (left.Width * left.Height * left.Bands - 1));
            var sigma12 = covSum / (left.Width * left.Height * left.Bands - 1);

            var l = (2 * miu1 * miu2 + c1) / (miu1 * miu1 + miu2 * miu2 + c1);
            var c = (2 * sigma1 * sigma2 + c2) / (sigma1 * sigma1 + sigma2 * sigma2 + c2);
            var s = (sigma12 + c3) / (sigma1 * sigma2 + c3);
            return l * c * s;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GuardChannels(Image left, Image right) {
            if(left.Bands != right.Bands) {
                throw new ArgumentException($"Mismatched bands: left ({left.Bands}) right ({right.Bands})");
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GuardDimensions(Image left, Image right) {
            if(left.Width != right.Width || left.Height != right.Height) {
                throw new ArgumentException($"Mismatched dimensions: left ({left.Width}x{left.Height}) right ({right.Width}x{right.Height})");
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GuardImages(Image left, Image right) {
            Guard.ArgumentNotNull(left, nameof(left));
            Guard.ArgumentNotNull(right, nameof(right));
            GuardChannels(left, right);
            GuardDimensions(left, right);
        }
    }
}
