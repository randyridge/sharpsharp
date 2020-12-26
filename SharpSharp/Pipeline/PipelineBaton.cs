using NetVips;

namespace SharpSharp.Pipeline {
	internal sealed class PipelineBaton {
		public AnimationOptions AnimationOptions { get; set; } = new();
		public int Channels { get; set; }

		public CropOffsetOptions CropOffsetOptions { get; set; } = new();
		
		public HeifOptions? HeifOptions { get; set; }

		public int Height { get; set; }

		public Image? Image { get; set; }

		public ImageType? InputImageType { get; set; }

		public JpegOptions? JpegOptions { get; set; }

		public MetadataOptions MetadataOptions { get; set; } = new();

		public OperationOptions OperationOptions { get; set; } = new();

		internal OutputImageInfo? OutputImageInfo { get; set; }

		public PngOptions? PngOptions { get; set; }

		public RawOptions? RawOptions { get; set; }

		public ResizeOptions ResizeOptions { get; set; } = new();

		public RotationOptions RotationOptions { get; set; } = new();

		public SharpenOptions? SharpenOptions { get; set; }

		public ToBufferOptions? ToBufferOptions { get; set; }

		public ToFileOptions? ToFileOptions { get; set; }

		public ToStreamOptions? ToStreamOptions { get; set; }

		public TrimOptions TrimOptions { get; set; } = new();

		public WebpOptions? WebpOptions { get; set; }

		public int Width { get; set; }
	}
}
