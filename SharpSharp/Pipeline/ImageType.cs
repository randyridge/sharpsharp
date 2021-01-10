using System;
using System.IO;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
	internal sealed class ImageType : IEquatable<ImageType> {
		private const int Density = 72; // TODO: hardcoding density for now
		//private static readonly string DensityText = Density.ToString();
		public static readonly ImageType Fits = new("fits", "application/fits");
		public static readonly ImageType Gif = new("gif", "image/gif", true);
		public static readonly ImageType Heif = new("heif", "image/heif", true);
		public static readonly ImageType Jpeg = new("jpeg", "image/jpeg");
		public static readonly ImageType Magick = new("magick", "application/octet-stream");
		public static readonly ImageType Missing = new("missing", "application/octet-stream");
		public static readonly ImageType Openslide = new("openslide", "application/octet-stream");
		public static readonly ImageType Pdf = new("pdf", "application/pdf", true);
		public static readonly ImageType Png = new("png", "image/png");
		public static readonly ImageType Ppm = new("ppm", "application/octet-stream");
		public static readonly ImageType Raw = new("raw", "application/octet-stream");
		public static readonly ImageType Svg = new("svg", "image/svg+xml");
		public static readonly ImageType Tiff = new("tiff", "image/tiff", true);
		public static readonly ImageType Unknown = new("unknown", "application/octet-stream");
		public static readonly ImageType Vips = new("vips", "application/octet-stream");
		public static readonly ImageType WebP = new("webp", "image/webp", true);

		private ImageType(string extension, string mimeType, bool supportsPages = false) {
			Extension = Guard.NotNullOrWhiteSpace(extension, nameof(extension));
			MimeType = Guard.NotNullOrWhiteSpace(mimeType, nameof(mimeType));
			SupportsPages = supportsPages;
		}

		public string Extension { get; }

		public string ImageTypeId => Extension;

		public string MimeType { get; }

		public bool SupportsPages { get; }

		public bool Equals(ImageType? other) {
			if(other is null) {
				return false;
			}

			if(ReferenceEquals(this, other)) {
				return true;
			}

			return Extension == other.Extension && MimeType == other.MimeType;
		}

		public static ImageType FromBuffer(byte[] buffer) {
			Guard.NotNullOrEmpty(buffer, nameof(buffer));

			var result = Image.FindLoadBuffer(buffer);

			if(result == null) {
				return Unknown;
			}

			if(result.EndsWithOrdinalIgnoreCase("JpegBuffer")) {
				return Jpeg;
			}

			if(result.EndsWithOrdinalIgnoreCase("PngBuffer")) {
				return Png;
			}

			if(result.EndsWithOrdinalIgnoreCase("WebpBuffer")) {
				return WebP;
			}

			if(result.EndsWithOrdinalIgnoreCase("TiffBuffer")) {
				return Tiff;
			}

			if(result.EndsWithOrdinalIgnoreCase("GifBuffer")) {
				return Gif;
			}

			if(result.EndsWithOrdinalIgnoreCase("SvgBuffer")) {
				return Svg;
			}

			if(result.EndsWithOrdinalIgnoreCase("HeifBuffer")) {
				return Heif;
			}

			if(result.EndsWithOrdinalIgnoreCase("PdfBuffer")) {
				return Pdf;
			}

			if(result.EndsWithOrdinalIgnoreCase("MagickBuffer")) {
				return Magick;
			}

			return Unknown;
		}

		public static ImageType FromFile(string path) {
			Guard.NotNullOrWhiteSpace(path, nameof(path));

			var result = Image.FindLoad(path);

			if(result == null) {
				return Missing;
			}

			if(result.EndsWithOrdinalIgnoreCase("JpegFile")) {
				return Jpeg;
			}

			if(result.EndsWithOrdinalIgnoreCase("Png")) {
				return Png;
			}

			if(result.EndsWithOrdinalIgnoreCase("WebpFile")) {
				return WebP;
			}

			if(result.EndsWithOrdinalIgnoreCase("Openslide")) {
				return Openslide;
			}

			if(result.EndsWithOrdinalIgnoreCase("TiffFile")) {
				return Tiff;
			}

			if(result.EndsWithOrdinalIgnoreCase("GifFile")) {
				return Gif;
			}

			if(result.EndsWithOrdinalIgnoreCase("SvgFile")) {
				return Svg;
			}

			if(result.EndsWithOrdinalIgnoreCase("HeifFile")) {
				return Heif;
			}

			if(result.EndsWithOrdinalIgnoreCase("PdfFile")) {
				return Pdf;
			}

			if(result.EndsWithOrdinalIgnoreCase("Ppm")) {
				return Ppm;
			}

			if(result.EndsWithOrdinalIgnoreCase("Fits")) {
				return Fits;
			}

			if(result.EndsWithOrdinalIgnoreCase("Vips")) {
				return Vips;
			}

			if(result.EndsWithOrdinalIgnoreCase("Magick") || result.EndsWithOrdinalIgnoreCase("MagickFile")) {
				return Magick;
			}

			return Unknown;
		}

		public static ImageType FromImage(Image image) {
			Guard.NotNull(image, nameof(image));
			return image.Filename.HasValue() ? FromFile(image.Filename) : Unknown;
		}

		public static ImageType FromStream(Stream stream) {
			Guard.NotNull(stream, nameof(stream));
			stream.Seek(0, SeekOrigin.Begin);
			var result = Image.FindLoadStream(stream);

			if(result == null) {
				return Unknown;
			}

			if(result.EndsWithOrdinalIgnoreCase("JpegSource")) {
				return Jpeg;
			}

			if(result.EndsWithOrdinalIgnoreCase("PngSource")) {
				return Png;
			}

			if(result.EndsWithOrdinalIgnoreCase("WebpSource")) {
				return WebP;
			}

			// TODO: Not supported?
			if(result.EndsWithOrdinalIgnoreCase("TiffSource")) {
				return Tiff;
			}

			// TODO: Not supported?
			if(result.EndsWithOrdinalIgnoreCase("GifSource")) {
				return Gif;
			}

			if(result.EndsWithOrdinalIgnoreCase("SvgSource")) {
				return Svg;
			}

			if(result.EndsWithOrdinalIgnoreCase("HeifSource")) {
				return Heif;
			}

			if(result.EndsWithOrdinalIgnoreCase("PdfSource")) {
				return Pdf;
			}

			// TODO: Not supported?
			if(result.EndsWithOrdinalIgnoreCase("MagickSource")) {
				return Magick;
			}

			return Unknown;
		}

		public static bool operator ==(ImageType left, ImageType right) => Equals(left, right);

		public static bool operator !=(ImageType left, ImageType right) => !Equals(left, right);

		public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is ImageType other && Equals(other);

		public override int GetHashCode() {
			unchecked {
				return ((Extension != null ? Extension.GetHashCode(StringComparison.OrdinalIgnoreCase) : 0) * 397) ^ (MimeType != null ? MimeType.GetHashCode(StringComparison.OrdinalIgnoreCase) : 0);
			}
		}
	}
}
