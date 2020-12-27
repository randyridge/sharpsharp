using NetVips;

namespace SharpSharp.Pipeline {
	internal sealed class PipelineBaton {
		public AnimationOptions AnimationOptions { get; set; } = new();

		public int Channels { get; set; }

		public CropOffsetOptions CropOffsetOptions { get; set; } = new();

		public GifOptions? GifOptions { get; set; } = null;

		public HeifOptions? HeifOptions { get; set; } = null;

		public int Height { get; set; } = -1;

		public Image? Image { get; set; } = null;

		public ImageType? InputImageType { get; set; } = null;

		public JpegOptions? JpegOptions { get; set; } = null;

		public MetadataOptions MetadataOptions { get; set; } = new();

		public OperationOptions OperationOptions { get; set; } = new();

		internal OutputImageInfo OutputImageInfo { get; set; } = new();

		public PngOptions? PngOptions { get; set; } = null;

		public RawOptions? RawOptions { get; set; } = null;

		public ResizeOptions ResizeOptions { get; set; } = new();

		public RotationOptions RotationOptions { get; set; } = new();

		public SharpenOptions SharpenOptions { get; set; } = new();

		public TiffOptions? TiffOptions { get; set; } = null;

		public ToBufferOptions? ToBufferOptions { get; set; } = null;

		public ToFileOptions? ToFileOptions { get; set; } = null;

		public ToStreamOptions? ToStreamOptions { get; set; } = null;

		public TrimOptions TrimOptions { get; set; } = new();

		public WebpOptions? WebpOptions { get; set; } = null;

		public int Width { get; set; } = -1;
	}
}
