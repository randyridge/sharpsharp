using NetVips;

namespace SharpSharp.Pipeline {
    internal sealed class PipelineBaton {
        public PipelineBaton() {
            MetadataOptions = new MetadataOptions();
            ResizeOptions = new ResizeOptions();
        }

        public ChannelOptions? ChannelOptions { get; set; }

        public int Channels { get; set; }

        public ColorizationOptions? ColorizationOptions { get; set; }

        public HeifOptions? HeifOptions { get; set; }

        public int Height { get; set; }

        public Image? Image { get; set; }

        public ImageType? InputImageType { get; set; }

        public JpegOptions? JpegOptions { get; set; }

        public MetadataOptions MetadataOptions { get; set; }

        public OperationOptions? OperationOptions { get; set; }

        public PngOptions? PngOptions { get; set; }

        public RawOptions? RawOptions { get; set; }

        public ResizeOptions ResizeOptions { get; set; }

        public SharpenOptions? SharpenOptions { get; set; }

        public ToBufferOptions? ToBufferOptions { get; set; }

        public ToFileOptions? ToFileOptions { get; set; }

        public WebpOptions? WebpOptions { get; set; }

        public int Width { get; set; }
    }
}
