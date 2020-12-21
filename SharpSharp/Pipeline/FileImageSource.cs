using NetVips;
using RandyRidge.Common;

namespace SharpSharp.Pipeline {
	internal sealed class FileImageSource : ImageSource {
		public FileImageSource(string path, ImageLoadOptions options) : base(options) {
			Path = Guard.NotNullOrEmpty(path, nameof(path));
		}

		public string Path { get; }

		public override (Image, ImageType) Load(VOption? options = null) {
			try {
				var imageType = ImageType.FromFile(Path);

				options ??= BuildLoadOptionsFromImageType(imageType);

				var image = Image.NewFromFile(
					Path, 
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
				throw new SharpSharpException("Error loading image from buffer.", ex);
			}
		}
	}
}
