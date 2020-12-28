using System;
using System.IO;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp {
	public sealed partial class ImagePipeline {
		public ImagePipeline Avif() => Heif(new HeifOptions());

		public ImagePipeline Avif(HeifOptions options) => Heif(options);
		
		public ImagePipeline Heif() => Heif(new HeifOptions());

		public ImagePipeline Heif(HeifOptions options) {
			options = Guard.NotNull(options, nameof(options));
			baton.HeifOptions = options;
			return this;
		}

		/// <summary>
		///   Use JPEG with default options for output image.
		/// </summary>
		/// <returns>
		///   The image pipeline.
		/// </returns>
		public ImagePipeline Jpeg() => Jpeg(new JpegOptions());

		/// <summary>
		///   Use JPEG with the specified options for output image.
		/// </summary>
		/// <param name="options">
		///   The options for the output image.
		/// </param>
		/// <returns>
		///   The image pipeline.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="options" /> is null.
		/// </exception>
		public ImagePipeline Jpeg(JpegOptions options) {
			Guard.NotNull(options, nameof(options));
			baton.JpegOptions = options;
			return this;
		}

		/// <summary>
		///   Use PNG with default options for output image.
		/// </summary>
		/// <returns>
		///   The image pipeline.
		/// </returns>
		public ImagePipeline Png() => Png(new PngOptions());

		/// <summary>
		///   Use PNG with the specified options for output image.
		/// </summary>
		/// <param name="options">
		///   The options for the output image.
		/// </param>
		/// <returns>
		///   The image pipeline.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="options" /> is null.
		/// </exception>
		public ImagePipeline Png(PngOptions options) {
			Guard.NotNull(options, nameof(options));
			baton.PngOptions = options;
			return this;
		}

		public ImagePipeline Raw() => Raw(new RawOptions());

		public ImagePipeline Raw(RawOptions options) {
			Guard.NotNull(options, nameof(options));
			baton.RawOptions = options;
			return this;
		}

//        // TODO: this
//        public ImagePipeline Raw() => throw new NotImplementedException();

//        // TODO: this
//        public ImagePipeline Tiff() => throw new NotImplementedException();

//        // TODO: this
//        public ImagePipeline Tile() => throw new NotImplementedException();

/// <summary>
///   Writes the result of the image pipeline to the specified buffer.
/// </summary>
/// <param name="buffer">
///   The buffer to write to.
/// </param>
public void ToBuffer(out byte[] buffer) {
			buffer = Array.Empty<byte>();
			ToBuffer(new ToBufferOptions(buffer, null));
			buffer = baton.ToBufferOptions!.Buffer;
		}

/// <summary>
///   Writes the result of the image pipeline to the specified buffer and invokes a callback with
///   <see cref="OutputInfo" />.
/// </summary>
/// <param name="callback">
///   The callback to receive <see cref="OutputInfo" />.
/// </param>
/// <param name="buffer">
///   The buffer to write to.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="callback" /> is null.
/// </exception>
public void ToBuffer(Action<OutputInfo> callback, out byte[] buffer) {
			buffer = Array.Empty<byte>();
			ToBuffer(new ToBufferOptions(buffer, callback));
			buffer = baton.ToBufferOptions!.Buffer;
		}

/// <summary>
///   Writes the result of the image pipeline using the specified <see cref="ToBufferOptions" />.
/// </summary>
/// <param name="bufferOptions">
///   The file options to use.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="bufferOptions" /> is null.
/// </exception>
public void ToBuffer(ToBufferOptions bufferOptions) {
			Guard.NotNull(bufferOptions, nameof(bufferOptions));
			baton.ToBufferOptions = bufferOptions;
			Execute();
		}

/// <summary>
///   Writes the result of the image pipeline to the specified file path.
/// </summary>
/// <param name="filePath">
///   The file path to write to.
/// </param>
/// <exception cref="ArgumentException">
///   Thrown if <paramref name="filePath" /> is empty or contains only whitespace.
/// </exception>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="filePath" /> is null.
/// </exception>
public void ToFile(string filePath) {
			Guard.NotNullOrWhiteSpace(filePath, nameof(filePath));
			ToFile(new ToFileOptions(filePath, null));
		}

/// <summary>
///   Writes the result of the image pipeline to the specified file path and invokes a callback with
///   <see cref="OutputInfo" />.
/// </summary>
/// <param name="filePath">
///   The file path to write to.
/// </param>
/// <param name="callback">
///   The callback to receive <see cref="OutputInfo" />.
/// </param>
/// <exception cref="ArgumentException">
///   Thrown if <paramref name="filePath" /> is empty or contains only whitespace.
/// </exception>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="filePath" /> or <paramref name="callback" /> is null.
/// </exception>
public void ToFile(string filePath, Action<OutputInfo> callback) {
			Guard.NotNullOrWhiteSpace(filePath, nameof(filePath));
			Guard.NotNull(callback, nameof(callback));
			ToFile(new ToFileOptions(filePath, callback));
		}

/// <summary>
///   Writes the result of the image pipeline using the specified <see cref="ToFileOptions" />.
/// </summary>
/// <param name="fileOptions">
///   The file options to use.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="fileOptions" /> is null.
/// </exception>
public void ToFile(ToFileOptions fileOptions) {
			Guard.NotNull(fileOptions, nameof(fileOptions));
			baton.ToFileOptions = fileOptions;
			Execute();
		}

/// <summary>
///   Writes the result of the image pipeline to the specified stream.
/// </summary>
/// <param name="stream">
///   The stream to write to.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="stream" /> is null.
/// </exception>
public void ToStream(Stream stream) {
			stream = Guard.NotNull(stream, nameof(stream));
			ToStream(new ToStreamOptions(stream, null));
		}

/// <summary>
///   Writes the result of the image pipeline to the specified stream and invokes a callback with
///   <see cref="OutputInfo" />.
/// </summary>
/// <param name="stream">
///   The stream to write to.
/// </param>
/// <param name="callback">
///   The callback to receive <see cref="OutputInfo" />.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="stream" /> or <paramref name="callback" /> is null.
/// </exception>
public void ToStream(Stream stream, Action<OutputInfo> callback) {
			stream = Guard.NotNull(stream, nameof(stream));
			Guard.NotNull(callback, nameof(callback));
			ToStream(new ToStreamOptions(stream, callback));
		}

/// <summary>
///   Writes the result of the image pipeline to the specified stream.
/// </summary>
/// <param name="stream">
///   The stream to write to.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="stream" /> is null.
/// </exception>
public void ToStream(out Stream stream) {
			stream = new MemoryStream();
			ToStream(new ToStreamOptions(stream, null));
		}

/// <summary>
///   Writes the result of the image pipeline to the specified stream and invokes a callback with
///   <see cref="OutputInfo" />.
/// </summary>
/// <param name="stream">
///   The stream to write to.
/// </param>
/// <param name="callback">
///   The callback to receive <see cref="OutputInfo" />.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="stream" /> or <paramref name="callback" /> is null.
/// </exception>
public void ToStream(Action<OutputInfo> callback, out Stream stream) {
			stream = new MemoryStream();
			Guard.NotNull(callback, nameof(callback));
			ToStream(new ToStreamOptions(stream, callback));
		}

/// <summary>
///   Writes the result of the image pipeline to the specified stream with the specified <see cref="ToStreamOptions" />.
/// </summary>
/// <param name="streamOptions">
///   The stream options to use.
/// </param>
/// <exception cref="ArgumentNullException">
///   Thrown if <paramref name="streamOptions" /> is null.
/// </exception>
public void ToStream(ToStreamOptions streamOptions) {
			Guard.NotNull(streamOptions, nameof(streamOptions));
			baton.ToStreamOptions = streamOptions;
			Execute();
			if(streamOptions.Callback.HasValue()) {
				streamOptions.Callback(baton.OutputInfo!);
			}
		}

		public ImagePipeline VipsImage(Action<Image> callback) {
			Guard.NotNull(callback, nameof(callback));
			var (image, _) = imageSource.Load();
			callback(image);
			image?.Dispose();
			return this;
		}

		/// <summary>
		///   Use WebP with default options for output image.
		/// </summary>
		/// <returns>
		///   The image pipeline.
		/// </returns>
		public ImagePipeline Webp() => Webp(new WebpOptions());

		/// <summary>
		///   Use WebP with the specified options for output image.
		/// </summary>
		/// <param name="options">
		///   The options for the output image.
		/// </param>
		/// <returns>
		///   The image pipeline.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="options" /> is null.
		/// </exception>
		public ImagePipeline Webp(WebpOptions options) {
			Guard.NotNull(options, nameof(options));
			baton.WebpOptions = options;
			return this;
		}

		public ImagePipeline WithMetadata() => WithMetadata(new MetadataOptions());

		public ImagePipeline WithMetadata(MetadataOptions options) {
			Guard.NotNull(options, nameof(options));
			baton.MetadataOptions = options;
			return this;
		}
	}
}
