namespace SharpSharp {
    public sealed partial class ImagePipeline {
        //public ImagePipeline Extend(int pixels) => Extend(new ExtendOptions(pixels));

        //public ImagePipeline Extend(ExtendOptions extendOptions) => AddOperationAndReturn(new ExtendOperation(extendOptions));

        public ImagePipeline Resize(int? width, int? height) => Resize(new ResizeOptions(width, height));

        public ImagePipeline Resize(ResizeOptions resizeOptions) {
            result.ResizeOptions = resizeOptions;
            return this;
        }

        //public ImagePipeline Trim(int threshold = 10) => AddOperationAndReturn(new TrimOperation(threshold));
    }
}
