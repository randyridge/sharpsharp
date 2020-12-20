using System;
using System.Collections.Generic;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
	internal static class NeedsBetterPlace {
		private static readonly int[] MinimumImage = {1, 1};

		public static (Image Image, double[] AlphaColors) ApplyAlpha(Image image, double[] colors) {
			var multiplier = Is16Bit(image.Interpretation) ? 256.0 : 1.0;
			var alphaColors = new List<double>(4);

			if(image.Bands > 2) {
				alphaColors.Add(multiplier * colors[0]);
				alphaColors.Add(multiplier * colors[1]);
				alphaColors.Add(multiplier * colors[2]);
			}
			else {
				alphaColors.Add(multiplier * (0.2126 * colors[0]));
				alphaColors.Add(multiplier * (0.7152 * colors[1]));
				alphaColors.Add(multiplier * (0.0722 * colors[2]));
			}

			if(colors[3] < 255.0 || image.HasAlpha()) {
				alphaColors.Add(colors[3] * multiplier);
			}

			var result = GetRgbaAsColorspace(alphaColors.ToArray(), image.Interpretation);
			if(colors[3] < 255.0 && !image.HasAlpha()) {
				var alphaPixels = new double[image.Width, image.Height];
				for(var x = 0; x < image.Width; x++) {
					for(var y = 0; y < image.Height; y++) {
						alphaPixels[x, y] = 255.0 * multiplier;
					}
				}

				image = image.Bandjoin(Image.NewFromArray(alphaPixels));
			}

			return (image, result);
		}

		public static (int Left, int Top) CalculateCrop(int inWidth, int inHeight, int outWidth, int outHeight, Gravity gravity) {
			var left = 0;
			var top = 0;

			switch(gravity) {
				case Gravity.North:
					left = (inWidth - outWidth + 1) / 2;
					break;
				case Gravity.East:
					left = inWidth - outWidth;
					top = (inHeight - outHeight + 1) / 2;
					break;
				case Gravity.South:
					left = (inWidth - outWidth + 1) / 2;
					top = inHeight - outHeight;
					break;
				case Gravity.West:
					top = (inHeight - outHeight + 1) / 2;
					break;
				case Gravity.Northeast:
					left = inWidth - outWidth;
					break;
				case Gravity.Southeast:
					left = inWidth - outWidth;
					top = inHeight - outHeight;
					break;
				case Gravity.Southwest:
					top = inHeight - outHeight;
					break;
				case Gravity.Northwest:
					break;
				// case Gravity.Center:
				default:
					left = (inWidth - outWidth + 1) / 2;
					top = (inHeight - outHeight + 1) / 2;
					break;
			}

			return (left, top);
		}

		public static (int Left, int Top) CalculateCrop(int inWidth, int inHeight, int outWidth, int outHeight, int x, int y) {
			var left = 0;
			var top = 0;

			if(x >= 0 && x < inWidth - outWidth) {
				left = x;
			}
			else if(x >= inWidth - outWidth) {
				left = inWidth - outWidth;
			}

			if(y >= 0 && y < inHeight - outHeight) {
				top = y;
			}
			else if(y >= inHeight - outHeight) {
				top = inHeight - outHeight;
			}

			return (Math.Max(0, left), Math.Max(0, top));
		}

		public static (int Left, int Top) CalculateEmbedPosition(int inWidth, int inHeight, int outWidth, int outHeight, Gravity gravity) {
			var left = 0;
			var top = 0;

			switch(gravity) {
				case Gravity.North:
					left = (outWidth - inWidth) / 2;
					break;
				case Gravity.East:
					left = outWidth - inWidth;
					top = (outHeight - inHeight) / 2;
					break;
				case Gravity.South:
					left = (outWidth - inWidth) / 2;
					top = outHeight - inHeight;
					break;
				case Gravity.West:
					top = (outHeight - inHeight) / 2;
					break;
				case Gravity.Northeast:
					left = outWidth - inWidth;
					break;
				case Gravity.Southeast:
					left = outWidth - inWidth;
					top = outHeight - inHeight;
					break;
				case Gravity.Southwest:
					top = outHeight - inHeight;
					break;
				case Gravity.Northwest:
					break;
				// case Gravity.Center:
				default:
					left = (outWidth - inWidth) / 2;
					top = (outHeight - inHeight) / 2;
					break;
			}

			return (left, top);
		}

		public static double[] GetRgbaAsColorspace(double[] rgba, string interpretation) {
			Guard.NotNull(rgba, nameof(rgba));
			Guard.NotNull(interpretation, nameof(interpretation));
			var bands = rgba.Length;
			if(bands < 3 || interpretation == Enums.Interpretation.Srgb || interpretation == Enums.Interpretation.Rgb) {
				return rgba;
			}

			var pixel = Image.NewFromArray(MinimumImage);
			pixel.Set("bands", bands);
			pixel = pixel.NewFromImage(rgba);
			pixel = pixel.Colourspace(interpretation, Enums.Interpretation.Srgb);
			return pixel[0, 0];
		}

		public static bool Is16Bit(this string interpretation) {
			Guard.NotNull(interpretation, nameof(interpretation));
			return interpretation == Enums.Interpretation.Rgb16 ||
			       interpretation == Enums.Interpretation.Grey16;
		}

		public static double MaximumImageAlpha(this string interpretation) {
			Guard.NotNull(interpretation, nameof(interpretation));
			return interpretation.Is16Bit() ? ushort.MaxValue : byte.MaxValue;
		}

		public static (Image Image, ImageType ImageType) OpenInput(InputDescriptor descriptor, string accessMethod) {
			descriptor = Guard.NotNull(descriptor, nameof(descriptor));

			Image image;
			ImageType imageType;

			if(descriptor.Buffer != null) {
				if(descriptor.RawChannels > 0) {
					// raw, uncompressed pixel data
					image = Image.NewFromMemory(descriptor.Buffer, descriptor.RawWidth, descriptor.RawHeight, descriptor.RawChannels, Enums.BandFormat.Uchar);
					image.Set("interpretation", descriptor.RawChannels < 3 ? Enums.Interpretation.Bw : Enums.Interpretation.Srgb);
					imageType = ImageType.Raw;
				}
				else {
					// Compressed data
					imageType = ImageType.FromBuffer(descriptor.Buffer);
					if(imageType == ImageType.Unknown) {
						throw new VipsException("Input buffer contains unsupported image format.");
					}

					try {
						var option = new VOption {
							{"access", accessMethod},
							{"fail", descriptor.FailOnError}
						};

						if(imageType == ImageType.Svg || imageType == ImageType.Pdf) {
							option.Add("dpi", descriptor.Density);
						}

						if(imageType == ImageType.Magick) {
							option.Add("density", descriptor.Density);
						}

						if(imageType.SupportsPages) {
							option.Add("n", descriptor.Pages);
							option.Add("page", descriptor.Page);
						}

						image = Image.NewFromBuffer(descriptor.Buffer, null, null, null, option);

						if(imageType == ImageType.Svg || imageType == ImageType.Pdf || imageType == ImageType.Magick) {
							image.SetDensity(descriptor.Density);
						}
					}
					catch(Exception ex) {
						throw new VipsException($"Input buffer has corrupt header: {ex.Message}", ex);
					}
				}
			}
			else {
				if(descriptor.CreateChannels > 0) {
					// create new image
					var background = new List<double> {
						descriptor.CreateBackground[0],
						descriptor.CreateBackground[1],
						descriptor.CreateBackground[2]
					};

					if(descriptor.CreateChannels == 4) {
						background.Add(descriptor.CreateBackground[3]);
					}

					image = Image.NewFromArray(background.ToArray()); // TODO: suspect
					image.Set("interpretation", Enums.Interpretation.Srgb);
					imageType = ImageType.Raw;
				}
				else {
					imageType = ImageType.FromFile(descriptor.File!);

					if(imageType == ImageType.Missing) {
						throw new VipsException($"Input file {descriptor.File} is missing.");
					}

					if(imageType == ImageType.Unknown) {
						throw new VipsException("Input file contains unsupported image format.");
					}

					try {
						var option = new VOption {
							{"access", accessMethod},
							{"fail", descriptor.FailOnError}
						};

						if(imageType == ImageType.Svg || imageType == ImageType.Pdf) {
							option.Add("dpi", descriptor.Density);
						}

						if(imageType == ImageType.Magick) {
							option.Add("density", descriptor.Density);
						}

						if(imageType.SupportsPages) {
							option.Add("n", descriptor.Pages);
							option.Add("page", descriptor.Page);
						}

						image = Image.NewFromFile(descriptor.File, null, null, null, option);
						if(imageType == ImageType.Svg || imageType == ImageType.Pdf || imageType == ImageType.Magick) {
							image.SetDensity(descriptor.Density);
						}
					}
					catch(Exception ex) {
						throw new VipsException($"Input file has corrupt header: {ex.Message}", ex);
					}
				}
			}

			return (image, imageType);
		}
	}
}
