using System;
using RandyRidge.Common;
using SharpSharp.Pipeline;

//using SharpSharp.Pipeline.Operations;

namespace SharpSharp {
    public sealed partial class ImagePipeline {
        private readonly PipelineBaton result;
        private readonly ImageSource imageSource;
        private readonly Processor processor;

        private void Execute() {
            processor.Process(imageSource, result);
        }

        //private void AddOperationAndExecute(IOperation operation) {
        //    Guard.ArgumentNotNull(operation, nameof(operation));
        //    operations.Add(operation);
        //    Execute();
        //}

        //private ImagePipeline AddOperationAndReturn(IOperation operation) {
        //    Guard.ArgumentNotNull(operation, nameof(operation));
        //    if(operations.Any(x => x.GetType() == operation.GetType())) {
        //        throw new SharpSharpException("Duplicate operation added.");
        //    }
        //    operations.Add(operation);
        //    return this;
        //}

        private ImagePipeline(ImageSource imageSource) {
            this.imageSource = Guard.ArgumentNotNull(imageSource, nameof(imageSource));
            result = new PipelineBaton();
            processor = new Processor();
        }

        public static Version VipsVersion => new Version(NetVips.NetVips.Version(0), NetVips.NetVips.Version(1), NetVips.NetVips.Version(2));
    }
}
