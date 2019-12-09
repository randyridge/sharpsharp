namespace SharpSharp {
    public sealed class Metadata {
        public int Channels { get; set; }

        public string? ChromaSubsampling { get; set; }

        public int Density { get; set; }

        public string? Depth { get; set; }

        public string? Exif { get; set; }

        public string? Format { get; set; }

        public bool HasAlpha { get; set; }

        public bool HasProfile { get; set; }

        public int Height { get; set; }

        public string? Icc { get; set; }

        public string? Iptc { get; set; }

        public bool IsProgressive { get; set; }

        public int Orientation { get; set; }

        public int PageHeight { get; set; }

        public int PagePrimary { get; set; } = -1;

        public int Pages { get; set; }

        public int PaletteBitDepth { get; set; }

        public string? Space { get; set; }

        public int Width { get; set; }

        public string? Xmp { get; set; }
    }
}
