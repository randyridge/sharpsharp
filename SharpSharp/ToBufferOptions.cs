using System;

namespace SharpSharp {
    public sealed class ToBufferOptions {
        public ToBufferOptions(Action<byte[]> callback) {
            Callback = callback;
        }

        public Action<byte[]> Callback { get; }
    }
}
