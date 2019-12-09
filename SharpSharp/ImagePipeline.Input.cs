using System;
using System.Collections.Generic;
using System.Globalization;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
    public sealed partial class ImagePipeline {
        public ImagePipeline Metadata(Action<Metadata> callback) {
            Guard.ArgumentNotNull(callback, nameof(callback));

            var (image, imageType) = imageSource.Load();

            if(imageType == ImageType.Unknown) {
                // TODO: ?
            }

            var metadata = new Metadata {
                Format = imageType.ImageTypeId,
                Width = image.Width,
                Height = image.Height,
                Space = image.Interpretation,
                Channels = image.Bands,
                Depth = image.Format,
                HasProfile = image.HasProfile(),
                HasAlpha = image.HasAlpha(),
                Orientation = image.ExifOrientation()
            };

            if(image.HasDensity()) {
                metadata.Density = image.GetDensity();
            }

            if(image.GetTypeOf("jpeg-chroma-subsample") == GValue.RefStrType) {
                metadata.ChromaSubsampling = image.Get("jpeg-chroma-subsample") as string;
            }

            if(image.GetTypeOf("interlaced") == GValue.GIntType) {
                metadata.IsProgressive = Convert.ToInt32(image.Get("interlaced"), CultureInfo.InvariantCulture) == 1;
            }

            if(image.GetTypeOf("palette-bit-depth") == GValue.GIntType) {
                metadata.PaletteBitDepth = Convert.ToInt32(image.Get("palette-bit-depth"), CultureInfo.InvariantCulture);
            }

            if(image.GetTypeOf("n-pages") == GValue.GIntType) {
                metadata.Pages = Convert.ToInt32(image.Get("n-pages"), CultureInfo.InvariantCulture);
            }

            if(image.GetTypeOf("page-height") == GValue.GIntType) {
                metadata.PageHeight = Convert.ToInt32(image.Get("page-height"), CultureInfo.InvariantCulture);
            }

            if(image.GetTypeOf("heif-primary") == GValue.GIntType) {
                metadata.PagePrimary = Convert.ToInt32(image.Get("heif-primary"), CultureInfo.InvariantCulture);
            }

            // EXIF
            if(image.GetTypeOf("exif-data") == GValue.BlobType) {
                metadata.Exif = image.Get("exif-data") as string;
            }

            // ICC profile
            if(image.GetTypeOf("icc-profile-data") == GValue.BlobType) {
                metadata.Icc = image.Get("icc-profile-data") as string;
            }

            // IPTC
            if(image.GetTypeOf("iptc-data") == GValue.BlobType) {
                metadata.Iptc = image.Get("iptc-data") as string;
            }

            // XMP
            if(image.GetTypeOf("xmp-data") == GValue.BlobType) {
                metadata.Xmp = image.Get("xmp-data") as string;
            }

            image?.Dispose();

            callback(metadata);

            return this;
        }

        public ImagePipeline Stats(Action<Stats> callback) {
            Guard.ArgumentNotNull(callback, nameof(callback));
            const int STAT_MIN_INDEX = 0;
            const int STAT_MAX_INDEX = 1;
            const int STAT_SUM_INDEX = 2;
            const int STAT_SQ_SUM_INDEX = 3;
            const int STAT_MEAN_INDEX = 4;
            const int STAT_STDEV_INDEX = 5;
            const int STAT_MINX_INDEX = 6;
            const int STAT_MINY_INDEX = 7;
            const int STAT_MAXX_INDEX = 8;
            const int STAT_MAXY_INDEX = 9;

            var (image, imageType) = imageSource.Load();

            if(imageType == ImageType.Unknown) {
                // TODO: ?
            }

            var stats = new Stats {
                Entropy = Math.Abs(image.Colourspace(Enums.Interpretation.Bw)[0].HistFind().HistEntropy()),
                IsOpaque = true
            };

            var vipsStats = image.Stats();
            var bands = image.Bands;
            var channelStats = new List<ChannelStats>(bands);
            for(var band = 1; band <= bands; band++) {
                channelStats.Add(new ChannelStats(
                    (int) vipsStats.Getpoint(STAT_MIN_INDEX, band)[0],
                    (int) vipsStats.Getpoint(STAT_MAX_INDEX, band)[0],
                    vipsStats.Getpoint(STAT_SUM_INDEX, band)[0],
                    vipsStats.Getpoint(STAT_SQ_SUM_INDEX, band)[0],
                    vipsStats.Getpoint(STAT_MEAN_INDEX, band)[0],
                    vipsStats.Getpoint(STAT_STDEV_INDEX, band)[0],
                    (int) vipsStats.Getpoint(STAT_MINX_INDEX, band)[0],
                    (int) vipsStats.Getpoint(STAT_MINY_INDEX, band)[0],
                    (int) vipsStats.Getpoint(STAT_MAXX_INDEX, band)[0],
                    (int) vipsStats.Getpoint(STAT_MAXY_INDEX, band)[0]
                ));
            }

            stats.ChannelStats = channelStats.ToArray();

            if(image.HasAlpha()) {
                var minAlpha = vipsStats.Getpoint(STAT_MIN_INDEX, bands)[0];
                if(!minAlpha.IsAboutEqualTo(image.Interpretation.MaximumImageAlpha())) {
                    stats.IsOpaque = false;
                }
            }

            image?.Dispose();
            callback(stats);
            return this;
        }
//        public ImagePipeline SequentialRead(bool useSequentialRead = true) {
//            // TODO: this
//            //options.SequentialRead = useSequentialRead;
//            return this;
//        }

//        public ImagePipeline LimitInputPixels(bool useLimit) {
//            if(useLimit) {
//                options.LimitInputPixels = (int) Math.Pow(0x3FFF, 2);
//            }
//            else {
//                options.LimitInputPixels = 0;
//            }

//            return this;
//        }

//        public ImagePipeline LimitInputPixels(int limit) {
//            if(limit < 0) {
//                throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");
//            }

//            options.LimitInputPixels = limit;

//            return this;
//        }

        public ImagePipeline TouchAllPixels() {
            var (image, _) = imageSource.Load();
            image.Avg();
            image?.Dispose();
            return this;
        }
    }
}
