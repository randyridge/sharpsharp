using System;
using System.IO;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
    internal sealed class Processor {
        public void Process(ImageSource imageSource, PipelineBaton baton) {
            Guard.ArgumentNotNull(imageSource, nameof(imageSource));
            Guard.ArgumentNotNull(baton, nameof(baton));

            var (image, imageType) = imageSource.Load();

            baton.Image = image;
            baton.InputImageType = imageType;

            // TODO: Calculate angle of rotation
            // var rotation = VipsAngle.Angle0;

            // TODO: rotate pre-extract
            // TODO: trim
            // TODO: pre-extraction

            // Get pre-resize image width and height
            var inputHeight = image.Height;
            var inputWidth = image.Width;
            // TODO: More rotation stuff

            // Scaling calculations
            var xFactor = 1.0;
            var yFactor = 1.0;
            baton.Height = baton.ResizeOptions.Height;
            baton.Width = baton.ResizeOptions.Width;
            var targetResizeHeight = baton.Height;
            var targetResizeWidth = baton.Width;
            if(baton.Width > 0 && baton.Height > 0) {
                // Fixed width and height
                xFactor = inputWidth / (double) baton.Width;
                yFactor = inputHeight / (double) baton.Height;
                switch(baton.ResizeOptions.Fit) {
                    case Fit.Cover:
                        if(xFactor < yFactor) {
                            targetResizeHeight = (int) Math.Round(inputHeight / xFactor, MidpointRounding.AwayFromZero);
                            yFactor = xFactor;
                        }
                        else {
                            targetResizeWidth = (int) Math.Round(inputWidth / yFactor, MidpointRounding.AwayFromZero);
                            xFactor = yFactor;
                        }

                        break;
                    case Fit.Contain:
                        if(xFactor > yFactor) {
                            targetResizeHeight = (int) Math.Round(inputHeight / xFactor, MidpointRounding.AwayFromZero);
                            yFactor = xFactor;
                        }
                        else {
                            targetResizeWidth = (int) Math.Round(inputWidth / yFactor, MidpointRounding.AwayFromZero);
                            xFactor = yFactor;
                        }

                        break;
                    case Fit.Inside:
                        if(xFactor > yFactor) {
                            targetResizeHeight = (int) Math.Round(inputHeight / xFactor, MidpointRounding.AwayFromZero);
                            yFactor = xFactor;
                            baton.Height = targetResizeHeight;
                        }
                        else {
                            targetResizeWidth = (int) Math.Round(inputWidth / yFactor, MidpointRounding.AwayFromZero);
                            xFactor = yFactor;
                            baton.Width = targetResizeWidth;
                        }

                        break;
                    case Fit.Outside:
                        if(xFactor < yFactor) {
                            targetResizeHeight = (int) Math.Round(inputHeight / xFactor, MidpointRounding.AwayFromZero);
                            yFactor = xFactor;
                            baton.Height = targetResizeHeight;
                        }
                        else {
                            targetResizeWidth = (int) Math.Round(inputWidth / yFactor, MidpointRounding.AwayFromZero);
                            xFactor = yFactor;
                            baton.Height = targetResizeWidth;
                        }

                        break;
                    case Fit.Fill:
                        // TODO: rotation stuff
                        break;
                }
            }
            else if(baton.Width > 0) {
                // Fixed width
                xFactor = inputWidth / (double) baton.Width;
                if(baton.ResizeOptions.Fit == Fit.Fill) {
                    targetResizeHeight = inputHeight;
                    baton.Height = inputHeight;
                }
                else {
                    // Auto height
                    yFactor = xFactor;
                    targetResizeHeight = (int) Math.Round(inputHeight / yFactor, MidpointRounding.AwayFromZero);
                    baton.Height = targetResizeHeight;
                }
            }
            else if(baton.Height > 0) {
                // Fixed height
                yFactor = inputHeight / (double) baton.Height;
                if(baton.ResizeOptions.Fit == Fit.Fill) {
                    targetResizeWidth = inputWidth;
                    baton.Width = inputWidth;
                }
                else {
                    // Auto width
                    xFactor = yFactor;
                    targetResizeWidth = (int) Math.Round(inputWidth / xFactor, MidpointRounding.AwayFromZero);
                    baton.Width = targetResizeWidth;
                }
            }
            else {
                // Identity transform
                baton.Width = inputWidth;
                baton.Height = inputHeight;
            }

            // Calculate integral box shrink
            var xShrink = Math.Max(1, (int) Math.Floor(xFactor));
            var yShrink = Math.Max(1, (int) Math.Floor(yFactor));

            // Calculate residual float affine transformation
            var xResidual = xShrink / xFactor;
            var yResidual = yShrink / yFactor;

            // Do not enlarge the output if the input width *or* height
            // are already less than the required dimensions
            if(baton.ResizeOptions.WithoutEnlargement) {
                if(inputWidth < baton.Width || inputHeight < baton.Height) {
                    xFactor = 1.0;
                    yFactor = 1.0;
                    xShrink = 1;
                    yShrink = 1;
                    xResidual = 1.0;
                    yResidual = 1.0;
                    baton.Width = inputWidth;
                    baton.Height = inputHeight;
                }
            }

            // If integral x and y shrink are equal, try to use shrink-on-load for JPEG and WebP,
            // but not when applying gamma correction, pre-resize extract or trim
            var shrinkOnLoad = 1;
            var shrinkOnLoadFactor = 1;

            // Leave at least a factor of two for the final resize step, when fastShrinkOnLoad: false
            // for more consistent results and avoid occasional small image shifting
            if(!baton.ResizeOptions.FastShrinkOnLoad) {
                shrinkOnLoadFactor = 2;
            }

            if(xShrink == yShrink && xShrink >= 2 * shrinkOnLoadFactor && (imageType == ImageType.Jpeg || imageType == ImageType.WebP)) {
                // TODO: && baton->gamma == 0 && baton->topOffsetPre == -1 && baton->trimThreshold == 0.0
                if(xShrink >= 8 * shrinkOnLoadFactor) {
                    xFactor /= 8;
                    yFactor /= 8;
                    shrinkOnLoad = 8;
                }
                else if(xShrink >= 4 * shrinkOnLoadFactor) {
                    xFactor /= 4;
                    yFactor /= 4;
                    shrinkOnLoad = 4;
                }
                else if(xShrink >= 2 * shrinkOnLoadFactor) {
                    xFactor /= 2;
                    yFactor /= 2;
                    shrinkOnLoad = 2;
                }
            }

            // Help ensure a final kernel-based reduction to prevent shrink aliasing
            if(shrinkOnLoad > 1 && (xResidual.IsAboutEqualTo(1.0) || yResidual.IsAboutEqualTo(1.0))) {
                shrinkOnLoad /= 2;
                xFactor *= 2;
                yFactor *= 2;
            }

            if(shrinkOnLoad > 1) {
                // Reload input using shrink-on-load
                var access = imageSource.Options.UseSequentialRead ? Enums.Access.Sequential : Enums.Access.Random;
                if(imageSource is BufferImageSource bufferImageSource) {
                    image = imageType == ImageType.Jpeg ?
                        Image.JpegloadBuffer(bufferImageSource.Buffer, shrinkOnLoad, null, null, access, true) :
                        Image.WebploadBuffer(bufferImageSource.Buffer, imageSource.Options.PageIndex, imageSource.Options.PageCount, null, null, access, true);
                }

                // Recalculate integral shrink and double residual
                var shrunkOnLoadWidth = image.Width;
                var shrunkOnLoadHeight = image.Height;
                // TODO: rotation pre extract stuff
                // if(baton.rotateBeforePreExtract
                //xFactor = shrunkOnLoadWidth / (double) targetResizeHeight;
                //yFactor = shrunkOnLoadHeight / (double) targetResizeWidth;
                // else {
                xFactor = shrunkOnLoadWidth / (double) targetResizeWidth;
                yFactor = shrunkOnLoadHeight / (double) targetResizeHeight;
            }

            // Ensure we're using a device-independent colour space
            if(image.HasProfile() && image.Interpretation != Enums.Interpretation.Labs) {
                // Convert to sRGB using embedded profile
                try {
                    image = image.IccTransform("srgb", null, Enums.Intent.Perceptual, true);
                }
                catch {
                    // Ignore failure of embedded profile
                }
            }
            else if(image.Interpretation == Enums.Interpretation.Cmyk) {
                image = image.IccTransform("srgb", null, Enums.Intent.Perceptual, null, "cmyk");
            }

            // TODO: Flatten image to remove alpha channel
            // TODO: Negate the colors in the image
            // TODO: Gamma encoding (darken)
            // TODO: Convert to grayscale (linear, therefore after gamma encoding, if any)
            if(baton.ColorizationOptions.HasValue() && baton.ColorizationOptions.MakeGrayscale) {
                image = image.Colourspace(Enums.Interpretation.Bw);
            }

            var shouldResize = !xFactor.IsAboutEqualTo(1.0) || !yFactor.IsAboutEqualTo(1.0);
            // TODO: var shouldBlur 
            // TODO: var shouldConf
            // TODO: var shouldSharpen
            // TODO: var shouldApplyMedian
            // TODO: var shouldComposite
            // TODO: var shouldModulate

            //if(shouldComposite && !image.HasAlpha()) {
            //    image = image.EnsureAlpha();
            //}

            var shouldPremultiplyAlpha = image.HasAlpha() && shouldResize; // TODO: (shouldResize || shouldBlur || shouldConv || shouldSharpen || shouldComposite);
            // Premultiply image alpha channel before all transformations to avoid
            // dark fringing around bright pixels
            // See: http://entropymine.com/imageworsener/resizealpha/
            if(shouldPremultiplyAlpha) {
                image = image.Premultiply();
            }

            // Resize
            if(shouldResize) {
                var kernel = baton.ResizeOptions.Kernel;
                // Ensure shortest edge is at least 1 pixel
                if(image.Width / xFactor < 0.5) {
                    xFactor = 2 * image.Width;
                    baton.Width = 1;
                }

                if(image.Height / yFactor < 0.5) {
                    yFactor = 2 * image.Height;
                    baton.Height = 1;
                }

                image = image.Resize(1.0 / xFactor, kernel, 1.0 / yFactor);
            }

            // TODO: Rotate post-extract 90-angle
            // TODO: Flip
            // TODO: Flop
            // TODO: Join additional color channels to the image

            // Crop/embed
            if(image.Width != baton.Width || image.Height != baton.Height) {
                var fit = baton.ResizeOptions.Fit;
                if(fit == Fit.Contain) {
                    // TODO: Embed
                }
                else if(fit != Fit.Fill && image.Width > baton.Width || image.Height > baton.Height) {
                    // Crop/max/min
                    // TODO: if(baton.Position < 9) {
                    // Gravity-based crop
                    var (left, top) = NeedsBetterPlace.CalculateCrop(image.Width, image.Height, baton.Width, baton.Height, Gravity.Center);//, baton.Position);
                    var width = Math.Min(image.Width, baton.Width);
                    var height = Math.Min(image.Height, baton.Height);
                    image = image.ExtractArea(left, top, width, height);
                    //}
                    //else {
                    // TODO: Attention-based or Entropy-based crop
                    //}
                }
            }

            // TODO: Rotate post-extract non-90 angle
            // TODO: Post extraction
            // TODO: Extend edges
            // TODO: Median - must happen before blurring, due to the utility of blurring after thresholding
            // TODO: Threshold - must happen before blurring, due to the utility of blurring after thresholding
            // TODO: Blur
            // TODO: Convolve
            // TODO: Recomb:
            // TODO: Modulate
            // TODO: Sharpen

            if(baton.SharpenOptions.HasValue()) {
                image = ImageExtensions.Sharpen(image, baton.SharpenOptions.Sigma, baton.SharpenOptions.Flat ?? 1, baton.SharpenOptions.Jagged ?? 2);
            }

            // TODO: Composite
            // TODO: Reverse premultiplication after all transformations:
            // TODO: Gamma decoding (brighten)
            // TODO: Linear adjustment (a * in + b)

            // Apply normalization - stretch luminance to cover full dynamic range
            if(baton.OperationOptions.HasValue() && baton.OperationOptions.MakeNormalized) {
                image = image.Normalize();
            }

            // TODO: Apply bitwise boolean operation between images
            // TODO: Apply per-channel Bandbool bitwise operations after all other operations
            // TODO: Tint the image
            // TODO: Extract an image channel (aka vips band)
            // TODO: Remove alpha channel, if any
            if(baton.ChannelOptions.HasValue() && baton.ChannelOptions.RemoveAlpha) {
                image = image.RemoveAlpha();
            }

            // TODO: Ensure alpha channel, if missing
            // TODO: Convert image to sRGB, if not already
            // TODO: Override EXIF Orientation tag

            // Number of channels used in output image
            baton.Channels = image.Bands;
            baton.Width = image.Width;
            baton.Height = image.Height;

            // Output
            // TODO: Buffer out
            var strip = baton.MetadataOptions.ShouldStripMetadata;

            baton.OutputImageInfo = new OutputImageInfo {
                // TODO: premultiplied, cropoffset*
                Channels = baton.Channels,
                Height = baton.Height,
                Width = baton.Width,
            };

            if(baton.ToStreamOptions.HasValue()) {
                var streamOptions = baton.ToStreamOptions;
                baton.ToBufferOptions = new ToBufferOptions(bytes => {
                    streamOptions.Stream.Write(bytes);
                });
            }

            if(baton.ToBufferOptions.HasValue()) {
                var bufferOptions = baton.ToBufferOptions;
                var buffer = Array.Empty<byte>();
                if(baton.JpegOptions.HasValue()) {
                    var o = baton.JpegOptions;
                    buffer = image.JpegsaveBuffer(
                        null,
                        o.Quality,
                        null,
                        o.OptimizeCoding,
                        o.MakeProgressive,
                        o.NoSubsampling,
                        o.ApplyTrellisQuantization,
                        o.ApplyOvershootDeringing,
                        o.OptimizeScans,
                        o.QuantizationTable,
                        strip // TODO: this
                    );
                    baton.OutputImageInfo.Format = "jpeg";
                    bufferOptions.Callback(buffer);
                }
                else if(baton.WebpOptions.HasValue()) {
                    var o = baton.WebpOptions;
                    buffer = image.WebpsaveBuffer(
                        null, // TODO: Shouldn't this have a value for animations?
                        o.Quality,
                        o.UseLossless,
                        null,
                        o.UseSmartSubsample,
                        o.UseNearLossless,
                        o.AlphaQuality,
                        null,
                        null,
                        null,
                        o.ReductionEffort,
                        strip
                    );
                    baton.OutputImageInfo.Format = "webp";
                    bufferOptions.Callback(buffer);
                }
                else if(baton.PngOptions.HasValue()) {
                    var o = baton.PngOptions;
                    buffer = image.PngsaveBuffer(
                        o.CompressionLevel,
                        o.MakeProgressive,
                        null,
                        null,
                        o.UseAdaptiveFiltering ? 0xF8 : 0x08, // TODO: enums
                        o.UsePalette,
                        o.Colors,
                        o.Quality,
                        o.Dither,
                        strip
                    );
                    baton.OutputImageInfo.Format = "png";
                    bufferOptions.Callback(buffer);
                }
                else if(baton.RawOptions.HasValue()) {
                    // Write raw, uncompressed image data to buffer
                    if(baton.ColorizationOptions.HasValue() && baton.ColorizationOptions.MakeGrayscale || image.Interpretation == Enums.Interpretation.Bw) {
                        // Extract first band for greyscale image
                        image = image[0];
                    }

                    if(image.Format != Enums.BandFormat.Uchar) {
                        // Cast pixels to uint8 (unsigned char)
                        image = image.Cast(Enums.BandFormat.Uchar);
                    }

                    buffer = image.WriteToMemory();
                    baton.OutputImageInfo.Format = "raw";
                    bufferOptions.Callback(buffer);
                }
                else {
                    throw new NotImplementedException("Unknown buffer output.");
                }
                baton.OutputImageInfo.Size = buffer.Length;
            }

            if(baton.ToFileOptions.HasValue()) {
                var fileOptions = baton.ToFileOptions;
                var path = fileOptions.FilePath;
                // File output
                if(baton.JpegOptions.HasValue()) {
                    var o = baton.JpegOptions;
                    image.Jpegsave(
                        path,
                        null,
                        o.Quality,
                        null,
                        o.OptimizeCoding,
                        o.MakeProgressive,
                        o.NoSubsampling,
                        o.ApplyTrellisQuantization,
                        o.ApplyOvershootDeringing,
                        o.OptimizeScans,
                        o.QuantizationTable,
                        strip // TODO: this
                    );
                    baton.OutputImageInfo.Format = "jpeg";
                }
                else if(baton.WebpOptions.HasValue()) {
                    var o = baton.WebpOptions;
                    image.Webpsave(
                        path,
                        null, // TODO: Shouldn't this have a value for animations?
                        o.Quality,
                        o.UseLossless,
                        null,
                        o.UseSmartSubsample,
                        o.UseNearLossless,
                        o.AlphaQuality,
                        null,
                        null,
                        null,
                        o.ReductionEffort,
                        strip
                    );
                    baton.OutputImageInfo.Format = "webp";
                }
                else if(baton.PngOptions.HasValue()) {
                    var o = baton.PngOptions;
                    image.Pngsave(
                        path,
                        o.CompressionLevel,
                        o.MakeProgressive,
                        null,
                        null,
                        o.UseAdaptiveFiltering ? 0xF8 : 0x08, // TODO: enums
                        o.UsePalette,
                        o.Colors,
                        o.Quality,
                        o.Dither,
                        strip
                    );
                    baton.OutputImageInfo.Format = "png";
                }

                baton.OutputImageInfo.Size = (int) new FileInfo(path).Length; // TODO: this seems bad
            }


            image?.Dispose();
        }
    }
}
