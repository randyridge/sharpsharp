using System;
using RandyRidge.Common;

namespace SharpSharp {
    /// <summary>
    ///     Options for writing image to a file.
    /// </summary>
    public sealed class ToFileOptions {
        /// <summary>
        ///     Initializes an instance of the options for writing to a file.
        /// </summary>
        /// <param name="filePath">
        ///     The path to the file to write to.
        /// </param>
        /// <param name="callback">
        ///     The call back, if any,  to call with the <see cref="OutputImageInfo" /> for the image written.
        /// </param>
        public ToFileOptions(string filePath, Action<OutputImageInfo>? callback) {
            FilePath = Guard.NotNullOrWhiteSpace(filePath, nameof(filePath));
            Callback = callback;
        }

        /// <summary>
        ///     Optional callback to get <see cref="OutputImageInfo" />.
        /// </summary>
        public Action<OutputImageInfo>? Callback { get; }

        /// <summary>
        ///     The file path to write to.
        /// </summary>
        public string FilePath { get; }
    }
}
