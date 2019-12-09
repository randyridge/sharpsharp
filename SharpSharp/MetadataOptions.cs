namespace SharpSharp {
    public sealed class MetadataOptions {
        public MetadataOptions(bool includeMetadata = false, Orientation? orientation = null) {
            IncludeMetadata = includeMetadata;
            Orientation = orientation;
        }

        public bool IncludeMetadata { get; }

        public Orientation? Orientation { get; }

        public bool ShouldStripMetadata => !IncludeMetadata;
    }
}
