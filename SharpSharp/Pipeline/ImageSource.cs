using System.Globalization;
using NetVips;

namespace SharpSharp.Pipeline {
    internal abstract class ImageSource {
        protected ImageSource(ImageLoadOptions options) {
            Options = options;
        }

        public ImageLoadOptions Options { get; }

        public abstract (Image Image, ImageType ImageType) Load(VOption? options = null);

        internal VOption BuildLoadOptionsFromImageType(ImageType imageType) {
            var result = new VOption();

            if(imageType == ImageType.Svg || imageType == ImageType.Pdf) {
                result.Add("dpi", Options.Density);
            }
            else if(imageType == ImageType.Magick) {
                result.Add("density", Options.Density.ToString(CultureInfo.InvariantCulture));
            }

            if(imageType.SupportsPages) {
                result.Add("n", Options.PageCount);
                result.Add("page", Options.PageIndex);
            }

            return result;
        }
    }
}
