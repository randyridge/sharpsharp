
namespace SharpSharp {
    public sealed partial class ImagePipeline {
        public ImagePipeline RemoveAlpha() {
            if(result.ChannelOptions == null) {
                result.ChannelOptions = new ChannelOptions();
            }

            result.ChannelOptions.RemoveAlpha = true;
            return this;
        }

//        public ImagePipeline EnsureAlpha() {
//            options.EnsureAlpha = true;
//            return this;
//        }

//        public ImagePipeline ExtractChannel(string channel) {
//            Guard.NotNullOrWhiteSpace(channel, nameof(channel));
//            if(string.Equals(channel, "red", StringComparison.OrdinalIgnoreCase)) {
//                options.ExtractChannel = 0;
//                return this;
//            }
//            if(string.Equals(channel, "green", StringComparison.OrdinalIgnoreCase)) {
//                options.ExtractChannel = 1;
//                return this;
//            }
//            if(string.Equals(channel, "blue", StringComparison.OrdinalIgnoreCase)) {
//                options.ExtractChannel = 2;
//                return this;
//            }

//            throw new ArgumentOutOfRangeException(nameof(channel), "Must be 'red', 'green', or 'blue'.");
//        }

//        public ImagePipeline ExtractChannel(int channel) {
//            if(channel < 0 || channel > 4) {
//                throw new ArgumentOutOfRangeException(nameof(channel), "Must be 0 to 4.");
//            }

//            options.ExtractChannel = channel;
//            return this;
//        }

//        // TODO: JoinChannel
//        public ImagePipeline JoinChannel() {
//            return this;
//        }

//        public ImagePipeline BandBool(string boolOp) {
//            if(string.Equals(boolOp, "and", StringComparison.OrdinalIgnoreCase)) {
//                options.BandBoolOp = boolOp;
//            }
//            if(string.Equals(boolOp, "or", StringComparison.OrdinalIgnoreCase)) {
//                options.BandBoolOp = boolOp;
//            }
//            if(string.Equals(boolOp, "eor", StringComparison.OrdinalIgnoreCase)) {
//                options.BandBoolOp = boolOp;
//            }
//            throw new ArgumentOutOfRangeException(nameof(boolOp), "Must be 'and', 'or', or 'eor'.");
//        }
    }
}
