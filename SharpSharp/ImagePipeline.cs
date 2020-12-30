using System;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
	/// <summary>
	///   Represents an image pipeline.
	/// </summary>
	public sealed partial class ImagePipeline {
		private readonly ImageSource imageSource;
		private readonly Processor processor;
		private readonly PipelineBaton baton;

		static ImagePipeline() {
			ModuleInitializer.Initialize();
			if(!ModuleInitializer.VipsInitialized) {
				throw new SharpSharpException("Failed to initialize NetVips.", ModuleInitializer.Exception);
			}
		}

		// Called by the From* methods in the partial ImagePipeline.Constructor
		private ImagePipeline(ImageSource imageSource) {
			this.imageSource = Guard.NotNull(imageSource, nameof(imageSource));
			baton = new PipelineBaton();
			processor = new Processor();
		}

		/// <summary>
		///   Returns the current version of vips.
		/// </summary>
		public static Version VipsVersion { get; } = new(NetVips.NetVips.Version(0), NetVips.NetVips.Version(1), NetVips.NetVips.Version(2));

		// Called by the To* methods in the partial ImagePipeline.Output
		private void Execute() {
			processor.Process(imageSource, baton);
		}
	}
}
