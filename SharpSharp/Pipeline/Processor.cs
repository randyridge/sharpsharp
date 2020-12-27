﻿using System;
using System.IO;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
	internal sealed class Processor {
		private static readonly bool SupportsGifOutput =
			NetVips.NetVips.TypeFind("VipsOperation", "magicksave") != IntPtr.Zero &&
			NetVips.NetVips.TypeFind("VipsOperation", "magicksave_buffer") != IntPtr.Zero;

		public void Process(ImageSource imageSource, PipelineBaton baton) {
			Guard.NotNull(imageSource, nameof(imageSource));
			Guard.NotNull(baton, nameof(baton));

			var (image, imageType) = LoadImage(imageSource, baton);

			var (rotation, rotationOptions) = CalculateAngleOfRotation(baton, image);
			image = RotatePreExtract(image, rotation, rotationOptions);
			image = Trim(baton, image);
			image = PreExtraction(baton, image);

			var (inputWidth, inputHeight) = GetPreResizeWidthAndHeight(baton, image, rotation);
			ClipWithoutEnlargement(baton, inputWidth, inputHeight);

			var (xFactor, yFactor, targetResizeWidth, targetResizeHeight) = CalculateScaling(baton, inputWidth, inputHeight);
			var (xShrink, yShrink) = CalculateIntegralBoxShrink(xFactor, yFactor);
			var (xResidual, yResidual) = CalculateResidualFloatAffineTransformation(xShrink, xFactor, yShrink, yFactor);
			image = ShrinkOnLoad(baton, image, imageType, imageSource, ref xFactor, ref yFactor, xShrink, yShrink, xResidual, yResidual, rotation, targetResizeWidth, targetResizeHeight);
			image = EnsureDeviceIndependentColorSpace(image);
			image = RemoveAlphaChannel(baton, image);
			image = NegateColors(baton, image);
			image = ApplyGammaEncoding(baton, image);
			image = ConvertToGrayscale(baton, image);

			var oo = baton.OperationOptions;
			var shouldResize = !xFactor.IsAboutEqualTo(1.0) || !yFactor.IsAboutEqualTo(1.0);
			var shouldBlur = !oo.BlurSigma.IsAboutEqualTo(0.0);
			var shouldConvolve = oo.ConvolveKernelWidth * oo.ConvolveKernelHeight > 0;
			var shouldSharpen = !oo.SharpenSigma.IsAboutEqualTo(0.0);
			var shouldComposite = oo.Composite.HasValue();
			var shouldPremultiplyAlpha =  image.HasAlpha() && (shouldResize || shouldBlur || shouldConvolve || shouldSharpen || shouldComposite);
			image = CreateAlphaChannel(image, shouldComposite);
			image = PremultiplyAlphaChannel(image, shouldPremultiplyAlpha);
			image = Resize(baton, image, shouldResize, xFactor, yFactor);
			image = RotatePostExtract(baton, image, rotation);
			image = Flip(baton, image);
			image = Flop(baton, image);
			image = JoinAdditionalColorChannels(baton, image);
			image = CropOrEmbed(baton, image);
			image = RotatePostExtract(baton, image);
			image = PostExtraction(baton, image);
			image = AffineTransform(baton, image);
			image = ExtendEdges(baton, image);
			image = Median(baton, image);
			image = Threshold(baton, image);
			image = Blur(baton, image);
			image = Convolve(baton, image);
			image = Recomb(baton, image);
			image = Modulate(baton, image);
			image = Sharpen(baton, image);
			// TODO: Composite
			image = ReversePremultiplication(baton, image, shouldPremultiplyAlpha);
			image = ApplyGammaDecoding(baton, image);
			image = LinearAdjustment(baton, image);
			image = Normalize(baton, image);
			// TODO: Apply bitwise boolean operation between images
			image = ApplyBandBoolOp(baton, image);
			image = Tint(baton, image);
			image = ExtractVipsBand(baton, image);
			image = RemoveAlphaChannelIfAny(baton, image);
			image = EnsureAlphaChannelIfMissing(baton, image);
			image = ConvertToSrgb(baton, image);
			image = ApplyIccProfile(baton, image);
			image = OverrideExifOrientation(baton, image);
			// Number of channels used in output image
			// TODO: Where should these go
			baton.Channels = image.Bands;
			baton.Width = image.Width;
			baton.Height = image.Height;

			image = SetAnimationPropertiesIfAny(baton, image);

			// Output
			if(baton.ToBufferOptions.HasValue()) {
					PotentiallySaveJpegBuffer(baton, image, imageType);
					PotentiallySavePngBuffer(baton, image, imageType);
					PotentiallySaveWebpBuffer(baton, image, imageType);
					PotentiallySaveGifBuffer(baton, image, imageType);
					PotentiallySaveTiffBuffer(baton, image, imageType);
					PotentiallySaveHeifBuffer(baton, image, imageType);
					PotentiallySaveRawBuffer(baton, image, imageType);
					baton.OutputImageInfo.Size = baton.ToBufferOptions.Buffer.Length;
					// TODO: Handle unknown
			}
			if(baton.ToFileOptions.HasValue()) {
				var fo = baton.ToFileOptions;
				var path = fo.FilePath;

				//var isJpeg = sharp::IsJpeg(baton->fileOut);
				//var isPng = sharp::IsPng(baton->fileOut);
				//var isWebp = sharp::IsWebp(baton->fileOut);
				//var isGif = sharp::IsGif(baton->fileOut);
				//var isTiff = sharp::IsTiff(baton->fileOut);
				//var isHeif = sharp::IsHeif(baton->fileOut);
				//var isDz = sharp::IsDz(baton->fileOut);
				//var isDzZip = sharp::IsDzZip(baton->fileOut);
				//var isV = sharp::IsV(baton->fileOut);
				//var mightMatchInput = baton->formatOut == "input";
				//var willMatchInput = mightMatchInput &&
				//	!(isJpeg || isPng || isWebp || isGif || isTiff || isHeif || isDz || isDzZip || isV);

				// File output
				if(baton.JpegOptions.HasValue()) {
					var o = baton.JpegOptions;
					image.Jpegsave(
						path,
						o.Quality,
						null,
						o.OptimizeCoding,
						o.MakeProgressive,
						subsampleMode:"4:4:4", // TODO: this
						trellisQuant:o.ApplyTrellisQuantization,
						overshootDeringing:o.ApplyOvershootDeringing,
						optimizeScans:o.OptimizeScans,
						quantTable:o.QuantizationTable,
						strip: baton.MetadataOptions.ShouldStripMetadata,
						background:null,
						pageHeight:null
					);
					baton.OutputImageInfo.Format = "jpeg";
				}
				else if(baton.WebpOptions.HasValue()) {
					var o = baton.WebpOptions;
					image.Webpsave(
						path,
						o.Quality,
						o.UseLossless,
						null,
						o.UseSmartSubsample,
						o.UseNearLossless,
						o.AlphaQuality,
						null,
						null,
						null,
						reductionEffort: o.ReductionEffort,
						strip: baton.MetadataOptions.ShouldStripMetadata
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
						o.UseAdaptiveFiltering ? 0xF8 : 0x08, // TODO: enums
						o.UsePalette,
						o.Quality,
						o.Dither,
						strip: baton.MetadataOptions.ShouldStripMetadata,
						background:null,
						pageHeight:null // TODO: value for animation?
					);
					baton.OutputImageInfo.Format = "png";
				}
				else if(baton.HeifOptions.HasValue()) {
					var o = baton.HeifOptions;
					image.Heifsave("lichtenstein.avif", q: 50, compression: Enums.ForeignHeifCompression.Av1, speed: 6, strip: true);
					//image.Heifsave(path, o.Quality, o.UseLossless, o.Compression, 5, strip);
				}

				//baton.OutputImageInfo.Size = (int) new FileInfo(path).Length; // TODO: this seems bad
			}


			//if(baton.ToStreamOptions.HasValue()) {
			//	baton.ToStreamOptions.Stream.Write(streamBytes);
			//}

			image?.Dispose();
		}

		private static void PotentiallySaveRawBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputImageInfo.Format;
			var ro = baton.RawOptions;
			var bo = baton.ToBufferOptions;
			if(ro != null && bo != null && (formatOut == "raw" || (formatOut == "input" && imageType == ImageType.Raw))) {
				// Write raw, uncompressed image data to buffer
				if(baton.OperationOptions.Grayscale || image.Interpretation == Enums.Interpretation.Bw) {
					// Extract first band for greyscale image
					image = image[0];
					baton.Channels = 1;
				}

				if(image.Format != Enums.BandFormat.Uchar) {
					// Cast pixels to uint8 (unsigned char)
					image = image.Cast(Enums.BandFormat.Uchar);
				}

				baton.OutputImageInfo.Format = "raw";
				// Get raw image data
				bo.Buffer = image.WriteToBuffer(baton.OutputImageInfo.Format);
			}
		}

		private static void PotentiallySaveHeifBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputImageInfo.Format;
			var ho = baton.HeifOptions;
			var bo = baton.ToBufferOptions;
			if(ho == null || bo == null || (formatOut != "heif" && (formatOut != "input" || imageType != ImageType.Heif))) {
				return;
			}

			bo.Buffer = image.HeifsaveBuffer(
				q: ho.Quality,
				lossless: ho.UseLossless,
				compression: ho.Compression,
				speed: ho.Speed,
				strip: baton.MetadataOptions.ShouldStripMetadata,
				background: null,
				pageHeight: null
			);

			baton.OutputImageInfo.Format = "heif";
		}

		private static void PotentiallySaveTiffBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputImageInfo.Format;
			var to = baton.TiffOptions;
			var bo = baton.ToBufferOptions;
			if(to == null || bo == null || (formatOut != "tiff" && (formatOut != "input" || imageType != ImageType.Tiff))) {
				return;
			}

			if(to.Compression == Enums.ForeignTiffCompression.Jpeg) {
				image.AssertImageTypeDimensions(ImageType.Jpeg);
				baton.Channels = Math.Min(baton.Channels, 3);
			}

			// Cast pixel values to float, if required
			if(to.Predictor == Enums.ForeignTiffPredictor.Float) {
				image = image.Cast(Enums.BandFormat.Float);
			}

			bo.Buffer = image.TiffsaveBuffer(
				compression: to.Compression,
				q: to.Quality,
				predictor: to.Predictor,
				profile: null,
				tile: to.Tile,
				tileWidth: to.TileWidth,
				tileHeight: to.TileHeight,
				pyramid:to.Pyramid,
				miniswhite: null,
				bitdepth: to.BitDepth,
				resunit: null,
				xres: to.XRes,
				yres: to.YRes,
				bigtiff: null,
				properties: null,
				regionShrink: null,
				level: null,
				subifd: null,
				lossless: null,
				depth: null,
				strip: baton.MetadataOptions.ShouldStripMetadata,
				background: null,
				pageHeight: null
			);

			baton.OutputImageInfo.Format = "tiff";
		}

		private static Image SetAnimationPropertiesIfAny(PipelineBaton baton, Image image) {
			var ao = baton.AnimationOptions;
			image = image.SetAnimationProperties(ao.PageHeight, ao.Delay, ao.Loop);
			return image;
		}

		private static void PotentiallySaveGifBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputImageInfo.Format;
			var go = baton.GifOptions;
			var bo = baton.ToBufferOptions;
			if(go == null || bo == null || (formatOut != "gif" && (formatOut != "input" || imageType != ImageType.Gif || !SupportsGifOutput))) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Gif);

			bo.Buffer = image.MagicksaveBuffer(
				format:"gif",
				quality:null,
				optimizeGifFrames:go.OptimizeFrames,
				optimizeGifTransparency:go.OptimizeTransparency,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputImageInfo.Format = "gif";
		}

		private static void PotentiallySaveWebpBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputImageInfo.Format;
			var wo = baton.WebpOptions;
			var bo = baton.ToBufferOptions;
			if(bo == null || wo == null || (formatOut != "webp" && (formatOut != "input" || imageType != ImageType.WebP))) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.WebP);

			bo.Buffer = image.WebpsaveBuffer(
				q: wo.Quality,
				lossless: wo.UseLossless,
				preset: null,
				smartSubsample: wo.UseSmartSubsample,
				nearLossless: wo.UseNearLossless,
				alphaQ: wo.AlphaQuality,
				minSize: null,
				kmin: null,
				kmax: null,
				reductionEffort: wo.ReductionEffort,
				profile: null,
				strip: baton.MetadataOptions.ShouldStripMetadata,
				background: null,
				pageHeight: null
			);

			baton.OutputImageInfo.Format = "webp";
		}

		private static void PotentiallySavePngBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputImageInfo.Format;
			var po = baton.PngOptions;
			var bo = baton.ToBufferOptions;
			if(po == null || bo == null || (formatOut != "png" && (formatOut != "input" || (imageType != ImageType.Png && imageType != ImageType.Gif && imageType != ImageType.Svg)))) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Png);

			bo.Buffer = image.PngsaveBuffer(
				po.CompressionLevel, 
				po.MakeProgressive,
				null,
				po.UseAdaptiveFiltering ? 5 : 0,  // TODO: Send netvips the enum for png https://libvips.github.io/libvips/API/current/VipsForeignSave.html#VipsForeignPngFilter https://www.w3.org/TR/PNG-Filters.html
				po.UsePalette,
				po.Quality,
				po.Dither,
				null,
				baton.MetadataOptions.ShouldStripMetadata
			);

			baton.OutputImageInfo.Format = "png";
		}

		private static void PotentiallySaveJpegBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputImageInfo.Format;
			var jo = baton.JpegOptions;
			var bo = baton.ToBufferOptions;
			if(bo == null || jo == null || formatOut != "jpeg" && (formatOut != "input" || imageType != ImageType.Jpeg)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Jpeg);

			bo.Buffer = image.JpegsaveBuffer(
				jo.Quality,
				optimizeCoding:jo.OptimizeCoding,
				interlace:jo.MakeProgressive,
				subsampleMode:jo.Subsampling == "4:4:4" ? Enums.ForeignJpegSubsample.Off : Enums.ForeignJpegSubsample.On,
				trellisQuant:jo.ApplyTrellisQuantization,
				overshootDeringing:jo.ApplyOvershootDeringing,
				optimizeScans:jo.OptimizeScans,
				quantTable:jo.QuantizationTable,
				strip:baton.MetadataOptions.ShouldStripMetadata
			);

			baton.Channels = Math.Min(baton.Channels, baton.OperationOptions.ColorSpace == Enums.Interpretation.Cmyk ? 4 : 3);
			baton.OutputImageInfo.Format = "jpeg";
		}

		private static Image OverrideExifOrientation(PipelineBaton baton, Image image) {
			// Override EXIF Orientation tag
			var mo = baton.MetadataOptions;
			if(mo.WithMetadata && mo.Orientation != -1) {
				image = image.SetExifOrientation(mo.Orientation);
			}
			
			return image;
		}

		private static Image ApplyIccProfile(PipelineBaton baton, Image image) {
			// Apply output ICC profile
			var icc = baton.MetadataOptions.Icc;
			if(icc.HasValue()) {
				image = image.IccTransform(icc, null, Enums.Intent.Perceptual, null, "srgb");
			}

			return image;
		}

		private static Image ConvertToSrgb(PipelineBaton baton, Image image) {
			// Convert image to sRGB, if not already
			if(image.Interpretation.Is16Bit()) {
				image = image.Cast(Enums.BandFormat.Ushort);
			}

			var colorSpace = baton.OperationOptions.ColorSpace;
			if(image.Interpretation == colorSpace) {
				return image;
			}

			// Convert colorspace, pass the current known interpretation so libvips doesn't have to guess
			image = image.Colourspace(colorSpace, image.Interpretation);
			// Transform colors from embedded profile to output profile
			if(baton.MetadataOptions.WithMetadata && image.HasProfile()) {
				image = image.IccTransform(colorSpace, embedded:true);
			}

			return image;
		}

		private static Image EnsureAlphaChannelIfMissing(PipelineBaton baton, Image image) {
			// Ensure alpha channel, if missing
			if(baton.OperationOptions.EnsureAlpha) {
				image = image.EnsureAlpha();
			}

			return image;
		}

		private static Image RemoveAlphaChannelIfAny(PipelineBaton baton, Image image) {
			// Remove alpha channel, if any
			if(baton.OperationOptions.RemoveAlpha) {
				image = image.RemoveAlpha();
			}

			return image;
		}

		private static Image ExtractVipsBand(PipelineBaton baton, Image image) {
			var ec = baton.OperationOptions.ExtractChannel;
			if(ec > -1) {
				if(ec >= image.Bands) {
					if(ec == 3 && image.HasAlpha()) {
						baton.OperationOptions.ExtractChannel = image.Bands - 1;
					}
					else {
						throw new Exception("Cannot extract channel from image. Too few channels in image.");
					}
				}

				var interpretation = image.Interpretation.Is16Bit() ?
					Enums.Interpretation.Grey16 : Enums.Interpretation.Bw;
				image = image
					.ExtractBand(baton.OperationOptions.ExtractChannel);
				image.Set("interpretation", interpretation);
			}

			return image;
		}

		private static Image Tint(PipelineBaton baton, Image image) {
			// Tint the image
			var oo = baton.OperationOptions;
			if(oo.TintA < 128.0 || oo.TintB < 128.0) {
				image = image.Tint(oo.TintA, oo.TintB);
			}

			return image;
		}

		private static Image ApplyBandBoolOp(PipelineBaton baton, Image image) {
			// Apply per-channel Bandbool bitwise operations after all other operations
			var bandBoolOp = baton.OperationOptions.BandBoolOp;
			if(bandBoolOp.HasValue()) {
				image = image.Bandbool(bandBoolOp);
			}

			return image;
		}

		private static Image Normalize(PipelineBaton baton, Image image) {
			if(baton.OperationOptions.Normalize) {
				image = image.Normalize();
			}

			return image;
		}

		private static Image LinearAdjustment(PipelineBaton baton, Image image) {
			// Linear adjustment (a * in + b)
			var oo = baton.OperationOptions;
			if(!oo.LinearA.IsAboutEqualTo(1.0) || !oo.LinearB.IsAboutEqualTo(0.0)) {
				image = image.Linear(oo.LinearA, oo.LinearB);
			}

			return image;
		}

		private static Image ApplyGammaDecoding(PipelineBaton baton, Image image) {
			var gammaOut = baton.OperationOptions.GammaOut;
			if(gammaOut >= 1 && gammaOut <= 3) {
				image = image.Gamma(gammaOut);
			}

			return image;
		}

		private static Image ReversePremultiplication(PipelineBaton baton, Image image, bool shouldPremultiplyAlpha) {
			if(shouldPremultiplyAlpha) {
				image = image.Unpremultiply();
				// Cast pixel values to integer
				if(NeedsBetterPlace.Is16Bit(image.Interpretation)) {
					image = image.Cast(Enums.BandFormat.Ushort);
				}
				else {
					image = image.Cast(Enums.BandFormat.Uchar);
				}
			}

			baton.OperationOptions.Premultiplied = shouldPremultiplyAlpha;
			return image;
		}

		private static Image Sharpen(PipelineBaton baton, Image image) {
			var oo = baton.OperationOptions;
			var shouldSharpen = !oo.SharpenSigma.IsAboutEqualTo(0.0);
			if(shouldSharpen) {
				image = ImageExtensions.Sharpen(image, oo.SharpenSigma, oo.SharpenFlat, oo.SharpenJagged);
			}

			return image;
		}

		private static Image Modulate(PipelineBaton baton, Image image) {
			var oo = baton.OperationOptions;
			var shouldModulate = !oo.Brightness.IsAboutEqualTo(1.0) || !oo.Saturation.IsAboutEqualTo(1.0) || oo.Hue != 0;
			if(shouldModulate) {
				image = image.Modulate(oo.Brightness, oo.Saturation, oo.Hue);
			}

			return image;
		}

		private static Image Recomb(PipelineBaton baton, Image image) {
			var oo = baton.OperationOptions;
			if(oo.RecombMatrix.HasValue()) {
				image = image.Recomb(oo.RecombMatrix);
			}

			return image;
		}

		private static Image Convolve(PipelineBaton baton, Image image) {
			var oo = baton.OperationOptions;
			var shouldConvolve = oo.ConvolveKernelWidth * oo.ConvolveKernelHeight > 0;
			if(shouldConvolve) {
				image = image.Convolve(oo.ConvolveKernelWidth, oo.ConvolveKernelHeight, oo.ConvolveKernelScale, oo.ConvolveKernelOffset, oo.ConvolveKernel);
			}

			return image;
		}

		private static Image Blur(PipelineBaton baton, Image image) {
			var sigma = baton.OperationOptions.BlurSigma;
			var shouldBlur = !sigma.IsAboutEqualTo(0.0);
			if(shouldBlur) {
				image = image.Blur(sigma);
			}

			return image;
		}

		private static Image Threshold(PipelineBaton baton, Image image) {
			// Threshold - must happen before blurring, due to the utility of blurring after thresholding
			var oo = baton.OperationOptions;
			if(oo.Threshold != 0) {
				image = image.Threshold(oo.Threshold, oo.ThresholdGrayscale);
			}

			return image;
		}

		private static Image Median(PipelineBaton baton, Image image) {
			// Median - must happen before blurring, due to the utility of blurring after thresholding
			var oo = baton.OperationOptions;
			var shouldApplyMedian = oo.MedianSize > 0;
			if(shouldApplyMedian) {
				image = image.Median(oo.MedianSize);
			}

			return image;
		}

		private static Image ExtendEdges(PipelineBaton baton, Image image) {
			var ro = baton.ResizeOptions;
			if(ro.ExtendTop > 0 || ro.ExtendBottom > 0 || ro.ExtendLeft > 0 || ro.ExtendRight > 0) {
				var (img, bg) = NeedsBetterPlace.ApplyAlpha(image, baton.ResizeOptions.ExtendBackground);
				// Embed
				baton.Width = img.Width + ro.ExtendLeft + ro.ExtendRight;
				baton.Height = img.Height + ro.ExtendTop + ro.ExtendBottom;
				image = img.Embed(ro.ExtendLeft, ro.ExtendTop, baton.Width, baton.Height, Enums.Extend.Background, bg);
			}

			return image;
		}

		private static Image AffineTransform(PipelineBaton baton, Image image) {
			var ro = baton.ResizeOptions;
			if(ro.AffineMatrix.HasValue()) {
				var (img, bg) = NeedsBetterPlace.ApplyAlpha(image, ro.AffineBackground);
				image = img.Affine(ro.AffineMatrix, ro.AffineInterpolator, null, ro.AffineOdx, ro.AffineOdy, ro.AffineIdx, ro.AffineIdy, bg);
			}

			return image;
		}

		private static Image PostExtraction(PipelineBaton baton, Image image) {
			var resizeOptions = baton.ResizeOptions;
			if(resizeOptions.TopOffsetPost != -1) {
				image = image.ExtractArea(resizeOptions.LeftOffsetPost, resizeOptions.TopOffsetPost, resizeOptions.WidthPost, resizeOptions.HeightPost);
			}

			return image;
		}

		private static Image RotatePostExtract(PipelineBaton baton, Image image) {
			if(!baton.RotationOptions.RotateBeforePreExtract && !baton.RotationOptions.RotationAngle.IsAboutEqualTo(0.0)) {
				var (img, background) = NeedsBetterPlace.ApplyAlpha(image, baton.RotationOptions.RotationBackground);
				image = img.Rotate(baton.RotationOptions.RotationAngle, background:background);
			}

			return image;
		}

		private Image JoinAdditionalColorChannels(PipelineBaton baton, Image image) {
			// TODO: this
			//if(baton.OperationOptions.JoinChannelIn.HasValue()) {
			//	for(var i = 0; i < baton.OperationOptions.JoinChannelIn.Length; i++) {
			//		var (joinImage, joinImageType) = sharp::OpenInput(baton->joinChannelIn[i]);
			//		image = image.bandjoin(joinImage);
			//	}
			//	image = image.copy(VImage::option()->set("interpretation", baton->colourspace));
			//}
			return image;
		}

		private static Image CropOrEmbed(PipelineBaton baton, Image image) {
			if(image.Width == baton.Width && image.Height == baton.Height) {
				return image;
			}

			if(baton.ResizeOptions.Canvas == Canvas.Embed) {
				image = Embed(baton, image);
			}
			else if(baton.ResizeOptions.Canvas != Canvas.IgnoreAspectRatio && (image.Width > baton.Width || image.Height > baton.Height)) {
				image = (int) baton.ResizeOptions.Position < 9 ? GravityCrop(baton, image) : AttentionOrEntropyCrop(baton, image);
			}

			return image;
		}

		private static Image AttentionOrEntropyCrop(PipelineBaton baton, Image image) {
			if(baton.Width > image.Width) {
				baton.Width = image.Width;
			}

			if(baton.Height > image.Height) {
				baton.Height = image.Height;
			}

			image = image.Tilecache(access:Enums.Access.Random, threaded:true);
			image = image.Smartcrop(baton.Width, baton.Height, (int) baton.ResizeOptions.Position == 16 ? Enums.Interesting.Entropy : Enums.Interesting.Attention);
			baton.CropOffsetOptions.HasCropOffset = true;
			baton.CropOffsetOptions.Left = image.Xoffset;
			baton.CropOffsetOptions.Top = image.Yoffset;
			return image;
		}

		private static Image GravityCrop(PipelineBaton baton, Image image) {
			var (left, top) = NeedsBetterPlace.CalculateCrop(image.Width, image.Height, baton.Width, baton.Height, baton.ResizeOptions.Position);
			var width = Math.Min(image.Width, baton.Width);
			var height = Math.Min(image.Height, baton.Height);
			image = image.ExtractArea(left, top, width, height);
			return image;
		}

		private static Image Embed(PipelineBaton baton, Image image) {
			var (img, background) = NeedsBetterPlace.ApplyAlpha(image, baton.ResizeOptions.Background);
			image = img;
			// Calculate where to position the embedded image if gravity specified, else center.
			var width = Math.Max(image.Width, baton.Width);
			var height = Math.Max(image.Height, baton.Height);
			var (left, top) = NeedsBetterPlace.CalculateEmbedPosition(image.Width, image.Height, baton.Width, baton.Height, baton.ResizeOptions.Position);
			image = image.Embed(left, top, width, height, "background", background);
			return image;
		}

		private static Image Flop(PipelineBaton baton, Image image) {
			if(baton.RotationOptions.Flop) {
				image = image.Flip(Enums.Direction.Horizontal);
				image = image.RemoveExifOrientation();
			}

			return image;
		}

		private static Image Flip(PipelineBaton baton, Image image) {
			if(baton.RotationOptions.Flip) {
				image = image.Flip(Enums.Direction.Vertical);
				image = image.RemoveExifOrientation();
			}

			return image;
		}

		private static Image RotatePostExtract(PipelineBaton baton, Image image, string rotation) {
			if(!baton.RotationOptions.RotateBeforePreExtract && rotation != Enums.Angle.D0) {
				image = image.Rot(rotation);
				image = image.RemoveExifOrientation();
			}

			return image;
		}

		private static Image Resize(PipelineBaton baton, Image image, bool shouldResize, double xFactor, double yFactor) {
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

			return image;
		}

		private static Image PremultiplyAlphaChannel(Image image, bool shouldPremultiplyAlpha) {
			// Premultiply image alpha channel before all transformations to avoid
			// dark fringing around bright pixels
			// See: http://entropymine.com/imageworsener/resizealpha/
			if(shouldPremultiplyAlpha) {
				image = image.Premultiply();
			}

			return image;
		}

		private static Image CreateAlphaChannel(Image image, bool shouldComposite) {
			if(shouldComposite && !image.HasAlpha()) {
				image = image.EnsureAlpha();
			}

			return image;
		}

		private static Image ConvertToGrayscale(PipelineBaton baton, Image image) {
			if(baton.OperationOptions.Grayscale) {
				image = image.Colourspace(Enums.Interpretation.Bw);
			}

			return image;
		}

		private static Image ApplyGammaEncoding(PipelineBaton baton, Image image) {
			var gamma = baton.OperationOptions.Gamma;
			if(gamma >= 1 && gamma <= 3) {
				image = image.Gamma(1.0 / gamma);
			}

			return image;
		}

		private static Image NegateColors(PipelineBaton baton, Image image) {
			if(baton.OperationOptions.Negate) {
				image = image.Invert();
			}

			return image;
		}

		private static Image RemoveAlphaChannel(PipelineBaton baton, Image image) {
			if(baton.OperationOptions.Flatten && image.HasAlpha()) {
				// Scale up 8-bit values to match 16-bit input image
				var multiplier = image.Interpretation.Is16Bit() ? 256.0 : 1.0;
				// Background color
				var fb = baton.OperationOptions.FlattenBackground;
				var background = new[] {fb[0] * multiplier, fb[1] * multiplier, fb[2] * multiplier};
				image = image.Flatten(background);
			}

			return image;
		}

		private static Image EnsureDeviceIndependentColorSpace(Image image) {
			var imageInterpretation = image.Interpretation;
			if(image.HasProfile() && imageInterpretation != Enums.Interpretation.Labs && imageInterpretation != Enums.Interpretation.Grey16) {
				// Convert to sRGB using embedded profile
				try {
					image = image.IccTransform("srgb", null, Enums.Intent.Perceptual, true, depth:imageInterpretation == Enums.Interpretation.Rgb16 ? 16 : 8);
				}
				catch {
					// Ignore failure of embedded profile
				}
			}
			else if(imageInterpretation == Enums.Interpretation.Cmyk) {
				image = image.IccTransform("srgb", null, Enums.Intent.Perceptual, null, "cmyk");
			}

			return image;
		}

		private static Image ShrinkOnLoad(PipelineBaton baton, Image image, ImageType imageType, ImageSource imageSource, ref double xFactor, ref double yFactor, int xShrink, int yShrink, double xResidual, double yResidual, string rotation, int targetResizeWidth, int targetResizeHeight) {
			var shrinkOnLoad = 1;
			var shrinkOnLoadFactor = 1;

			// Leave at least a factor of two for the final resize step, when fastShrinkOnLoad: false
			// for more consistent results and avoid occasional small image shifting
			if(!baton.ResizeOptions.FastShrinkOnLoad) {
				shrinkOnLoadFactor = 2;
			}

			// If integral x and y shrink are equal, try to use shrink-on-load for JPEG and WebP,
			// but not when applying gamma correction, pre-resize extract or trim
			if(xShrink == yShrink && xShrink >= 2 * shrinkOnLoadFactor && (imageType == ImageType.Jpeg || imageType == ImageType.WebP)
				&& baton.OperationOptions.Gamma == 0 && baton.ResizeOptions.TopOffsetPre == -1 && baton.TrimOptions.TrimThreshold.IsAboutEqualTo(0.0)) {
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
				else if(imageSource is FileImageSource fileImageSource) {
					if(imageType == ImageType.Jpeg) {
						image = Image.Jpegload(fileImageSource.Path, shrinkOnLoad, null, null, access, true);
					}
					else if(imageType == ImageType.WebP) {
						image = Image.Webpload(fileImageSource.Path, imageSource.Options.PageIndex, imageSource.Options.PageCount, null, null, access, true);
					}
				}

				// Recalculate integral shrink and double residual
				var shrunkOnLoadWidth = image.Width;
				var shrunkOnLoadHeight = image.Height;

				if (!baton.ResizeOptions.RotateBeforePreExtract && (rotation == Enums.Angle.D90 || rotation == Enums.Angle.D270)) {
					// Swap when rotating by 90 or 270 degrees
					xFactor = shrunkOnLoadWidth / (double) targetResizeHeight;
					yFactor = shrunkOnLoadHeight / (double) targetResizeWidth;
				} else {
					xFactor = shrunkOnLoadWidth / (double) targetResizeWidth;
					yFactor = shrunkOnLoadHeight / (double) targetResizeHeight;
				}
			}

			return image;
		}

		private static (double xResidual, double yResidual) CalculateResidualFloatAffineTransformation(int xShrink, double xFactor, int yShrink, double yFactor) 
			=> (xShrink / xFactor, yShrink / yFactor);

		private static (int XShrink, int YShrink) CalculateIntegralBoxShrink(double xFactor, double yFactor) =>
			(Math.Max(1, (int) Math.Floor(xFactor)), Math.Max(1, (int) Math.Floor(yFactor)));

		private static (double XFactor, double YFactor, int TargetResizeWidth, int TargetResizeHeight) CalculateScaling(PipelineBaton baton, int inputWidth, int inputHeight) {
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
				switch(baton.ResizeOptions.Canvas) {
					case Canvas.Crop:
						if(xFactor < yFactor) {
							targetResizeHeight = (int) Math.Round(inputHeight / xFactor, MidpointRounding.AwayFromZero);
							yFactor = xFactor;
						}
						else {
							targetResizeWidth = (int) Math.Round(inputWidth / yFactor, MidpointRounding.AwayFromZero);
							xFactor = yFactor;
						}

						break;
					case Canvas.Embed:
						if(xFactor > yFactor) {
							targetResizeHeight = (int) Math.Round(inputHeight / xFactor, MidpointRounding.AwayFromZero);
							yFactor = xFactor;
						}
						else {
							targetResizeWidth = (int) Math.Round(inputWidth / yFactor, MidpointRounding.AwayFromZero);
							xFactor = yFactor;
						}

						break;
					case Canvas.Max:
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
					case Canvas.Min:
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
					case Canvas.IgnoreAspectRatio:
						// TODO: rotation stuff
						break;
				}
			}
			else if(baton.Width > 0) {
				// Fixed width
				xFactor = inputWidth / (double) baton.Width;
				if(baton.ResizeOptions.Canvas == Canvas.IgnoreAspectRatio) {
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
				if(baton.ResizeOptions.Canvas == Canvas.IgnoreAspectRatio) {
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

			return (xFactor, yFactor, targetResizeWidth, targetResizeHeight);
		}

		private static (string Rotation, RotationOptions rotationOptions) CalculateAngleOfRotation(PipelineBaton baton, Image image) {
			var rotationOptions = baton.RotationOptions;
			string rotation;
			if(rotationOptions.UseExifOrientation) {
				// Rotate and flip image according to Exif orientation
				var (rotationString, flip, flop) = CalculateExifRotationAndFlip(image.ExifOrientation());
				rotation = rotationString;
				rotationOptions.Flip = rotationOptions.Flip || flip;
				rotationOptions.Flop = rotationOptions.Flop || flop;
			}
			else {
				rotation = CalculateAngleRotation(rotationOptions.Angle);
			}

			return (rotation, rotationOptions);
		}

		private static string CalculateAngleRotation(int angle) {
			angle %= 360;

			if(angle < 0) {
				angle = 360 + angle;
			}

			return angle switch {
				90 => Enums.Angle.D90,
				180 => Enums.Angle.D180,
				270 => Enums.Angle.D270,
				_ => Enums.Angle.D0
			};
		}

		private static (string Rotation, bool Flip, bool Flop) CalculateExifRotationAndFlip(int exifOrientation) {
			var rotation = Enums.Angle.D0;
			var flip = false;
			var flop = false;
			switch(exifOrientation) {
				case 6: {
					rotation = Enums.Angle.D90;
					break;
				}
				case 3:
					rotation = Enums.Angle.D180;
					break;
				case 8:
					rotation = Enums.Angle.D270;
					break;
				case 2:
					flop = true;
					break; // flop 1
				case 7:
					flip = true;
					rotation = Enums.Angle.D90;
					break; // flip 6
				case 4:
					flop = true;
					rotation = Enums.Angle.D180;
					break; // flop 3
				case 5:
					flip = true;
					rotation = Enums.Angle.D270;
					break; // flip 8
			}

			return (rotation, flip, flop);
		}

		private static (int IntputWidth, int InputHeight) GetPreResizeWidthAndHeight(PipelineBaton baton, Image image, string rotation) {
			var inputWidth = image.Width;
			var inputHeight = image.Height;

			if(baton.RotationOptions.RotateBeforePreExtract || rotation != Enums.Angle.D90 && rotation != Enums.Angle.D270) {
				return (inputWidth, inputHeight);
			}

			// Swap input output width and height when rotating by 90 or 270 degrees
			var temp = inputWidth;
			inputWidth = inputHeight;
			inputHeight = temp;
			return (inputWidth, inputHeight);
		}

		private static void ClipWithoutEnlargement(PipelineBaton baton, int inputWidth, int inputHeight) {
			var resizeOptions = baton.ResizeOptions;
			if(!resizeOptions.WithoutEnlargement) {
				return;
			}

			// Override target width and height if exceeds respective value from input file

			if(resizeOptions.Width > inputWidth) {
				resizeOptions.Width = inputWidth;
			}

			if(resizeOptions.Height > inputHeight) {
				resizeOptions.Height = inputHeight;
			}
		}

		private static (Image Image, ImageType ImageType) LoadImage(ImageSource imageSource, PipelineBaton baton) {
			var (image, imageType) = imageSource.Load();
			baton.Image = image;
			baton.InputImageType = imageType;
			return (image, imageType);
		}

		private static Image PreExtraction(PipelineBaton baton, Image image) {
			var ro = baton.ResizeOptions;
			if(ro.TopOffsetPre != -1) {
				image = image.ExtractArea(ro.LeftOffsetPre, ro.TopOffsetPre, ro.Width, ro.Height);
			}

			return image;
		}

		private static Image RotatePreExtract(Image image, string rotation, RotationOptions rotationOptions) {
			if(rotationOptions.RotateBeforePreExtract) {
				if(rotation != Enums.Angle.D0) {
					image = image.Rot(rotation);
					image = image.RemoveExifOrientation();
				}

				if(rotationOptions.RotationAngle != 0.0) {
					var (img, background) = NeedsBetterPlace.ApplyAlpha(image, rotationOptions.RotationBackground);
					image = img;
					image = image.Rotate(rotationOptions.RotationAngle, background:background);
				}
			}

			return image;
		}

		private static Image Trim(PipelineBaton baton, Image image) {
			var trimOptions = baton.TrimOptions;
			if(trimOptions.TrimThreshold > 0.0) {
				image = image.Trim(trimOptions.TrimThreshold);
				trimOptions.TrimOffsetLeft = image.Xoffset;
				trimOptions.TrimOffsetTop = image.Yoffset;
			}

			return image;
		}
	}
}
