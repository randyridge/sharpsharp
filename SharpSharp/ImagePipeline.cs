using System;
using RandyRidge.Common;
using SharpSharp.Pipeline;

//using SharpSharp.Pipeline.Operations;

namespace SharpSharp {
    /// <summary>
    ///     Represents an image pipeline.
    /// </summary>
    public sealed partial class ImagePipeline {
        private readonly ImageSource imageSource;
        private readonly Processor processor;
        private readonly PipelineBaton result;

        //private void AddOperationAndExecute(IOperation operation) {
        //    Guard.NotNull(operation, nameof(operation));
        //    operations.Add(operation);
        //    Execute();
        //}

        //private ImagePipeline AddOperationAndReturn(IOperation operation) {
        //    Guard.NotNull(operation, nameof(operation));
        //    if(operations.Any(x => x.GetType() == operation.GetType())) {
        //        throw new SharpSharpException("Duplicate operation added.");
        //    }
        //    operations.Add(operation);
        //    return this;
        //}

        private ImagePipeline(ImageSource imageSource) {
            this.imageSource = Guard.NotNull(imageSource, nameof(imageSource));
            result = new PipelineBaton();
            processor = new Processor();
        }

        /// <summary>
        ///     Returns the current version of vips.
        /// </summary>
        public static Version VipsVersion { get; } = new Version(NetVips.NetVips.Version(0), NetVips.NetVips.Version(1), NetVips.NetVips.Version(2));

        private void Execute() {
            processor.Process(imageSource, result);
        }
    }
}
