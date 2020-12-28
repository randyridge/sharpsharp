using System;
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
			var shouldPremultiplyAlpha = image.HasAlpha() && (shouldResize || shouldBlur || shouldConvolve || shouldSharpen || shouldComposite);
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
				// TODO: Handle unknown
				baton.OutputInfo.Size = baton.ToBufferOptions.Buffer.Length;
			}

			if(baton.ToFileOptions.HasValue()) {
				PotentiallySaveJpegFile(baton, image, imageType);
				PotentiallySavePngFile(baton, image, imageType);
				PotentiallySaveWebpFile(baton, image, imageType);
				PotentiallySaveGifFile(baton, image, imageType);
				PotentiallySaveTiffFile(baton, image, imageType);
				PotentiallySaveHeifFile(baton, image, imageType);
				// TODO: PotentiallySaveDzFile(baton, image, imageType);
				PotentiallySaveVFile(baton, image, imageType);
				// TODO: Handle unknown
				baton.OutputInfo.Size = (int) new FileInfo(baton.ToFileOptions.FilePath).Length; // TODO: this seems bad
			}

			if(baton.ToStreamOptions.HasValue()) {
				PotentiallySaveJpegStream(baton, image, imageType);
				PotentiallySavePngStream(baton, image, imageType);
				PotentiallySaveWebpStream(baton, image, imageType);
				PotentiallySaveGifStream(baton, image, imageType);
				PotentiallySaveTiffStream(baton, image, imageType);
				PotentiallySaveHeifStream(baton, image, imageType);
				PotentiallySaveRawStream(baton, image, imageType);
				// TODO: Handle unknown
				baton.OutputInfo.Size = (int) baton.ToStreamOptions.Stream.Length; // TODO: this seems bad
			}

			image.Dispose();
		}

		private static Image AffineTransform(PipelineBaton baton, Image image) {
			var ro = baton.ResizeOptions;
			if(!ro.AffineMatrix.HasValue()) {
				return image;
			}

			var (img, bg) = Common.ApplyAlpha(image, ro.AffineBackground);
			image = img.Affine(ro.AffineMatrix, ro.AffineInterpolator, null, ro.AffineOdx, ro.AffineOdy, ro.AffineIdx, ro.AffineIdy, bg);

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

		private static Image ApplyGammaDecoding(PipelineBaton baton, Image image) {
			var gammaOut = baton.OperationOptions.GammaOut;
			if(gammaOut >= 1 && gammaOut <= 3) {
				image = image.Gamma(gammaOut);
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

		private static Image ApplyIccProfile(PipelineBaton baton, Image image) {
			// Apply output ICC profile
			var icc = baton.MetadataOptions.Icc;
			if(icc.HasValue()) {
				image = image.IccTransform(icc, null, Enums.Intent.Perceptual, null, "srgb");
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

		private static Image Blur(PipelineBaton baton, Image image) {
			var sigma = baton.OperationOptions.BlurSigma;
			var shouldBlur = !sigma.IsAboutEqualTo(0.0);
			if(shouldBlur) {
				image = image.Blur(sigma);
			}

			return image;
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

		private static (int XShrink, int YShrink) CalculateIntegralBoxShrink(double xFactor, double yFactor) =>
			(Math.Max(1, (int) Math.Floor(xFactor)), Math.Max(1, (int) Math.Floor(yFactor)));

		private static (double xResidual, double yResidual) CalculateResidualFloatAffineTransformation(int xShrink, double xFactor, int yShrink, double yFactor)
			=> (xShrink / xFactor, yShrink / yFactor);

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

		private static Image ConvertToGrayscale(PipelineBaton baton, Image image) {
			if(baton.OperationOptions.Grayscale) {
				image = image.Colourspace(Enums.Interpretation.Bw);
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

		private static Image Convolve(PipelineBaton baton, Image image) {
			var oo = baton.OperationOptions;
			var shouldConvolve = oo.ConvolveKernelWidth * oo.ConvolveKernelHeight > 0;
			if(shouldConvolve) {
				image = image.Convolve(oo.ConvolveKernelWidth, oo.ConvolveKernelHeight, oo.ConvolveKernelScale, oo.ConvolveKernelOffset, oo.ConvolveKernel);
			}

			return image;
		}

		private static Image CreateAlphaChannel(Image image, bool shouldComposite) {
			if(shouldComposite && !image.HasAlpha()) {
				image = image.EnsureAlpha();
			}

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

		private static Image Embed(PipelineBaton baton, Image image) {
			var (img, background) = Common.ApplyAlpha(image, baton.ResizeOptions.Background);
			image = img;
			// Calculate where to position the embedded image if gravity specified, else center.
			var width = Math.Max(image.Width, baton.Width);
			var height = Math.Max(image.Height, baton.Height);
			var (left, top) = Common.CalculateEmbedPosition(image.Width, image.Height, baton.Width, baton.Height, baton.ResizeOptions.Position);
			image = image.Embed(left, top, width, height, "background", background);
			return image;
		}

		private static Image EnsureAlphaChannelIfMissing(PipelineBaton baton, Image image) {
			// Ensure alpha channel, if missing
			if(baton.OperationOptions.EnsureAlpha) {
				image = image.EnsureAlpha();
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

		private static Image ExtendEdges(PipelineBaton baton, Image image) {
			var ro = baton.ResizeOptions;
			if(ro.ExtendTop > 0 || ro.ExtendBottom > 0 || ro.ExtendLeft > 0 || ro.ExtendRight > 0) {
				var (img, bg) = Common.ApplyAlpha(image, baton.ResizeOptions.ExtendBackground);
				// Embed
				baton.Width = img.Width + ro.ExtendLeft + ro.ExtendRight;
				baton.Height = img.Height + ro.ExtendTop + ro.ExtendBottom;
				image = img.Embed(ro.ExtendLeft, ro.ExtendTop, baton.Width, baton.Height, Enums.Extend.Background, bg);
			}

			return image;
		}

		private static Image ExtractVipsBand(PipelineBaton baton, Image image) {
			var ec = baton.OperationOptions.ExtractChannel;
			if(ec <= -1) {
				return image;
			}

			if(ec >= image.Bands) {
				if(ec == 3 && image.HasAlpha()) {
					baton.OperationOptions.ExtractChannel = image.Bands - 1;
				}
				else {
					throw new SharpSharpException("Cannot extract channel from image. Too few channels in image.");
				}
			}

			var interpretation = image.Interpretation.Is16Bit() ?
				Enums.Interpretation.Grey16 : Enums.Interpretation.Bw;
			image = image
				.ExtractBand(baton.OperationOptions.ExtractChannel);
			image.Set("interpretation", interpretation);

			return image;
		}

		private static Image Flip(PipelineBaton baton, Image image) {
			if(baton.RotationOptions.Flip) {
				image = image.Flip(Enums.Direction.Vertical);
				image = image.RemoveExifOrientation();
			}

			return image;
		}

		private static Image Flop(PipelineBaton baton, Image image) {
			if(baton.RotationOptions.Flop) {
				image = image.Flip(Enums.Direction.Horizontal);
				image = image.RemoveExifOrientation();
			}

			return image;
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

		private static Image GravityCrop(PipelineBaton baton, Image image) {
			var (left, top) = Common.CalculateCrop(image.Width, image.Height, baton.Width, baton.Height, baton.ResizeOptions.Position);
			var width = Math.Min(image.Width, baton.Width);
			var height = Math.Min(image.Height, baton.Height);
			image = image.ExtractArea(left, top, width, height);
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

		private static (Image Image, ImageType ImageType) LoadImage(ImageSource imageSource, PipelineBaton baton) {
			var (image, imageType) = imageSource.Load();
			baton.Image = image;
			baton.InputImageType = imageType;
			return (image, imageType);
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

		private static bool MightMatchInput(string path) => path == "input";

		private static Image Modulate(PipelineBaton baton, Image image) {
			var oo = baton.OperationOptions;
			var shouldModulate = !oo.Brightness.IsAboutEqualTo(1.0) || !oo.Saturation.IsAboutEqualTo(1.0) || oo.Hue != 0;
			if(shouldModulate) {
				image = image.Modulate(oo.Brightness, oo.Saturation, oo.Hue);
			}

			return image;
		}

		private static Image NegateColors(PipelineBaton baton, Image image) {
			if(baton.OperationOptions.Negate) {
				image = image.Invert();
			}

			return image;
		}

		private static Image Normalize(PipelineBaton baton, Image image) {
			if(baton.OperationOptions.Normalize) {
				image = image.Normalize();
			}

			return image;
		}

		private static Image OverrideExifOrientation(PipelineBaton baton, Image image) {
			// Override EXIF Orientation tag
			var mo = baton.MetadataOptions;
			if(mo.WithMetadata && mo.Orientation != -1) {
				image = image.SetExifOrientation(mo.Orientation);
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

		private static void PotentiallySaveGifBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var go = baton.GifOptions;
			var bo = baton.ToBufferOptions;
			if(go == null || bo == null || formatOut != "gif" && (formatOut != "input" || imageType != ImageType.Gif || !SupportsGifOutput)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Gif);

			bo.Buffer = image.MagicksaveBuffer(
				format: "gif",
				quality: null,
				optimizeGifFrames: go.OptimizeFrames,
				optimizeGifTransparency: go.OptimizeTransparency,
				strip: baton.MetadataOptions.ShouldStripMetadata,
				background: null,
				pageHeight: null
			);

			baton.OutputInfo.Format = "gif";
		}

		private static void PotentiallySaveGifFile(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var go = baton.GifOptions;
			var fo = baton.ToFileOptions;
			if(go == null || fo == null || (formatOut != "gif" && (!MightMatchInput(fo.FilePath) || !Common.IsGif(fo.FilePath)) && (!WillMatchInput(fo.FilePath) || imageType != ImageType.Gif || !SupportsGifOutput))) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Gif);

			image.Magicksave(
				format:"gif",
				quality:null,
				optimizeGifFrames:go.OptimizeFrames,
				optimizeGifTransparency:go.OptimizeTransparency,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null,
				filename:fo.FilePath
			);

			baton.OutputInfo.Format = "gif";
		}

		private static void PotentiallySaveGifStream(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var go = baton.GifOptions;
			var so = baton.ToStreamOptions;
			if(go == null || so == null || formatOut != "gif" && (formatOut != "input" || imageType != ImageType.Gif || !SupportsGifOutput)) {
				return;
			}

			throw new SharpSharpException("Cannot save GIF to stream, it's unsupported by libvips.");
		}

		private static void PotentiallySaveHeifBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var ho = baton.HeifOptions;
			var bo = baton.ToBufferOptions;
			if(ho == null || bo == null || formatOut != "heif" && (formatOut != "input" || imageType != ImageType.Heif)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Heif);

			bo.Buffer = image.HeifsaveBuffer(
				q: ho.Quality,
				lossless: ho.UseLossless,
				compression: ho.Compression,
				speed: ho.Speed,
				strip: baton.MetadataOptions.ShouldStripMetadata,
				background: null,
				pageHeight: null
			);

			baton.OutputInfo.Format = "heif";
		}

		private static void PotentiallySaveHeifFile(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var ho = baton.HeifOptions;
			var fo = baton.ToFileOptions;
			if(ho == null || fo == null || formatOut != "heif" && (!MightMatchInput(fo.FilePath) || !Common.IsHeif(fo.FilePath)) && (!WillMatchInput(fo.FilePath) || imageType != ImageType.Heif)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Heif);

			image.Heifsave(
				q:ho.Quality,
				lossless:ho.UseLossless,
				compression:ho.Compression,
				speed:ho.Speed,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null,
				filename:fo.FilePath
			);

			baton.OutputInfo.Format = "heif";
		}

		private static void PotentiallySaveHeifStream(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var ho = baton.HeifOptions;
			var so = baton.ToStreamOptions;
			if(ho == null || so == null || formatOut != "heif" && (formatOut != "input" || imageType != ImageType.Heif)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Heif);

			image.HeifsaveStream(
				stream:so.Stream,
				q:ho.Quality,
				lossless:ho.UseLossless,
				compression:ho.Compression,
				speed:ho.Speed,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "heif";
		}

		private static void PotentiallySaveJpegBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var jo = baton.JpegOptions;
			var bo = baton.ToBufferOptions;
			if(bo == null || jo == null || formatOut != "jpeg" && (formatOut != "input" || imageType != ImageType.Jpeg)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Jpeg);

			bo.Buffer = image.JpegsaveBuffer(
				q:jo.Quality,
				optimizeCoding:jo.OptimizeCoding,
				interlace:jo.MakeProgressive,
				subsampleMode:jo.Subsampling == "4:4:4" ? Enums.ForeignJpegSubsample.Off : Enums.ForeignJpegSubsample.On,
				trellisQuant:jo.ApplyTrellisQuantization,
				overshootDeringing:jo.ApplyOvershootDeringing,
				optimizeScans:jo.OptimizeScans,
				quantTable:jo.QuantizationTable,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				profile:null,
				background:null,
				pageHeight:null
			);

			baton.Channels = Math.Min(baton.Channels, baton.OperationOptions.ColorSpace == Enums.Interpretation.Cmyk ? 4 : 3);
			baton.OutputInfo.Format = "jpeg";
		}

		private static void PotentiallySaveJpegFile(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var jo = baton.JpegOptions;
			var fo = baton.ToFileOptions;
			if(jo == null || fo == null || formatOut != "jpeg" && (!MightMatchInput(fo.FilePath) || !Common.IsJpeg(fo.FilePath)) && (!WillMatchInput(fo.FilePath) || imageType != ImageType.Jpeg)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Jpeg);

			image.Jpegsave(
				filename:fo.FilePath,
				q:jo.Quality,
				profile:null,
				subsampleMode:jo.Subsampling == "4:4:4" ? Enums.ForeignJpegSubsample.Off : Enums.ForeignJpegSubsample.On,
				optimizeCoding:jo.OptimizeCoding,
				interlace:jo.MakeProgressive,
				trellisQuant:jo.ApplyTrellisQuantization,
				overshootDeringing:jo.ApplyOvershootDeringing,
				optimizeScans:jo.OptimizeScans,
				quantTable:jo.QuantizationTable,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.Channels = Math.Min(baton.Channels, baton.OperationOptions.ColorSpace == Enums.Interpretation.Cmyk ? 4 : 3);
			baton.OutputInfo.Format = "jpeg";
		}

		private static void PotentiallySaveJpegStream(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var jo = baton.JpegOptions;
			var so = baton.ToStreamOptions;
			if(so == null || jo == null || formatOut != "jpeg" && (formatOut != "input" || imageType != ImageType.Jpeg)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Jpeg);

			image.JpegsaveStream(
				stream: so.Stream,
				q:jo.Quality,
				optimizeCoding:jo.OptimizeCoding,
				interlace:jo.MakeProgressive,
				subsampleMode:jo.Subsampling == "4:4:4" ? Enums.ForeignJpegSubsample.Off : Enums.ForeignJpegSubsample.On,
				trellisQuant:jo.ApplyTrellisQuantization,
				overshootDeringing:jo.ApplyOvershootDeringing,
				optimizeScans:jo.OptimizeScans,
				quantTable:jo.QuantizationTable,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				profile:null,
				background:null,
				pageHeight:null
			);

			baton.Channels = Math.Min(baton.Channels, baton.OperationOptions.ColorSpace == Enums.Interpretation.Cmyk ? 4 : 3);
			baton.OutputInfo.Format = "jpeg";
		}

		private static void PotentiallySavePngBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var po = baton.PngOptions;
			var bo = baton.ToBufferOptions;
			if(po == null || bo == null || formatOut != "png" && (formatOut != "input" || imageType != ImageType.Png && imageType != ImageType.Gif && imageType != ImageType.Svg)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Png);

			image.Set("colours", po.Colors);

			bo.Buffer = image.PngsaveBuffer(
				compression:po.CompressionLevel,
				interlace:po.MakeProgressive,
				profile:null,
				filter:po.UseAdaptiveFiltering ? 0xF8 : 0x08, // TODO: Send netvips the enum for png https://libvips.github.io/libvips/API/current/VipsForeignSave.html#VipsForeignPngFilter https://www.w3.org/TR/PNG-Filters.html
				palette:po.UsePalette,
				q:po.Quality,
				dither:po.Dither,
				bitdepth:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "png";
		}

		private static void PotentiallySavePngFile(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var po = baton.PngOptions;
			var fo = baton.ToFileOptions;
			if(po == null || fo == null || formatOut != "png" && (!MightMatchInput(fo.FilePath) || !Common.IsPng(fo.FilePath)) && (!WillMatchInput(fo.FilePath) || imageType != ImageType.Png && imageType != ImageType.Gif && imageType != ImageType.Svg)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Png);

			image.Set("colours", po.Colors);

			image.Pngsave(
				filename:fo.FilePath,
				compression:po.CompressionLevel,
				interlace:po.MakeProgressive,
				profile:null,
				filter:po.UseAdaptiveFiltering ? 0xF8 : 0x08, // TODO: Send netvips the enum for png https://libvips.github.io/libvips/API/current/VipsForeignSave.html#VipsForeignPngFilter https://www.w3.org/TR/PNG-Filters.html
				palette:po.UsePalette,
				q:po.Quality,
				dither:po.Dither,
				bitdepth:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "png";
		}

		private static void PotentiallySavePngStream(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var po = baton.PngOptions;
			var so = baton.ToStreamOptions;
			if(po == null || so == null || formatOut != "png" && (formatOut != "input" || imageType != ImageType.Png && imageType != ImageType.Gif && imageType != ImageType.Svg)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.Png);

			image.Set("colours", po.Colors);

			image.PngsaveStream(
				stream:so.Stream,
				compression:po.CompressionLevel,
				interlace:po.MakeProgressive,
				profile:null,
				filter:po.UseAdaptiveFiltering ? 0xF8 : 0x08, // TODO: Send netvips the enum for png https://libvips.github.io/libvips/API/current/VipsForeignSave.html#VipsForeignPngFilter https://www.w3.org/TR/PNG-Filters.html
				palette:po.UsePalette,
				q:po.Quality,
				dither:po.Dither,
				bitdepth:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "png";
		}

		private static void PotentiallySaveRawBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var ro = baton.RawOptions;
			var bo = baton.ToBufferOptions;
			if(ro == null || bo == null || (formatOut != "raw" && (formatOut != "input" || imageType != ImageType.Raw))) {
				return;
			}

			if(baton.OperationOptions.Grayscale || image.Interpretation == Enums.Interpretation.Bw) {
				// Extract first band for greyscale image
				image = image[0];
				baton.Channels = 1;
			}

			if(image.Format != Enums.BandFormat.Uchar) {
				// Cast pixels to uint8 (unsigned char)
				image = image.Cast(Enums.BandFormat.Uchar);
			}

			bo.Buffer = image.WriteToBuffer(baton.OutputInfo.Format);

			baton.OutputInfo.Format = "raw";
		}

		private static void PotentiallySaveRawStream(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var ro = baton.RawOptions;
			var so = baton.ToStreamOptions;
			if(ro == null || so == null || (formatOut != "raw" && (formatOut != "input" || imageType != ImageType.Raw))) {
				return;
			}

			if(baton.OperationOptions.Grayscale || image.Interpretation == Enums.Interpretation.Bw) {
				// Extract first band for greyscale image
				image = image[0];
				baton.Channels = 1;
			}

			if(image.Format != Enums.BandFormat.Uchar) {
				// Cast pixels to uint8 (unsigned char)
				image = image.Cast(Enums.BandFormat.Uchar);
			}

			image.WriteToStream(so.Stream, baton.OutputInfo.Format);

			baton.OutputInfo.Format = "raw";
		}

		private static void PotentiallySaveTiffBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var to = baton.TiffOptions;
			var bo = baton.ToBufferOptions;
			if(to == null || bo == null || formatOut != "tiff" && (formatOut != "input" || imageType != ImageType.Tiff)) {
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
				compression:to.Compression,
				q:to.Quality,
				predictor:to.Predictor,
				profile:null,
				tile:to.Tile,
				tileWidth:to.TileWidth,
				tileHeight:to.TileHeight,
				pyramid:to.Pyramid,
				miniswhite:null,
				bitdepth:to.BitDepth,
				resunit:null,
				xres:to.XRes,
				yres:to.YRes,
				bigtiff:null,
				properties:null,
				regionShrink:null,
				level:null,
				subifd:null,
				lossless:null,
				depth:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "tiff";
		}

		private static void PotentiallySaveTiffFile(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var to = baton.TiffOptions;
			var fo = baton.ToFileOptions;
			if(to == null || fo == null || formatOut != "tiff" && (!MightMatchInput(fo.FilePath) || !Common.IsTiff(fo.FilePath)) && (!WillMatchInput(fo.FilePath) || imageType != ImageType.Tiff)) {
				return;
			}

			if(to.Compression == Enums.ForeignTiffCompression.Jpeg) {
				image.AssertImageTypeDimensions(ImageType.Jpeg);
				baton.Channels = Math.Min(baton.Channels, 3);
			}

			// TODO: this isn't in sharp yet, opened issue https://github.com/lovell/sharp/issues/2500
			// Cast pixel values to float, if required
			if(to.Predictor == Enums.ForeignTiffPredictor.Float) {
				image = image.Cast(Enums.BandFormat.Float);
			}

			image.Tiffsave(
				compression:to.Compression,
				q:to.Quality,
				predictor:to.Predictor,
				profile:null,
				tile:to.Tile,
				tileWidth:to.TileWidth,
				tileHeight:to.TileHeight,
				pyramid:to.Pyramid,
				miniswhite:null,
				bitdepth:to.BitDepth,
				resunit:null,
				xres:to.XRes,
				yres:to.YRes,
				bigtiff:null,
				properties:null,
				regionShrink:null,
				level:null,
				subifd:null,
				lossless:null,
				depth:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null,
				filename:fo.FilePath
			);

			baton.OutputInfo.Format = "tiff";
		}

		private static void PotentiallySaveTiffStream(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var to = baton.TiffOptions;
			var so = baton.ToStreamOptions;
			if(to == null || so == null || formatOut != "tiff" && (formatOut != "input" || imageType != ImageType.Tiff)) {
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

			// TODO: Pending netvips
			//image.TiffsaveStream(
			//	compression:to.Compression,
			//	q:to.Quality,
			//	predictor:to.Predictor,
			//	profile:null,
			//	tile:to.Tile,
			//	tileWidth:to.TileWidth,
			//	tileHeight:to.TileHeight,
			//	pyramid:to.Pyramid,
			//	miniswhite:null,
			//	bitdepth:to.BitDepth,
			//	resunit:null,
			//	xres:to.XRes,
			//	yres:to.YRes,
			//	bigtiff:null,
			//	properties:null,
			//	regionShrink:null,
			//	level:null,
			//	subifd:null,
			//	lossless:null,
			//	depth:null,
			//	strip:baton.MetadataOptions.ShouldStripMetadata,
			//	background:null,
			//	pageHeight:null
			//);

			baton.OutputInfo.Format = "tiff";
		}

		private static void PotentiallySaveVFile(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var fo = baton.ToFileOptions;
			if(fo == null || formatOut != "v" && (!MightMatchInput(fo.FilePath) || !Common.IsV(fo.FilePath)) && (!WillMatchInput(fo.FilePath) || imageType != ImageType.Vips)) {
				return;
			}

			image.Vipssave(
				filename:fo.FilePath,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "v";
		}

		private static void PotentiallySaveWebpBuffer(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var wo = baton.WebpOptions;
			var bo = baton.ToBufferOptions;
			if(bo == null || wo == null || formatOut != "webp" && (formatOut != "input" || imageType != ImageType.WebP)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.WebP);

			bo.Buffer = image.WebpsaveBuffer(
				q:wo.Quality,
				lossless:wo.UseLossless,
				preset:null,
				smartSubsample:wo.UseSmartSubsample,
				nearLossless:wo.UseNearLossless,
				alphaQ:wo.AlphaQuality,
				minSize:null,
				kmin:null,
				kmax:null,
				reductionEffort:wo.ReductionEffort,
				profile:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "webp";
		}

		private static void PotentiallySaveWebpFile(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var wo = baton.WebpOptions;
			var fo = baton.ToFileOptions;
			if(wo == null || fo == null || formatOut != "webp" && (!MightMatchInput(fo.FilePath) || !Common.IsWebp(fo.FilePath)) && (!WillMatchInput(fo.FilePath) || imageType != ImageType.WebP)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.WebP);

			image.Webpsave(
				filename:fo.FilePath,
				q:wo.Quality,
				lossless:wo.UseLossless,
				preset:null,
				smartSubsample:wo.UseSmartSubsample,
				nearLossless:wo.UseNearLossless,
				alphaQ:wo.AlphaQuality,
				minSize:null,
				kmin:null,
				kmax:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				reductionEffort:wo.ReductionEffort,
				profile:null,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "webp";
		}

		private static void PotentiallySaveWebpStream(PipelineBaton baton, Image image, ImageType imageType) {
			var formatOut = baton.OutputInfo.Format;
			var wo = baton.WebpOptions;
			var so = baton.ToStreamOptions;
			if(so == null || wo == null || formatOut != "webp" && (formatOut != "input" || imageType != ImageType.WebP)) {
				return;
			}

			image.AssertImageTypeDimensions(ImageType.WebP);

			image.WebpsaveStream(
				stream:so.Stream,
				q:wo.Quality,
				lossless:wo.UseLossless,
				preset:null,
				smartSubsample:wo.UseSmartSubsample,
				nearLossless:wo.UseNearLossless,
				alphaQ:wo.AlphaQuality,
				minSize:null,
				kmin:null,
				kmax:null,
				reductionEffort:wo.ReductionEffort,
				profile:null,
				strip:baton.MetadataOptions.ShouldStripMetadata,
				background:null,
				pageHeight:null
			);

			baton.OutputInfo.Format = "webp";
		}

		private static Image PreExtraction(PipelineBaton baton, Image image) {
			var ro = baton.ResizeOptions;
			if(ro.TopOffsetPre != -1) {
				image = image.ExtractArea(ro.LeftOffsetPre, ro.TopOffsetPre, ro.Width, ro.Height);
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

		private static Image Recomb(PipelineBaton baton, Image image) {
			var oo = baton.OperationOptions;
			if(oo.RecombMatrix.HasValue()) {
				image = image.Recomb(oo.RecombMatrix);
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

		private static Image RemoveAlphaChannelIfAny(PipelineBaton baton, Image image) {
			// Remove alpha channel, if any
			if(baton.OperationOptions.RemoveAlpha) {
				image = image.RemoveAlpha();
			}

			return image;
		}

		private static Image Resize(PipelineBaton baton, Image image, bool shouldResize, double xFactor, double yFactor) {
			if(!shouldResize) {
				return image;
			}

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

			return image;
		}

		private static Image ReversePremultiplication(PipelineBaton baton, Image image, bool shouldPremultiplyAlpha) {
			if(shouldPremultiplyAlpha) {
				image = image.Unpremultiply();
				// Cast pixel values to integer
				image = image.Cast(image.Interpretation.Is16Bit() ? Enums.BandFormat.Ushort : Enums.BandFormat.Uchar);
			}

			baton.OperationOptions.Premultiplied = shouldPremultiplyAlpha;
			return image;
		}

		private static Image RotatePostExtract(PipelineBaton baton, Image image) {
			if(baton.RotationOptions.RotateBeforePreExtract || baton.RotationOptions.RotationAngle.IsAboutEqualTo(0.0)) {
				return image;
			}

			var (img, background) = Common.ApplyAlpha(image, baton.RotationOptions.RotationBackground);
			image = img.Rotate(baton.RotationOptions.RotationAngle, background:background);

			return image;
		}

		private static Image RotatePostExtract(PipelineBaton baton, Image image, string rotation) {
			if(baton.RotationOptions.RotateBeforePreExtract || rotation == Enums.Angle.D0) {
				return image;
			}

			image = image.Rot(rotation);
			image = image.RemoveExifOrientation();

			return image;
		}

		private static Image RotatePreExtract(Image image, string rotation, RotationOptions rotationOptions) {
			if(!rotationOptions.RotateBeforePreExtract) {
				return image;
			}

			if(rotation != Enums.Angle.D0) {
				image = image.Rot(rotation);
				image = image.RemoveExifOrientation();
			}

			if(rotationOptions.RotationAngle == 0.0) {
				return image;
			}

			var (img, background) = Common.ApplyAlpha(image, rotationOptions.RotationBackground);
			image = img;
			image = image.Rotate(rotationOptions.RotationAngle, background:background);

			return image;
		}

		private static Image SetAnimationPropertiesIfAny(PipelineBaton baton, Image image) {
			var ao = baton.AnimationOptions;
			image = image.SetAnimationProperties(ao.PageHeight, ao.Delay, ao.Loop);
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

			if(shrinkOnLoad <= 1) {
				return image;
			}

			// Reload input using shrink-on-load
			var access = imageSource.Options.UseSequentialRead ? Enums.Access.Sequential : Enums.Access.Random;
			switch(imageSource) {
				case BufferImageSource bufferImageSource:
					image = imageType == ImageType.Jpeg ?
						Image.JpegloadBuffer(bufferImageSource.Buffer, shrinkOnLoad, null, null, access, true) :
						Image.WebploadBuffer(bufferImageSource.Buffer, imageSource.Options.PageIndex, imageSource.Options.PageCount, null, null, access, true);
					break;
				case FileImageSource fileImageSource when imageType == ImageType.Jpeg:
					image = Image.Jpegload(fileImageSource.Path, shrinkOnLoad, null, null, access, true);
					break;
				case FileImageSource fileImageSource: {
					if(imageType == ImageType.WebP) {
						image = Image.Webpload(fileImageSource.Path, imageSource.Options.PageIndex, imageSource.Options.PageCount, null, null, access, true);
					}

					break;
				}
			}

			// Recalculate integral shrink and double residual
			var shrunkOnLoadWidth = image.Width;
			var shrunkOnLoadHeight = image.Height;

			if(!baton.ResizeOptions.RotateBeforePreExtract && (rotation == Enums.Angle.D90 || rotation == Enums.Angle.D270)) {
				// Swap when rotating by 90 or 270 degrees
				xFactor = shrunkOnLoadWidth / (double) targetResizeHeight;
				yFactor = shrunkOnLoadHeight / (double) targetResizeWidth;
			}
			else {
				xFactor = shrunkOnLoadWidth / (double) targetResizeWidth;
				yFactor = shrunkOnLoadHeight / (double) targetResizeHeight;
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

		private static Image Tint(PipelineBaton baton, Image image) {
			// Tint the image
			var oo = baton.OperationOptions;
			if(oo.TintA < 128.0 || oo.TintB < 128.0) {
				image = image.Tint(oo.TintA, oo.TintB);
			}

			return image;
		}

		private static Image Trim(PipelineBaton baton, Image image) {
			var trimOptions = baton.TrimOptions;
			if(!(trimOptions.TrimThreshold > 0.0)) {
				return image;
			}

			image = image.Trim(trimOptions.TrimThreshold);
			trimOptions.TrimOffsetLeft = image.Xoffset;
			trimOptions.TrimOffsetTop = image.Yoffset;

			return image;
		}

		private static bool WillMatchInput(string path) => MightMatchInput(path) && !(Common.IsJpeg(path) || Common.IsPng(path) || Common.IsWebp(path) || Common.IsGif(path) || Common.IsTiff(path) || Common.IsHeif(path) || Common.IsDz(path) || Common.IsDzZip(path) || Common.IsV(path));

		private Image JoinAdditionalColorChannels(PipelineBaton baton, Image image) =>
			// TODO: this
			//if(baton.OperationOptions.JoinChannelIn.HasValue()) {
			//	for(var i = 0; i < baton.OperationOptions.JoinChannelIn.Length; i++) {
			//		var (joinImage, joinImageType) = sharp::OpenInput(baton->joinChannelIn[i]);
			//		image = image.bandjoin(joinImage);
			//	}
			//	image = image.copy(VImage::option()->set("interpretation", baton->colourspace));
			//}
			image;
	}
}
