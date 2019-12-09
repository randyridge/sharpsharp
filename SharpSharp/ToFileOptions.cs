using System;
using RandyRidge.Common;

namespace SharpSharp {
    public sealed class ToFileOptions {
        public ToFileOptions(string filePath, Action<ImageInfo>? callback) {
            FilePath = Guard.ArgumentNotNullOrWhiteSpace(filePath, nameof(filePath));
            Callback = callback;
        }

        public Action<ImageInfo>? Callback { get; }

        public string FilePath { get; }
    }
}
