using System.IO;
using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
	internal sealed class StreamImageSource : ImageSource {
		public StreamImageSource(Stream stream, ImageLoadOptions options) : base(options) {
			Stream = Guard.NotNull(stream, nameof(stream));
		}

		public Stream Stream { get; }

		public override (Image, ImageType) Load(VOption? options = null) {
			try {
				var ms = new MemoryStream(); // TODO: GlobalStatics.RecyclableMemoryStreamManager.GetStream();

				Stream.CopyTo(ms);
				Stream.Reset();

				var imageType = ImageType.FromStream(ms);
				options ??= BuildLoadOptionsFromImageType(imageType);

				ms.Reset();

				var image = Image.NewFromStream(
					ms, 
					null,
					Options.UseSequentialRead ? Enums.Access.Sequential : Enums.Access.Random,
					true,
					options
				);

				// TODO: this
				//if(image.Width * image.Height > Options.PixelLimit) {
				//    throw new SharpSharpException("Input image exceeds pixel limit.");
				//}

				return (image, imageType);
			}
			catch(VipsException ex) {
				throw new SharpSharpException("Error loading image from stream.", ex);
			}
		}
	}
}
