using System;
using RandyRidge.Common;

namespace SharpSharp {
    public sealed partial class ImagePipeline {
        //public ImagePipeline Extend(int pixels) => Extend(new ExtendOptions(pixels));

        //public ImagePipeline Extend(ExtendOptions extendOptions) => AddOperationAndReturn(new ExtendOperation(extendOptions));

        /// <summary>
        ///     Resizes an image to the specified dimensions.
        /// </summary>
        /// <param name="width">
        ///     The expected image width.
        /// </param>
        /// <param name="height">
        ///     The expected image height.
        /// </param>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        public ImagePipeline Resize(int? width, int? height) => Resize(new ResizeOptions(width, height));

        /// <summary>
        ///     Resizes an image with the specified options.
        /// </summary>
        /// <param name="resizeOptions">
        ///     The <see cref="ResizeOptions" /> to use.
        /// </param>
        /// <returns>
        ///     The image pipeline.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="resizeOptions" /> is null.
        /// </exception>
        public ImagePipeline Resize(ResizeOptions resizeOptions) {
            Guard.NotNull(resizeOptions, nameof(resizeOptions));
            result.ResizeOptions = resizeOptions;
            return this;
        }

        //public ImagePipeline Trim(int threshold = 10) => AddOperationAndReturn(new TrimOperation(threshold));
    }
}
