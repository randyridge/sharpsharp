using SharpSharp.Pipeline;

namespace SharpSharp {
    public sealed class OutputImageInfo {
        public int Channels { get; internal set; }

        public string Format { get; internal set; } = "";

        public int Height { get; internal set; }

        public int Size { get; internal set; }

        public int Width { get; internal set; }
    }
}
