using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
	/// <summary>
	///		Provides a pipeline of operations for images.
	/// </summary>
	public sealed partial class ImagePipeline {
		private static readonly HttpClient DefaultHttpClient = new();

		/// <summary>
		///   Reads an image from the specified buffer using the specified image load options.
		/// </summary>
		/// <param name="buffer">
		///   The buffer containing the image.
		/// </param>
		/// <param name="options">
		///   The image load options.
		/// </param>
		/// <returns>
		///   An image pipeline.
		/// </returns>
		/// <exception cref="ArgumentException">
		///   Thrown if <paramref name="buffer" /> is empty.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="buffer" /> is null.
		/// </exception>
		public static ImagePipeline FromBuffer(byte[] buffer, ImageLoadOptions? options = null) {
			buffer = Guard.NotNull(buffer, nameof(buffer));
			return new ImagePipeline(new BufferImageSource(buffer, options ?? new ImageLoadOptions()));
		}

		/// <summary>
		///   Reads an image from the specified path using the specified image load options.
		/// </summary>
		/// <param name="path">
		///   The path to the image.
		/// </param>
		/// <param name="options">
		///   The image load options.
		/// </param>
		/// <returns>
		///   An image pipeline.
		/// </returns>
		/// <exception cref="ArgumentException">
		///   Thrown if <paramref name="path" /> is empty or contains only whitespace.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="path" /> is null.
		/// </exception>
		/// <exception cref="FileNotFoundException">
		///   Thrown if <paramref name="path" /> is not found.
		/// </exception>
		public static ImagePipeline FromFile(string path, ImageLoadOptions? options = null) {
			path = Guard.FileExists(path, nameof(path));
			return new ImagePipeline(new FileImageSource(path, options ?? new ImageLoadOptions()));
		}

		/// <summary>
		///   Reads an image from the specified vips image with the specified image load options.
		/// </summary>
		/// <param name="image">
		///   The vips image.
		/// </param>
		/// <param name="options">
		///   The image load options.
		/// </param>
		/// <returns>
		///   An image pipeline.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="image" /> is null.
		/// </exception>
		public static ImagePipeline FromImage(Image image, ImageLoadOptions? options = null) {
			image = Guard.NotNull(image, nameof(image));
			return new ImagePipeline(new VipsImageSource(image, options ?? new ImageLoadOptions()));
		}

		/// <summary>
		///   Reads an image from the specified stream with the specified image load options.
		/// </summary>
		/// <param name="stream">
		///   The stream containing the image.
		/// </param>
		/// <param name="options">
		///   The image load options.
		/// </param>
		/// <returns>
		///   An image pipeline.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="stream" /> is null.
		/// </exception>
		public static ImagePipeline FromStream(Stream stream, ImageLoadOptions? options = null) {
			stream = Guard.NotNull(stream, nameof(stream));
			return new ImagePipeline(new StreamImageSource(stream, options ?? new ImageLoadOptions()));
		}

		/// <summary>
		///   Reads an image from the specified URI string with the specified image load options.
		/// </summary>
		/// <param name="uri">
		///   The URI to the image.
		/// </param>
		/// <param name="options">
		///   The image load options.
		/// </param>
		/// <param name="client">
		///   An optional HTTP client, a default is provided.
		/// </param>
		/// <returns>
		///   A task representing the image pipeline read operation.
		/// </returns>
		/// <exception cref="ArgumentException">
		///   Thrown if <paramref name="uri" /> is empty or contains only whitespace.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="uri" /> is null.
		/// </exception>
		public static Task<ImagePipeline> FromUriAsync(string uri, ImageLoadOptions? options = null, HttpClient? client = null) {
			uri = Guard.NotNullOrWhiteSpace(uri, nameof(uri));
			return FromUriAsync(new Uri(uri), options, client);
		}

		/// <summary>
		///   Reads an image from the specified URI with the specified image load options.
		/// </summary>
		/// <param name="uri">
		///   The URI to the image.
		/// </param>
		/// <param name="options">
		///   The image load options.
		/// </param>
		/// <param name="client">
		///   An optional HTTP client, a default is provided.
		/// </param>
		/// <returns>
		///   A task representing the image pipeline read operation.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///   Thrown if <paramref name="uri" /> is null.
		/// </exception>
		public static async Task<ImagePipeline> FromUriAsync(Uri uri, ImageLoadOptions? options = null, HttpClient? client = null) {
			uri = Guard.NotNull(uri, nameof(uri));
			client ??= DefaultHttpClient;
			return new ImagePipeline(new StreamImageSource(await client.GetStreamAsync(uri).ForAwait(), options ?? new ImageLoadOptions()));
		}
	}
}
