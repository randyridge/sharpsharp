using RandyRidge.Common;

namespace SharpSharp {
	public sealed partial class ImagePipeline {
//        public ImagePipeline Tint(string color) {
//            var c = Color.Parse(color);
//            options.TintA = c.A;
//            options.TintB = c.B;
//            return this;
//        }

		public ImagePipeline Grayscale(bool makeGrayscale = true) {
			if(result.ColorizationOptions.HasValue()) {
				result.ColorizationOptions.MakeGrayscale = makeGrayscale;
			}
			else {
				result.ColorizationOptions = new ColorizationOptions(makeGrayscale);
			}

			return this;
		}

//        public ImagePipeline ToColorspace(string colorspace) {
//            Guard.NotNullOrWhiteSpace(colorspace, nameof(colorspace));
//            // TODO: valid values
//            options.Colorspace = colorspace;
//            return this;
//        }

//        public ImagePipeline SetBackgroundColorOption() {
//            // TODO: this
//            return this;
//        }
	}
}
