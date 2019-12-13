using System;
using System.IO;
using RandyRidge.Common;

namespace SharpSharp {
    public sealed class ToStreamOptions {
        public ToStreamOptions(Stream stream, Action<OutputImageInfo>? callback) {
            Stream = Guard.ArgumentNotNull(stream, nameof(stream));
            Callback = callback;
        }

        public Action<OutputImageInfo>? Callback { get; }

        public Stream Stream { get; }
    }
}
