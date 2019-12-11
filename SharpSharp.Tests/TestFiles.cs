using System.IO;

namespace SharpSharp {
    public static class TestFiles {
        public static readonly string InputJpgWithLandscapeExif1 = GetPath("Landscape_1.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif2 = GetPath("Landscape_2.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif3 = GetPath("Landscape_3.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif4 = GetPath("Landscape_4.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif5 = GetPath("Landscape_5.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif6 = GetPath("Landscape_6.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif7 = GetPath("Landscape_7.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif8 = GetPath("Landscape_8.jpg"); // https://github.com/recurser/exif-orientation-examples

        public static readonly string InputJpgWithPortraitExif1 = GetPath("Portrait_1.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif2 = GetPath("Portrait_2.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif3 = GetPath("Portrait_3.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif4 = GetPath("Portrait_4.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif5 = GetPath("Portrait_5.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif6 = GetPath("Portrait_6.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif7 = GetPath("Portrait_7.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif8 = GetPath("Portrait_8.jpg"); // https://github.com/recurser/exif-orientation-examples

        public static readonly string InputJpg = GetPath("2569067123_aca715a2ee_o.jpg"); // http://www.flickr.com/photos/grizdave/2569067123/
        public static readonly string InputJpgWithExif = GetPath("Landscape_8.jpg"); // https://github.com/recurser/exif-orientation-examples/blob/master/Landscape_8.jpg
        public static readonly string InputJpgWithIptcAndXmp = GetPath("Landscape_9.jpg"); // https://unsplash.com/photos/RWAIyGmgHTQ
        public static readonly string InputJpgWithExifMirroring = GetPath("Landscape_5.jpg"); // https://github.com/recurser/exif-orientation-examples/blob/master/Landscape_5.jpg
        public static readonly string InputJpgWithGammaHoliness = GetPath("gamma_dalai_lama_gray.jpg"); // http://www.4p8.com/eric.brasseur/gamma.html
        public static readonly string InputJpgWithCmykProfile = GetPath("Channel_digital_image_CMYK_color.jpg"); // http://en.wikipedia.org/wiki/File:Channel_digital_image_CMYK_color.jpg
        public static readonly string InputJpgWithCmykNoProfile = GetPath("Channel_digital_image_CMYK_color_no_profile.jpg");
        public static readonly string InputJpgWithCorruptHeader = GetPath("corrupt-header.jpg");
        public static readonly string InputJpgWithLowContrast = GetPath("low-contrast.jpg"); // http://www.flickr.com/photos/grizdave/2569067123/
        public static readonly string InputJpgLarge = GetPath("giant-image.jpg");
        public static readonly string InputJpg320x240 = GetPath("320x240.jpg"); // http://www.andrewault.net/2010/01/26/create-a-test-pattern-video-with-perl/
        public static readonly string InputJpgOverlayLayer2 = GetPath("alpha-layer-2-ink.jpg");
        public static readonly string InputJpgTruncated = GetPath("truncated.jpg"); // head -c 10000 2569067123_aca715a2ee_o.jpg > truncated.jpg
        public static readonly string InputJpgCenteredImage = GetPath("centered_image.jpeg");
        public static readonly string InputJpgRandom = GetPath("random.jpg"); // convert -size 200x200 xc:   +noise Random   random.jpg
        public static readonly string InputJpgThRandom = GetPath("thRandom.jpg"); // convert random.jpg  -channel G -threshold 5% -separate +channel -negate thRandom.jpg
        public static readonly string InputJpgLossless = GetPath("testimgl.jpg"); // Lossless JPEG from ftp://ftp.fu-berlin.de/unix/X11/graphics/ImageMagick/delegates/ljpeg-6b.tar.gz

        public static readonly string InputPng = GetPath("50020484-00001.png"); // http://c.searspartsdirect.com/lis_png/PLDM/50020484-00001.png
        public static readonly string InputPngWithTransparency = GetPath("blackbug.png"); // public domain
        public static readonly string InputPngCompleteTransparency = GetPath("full-transparent.png");
        public static readonly string InputPngWithGreyAlpha = GetPath("grey-8bit-alpha.png");
        public static readonly string InputPngWithOneColor = GetPath("2x2_fdcce6.png");
        public static readonly string InputPngWithTransparency16bit = GetPath("tbgn2c16.png"); // http://www.schaik.com/pngsuite/tbgn2c16.png
        public static readonly string InputPngOverlayLayer0 = GetPath("alpha-layer-0-background.png");
        public static readonly string InputPngOverlayLayer1 = GetPath("alpha-layer-1-fill.png");
        public static readonly string InputPngAlphaPremultiplicationSmall = GetPath("alpha-premultiply-1024x768-paper.png");
        public static readonly string InputPngAlphaPremultiplicationLarge = GetPath("alpha-premultiply-2048x1536-paper.png");
        public static readonly string InputPngBooleanNoAlpha = GetPath("bandbool.png");
        public static readonly string InputPngTestJoinChannel = GetPath("testJoinChannel.png");
        public static readonly string InputPngTruncated = GetPath("truncated.png"); // gm convert 2569067123_aca715a2ee_o.jpg -resize 320x240 saw.png ; head -c 10000 saw.png > truncated.png
        public static readonly string InputPngEmbed = GetPath("embedgravitybird.png"); // Released to sharp under a CC BY 4.0
        public static readonly string InputPngRGBWithAlpha = GetPath("2569067123_aca715a2ee_o.png"); // http://www.flickr.com/photos/grizdave/2569067123/ (same as inputJpg)
        public static readonly string InputPngImageInAlpha = GetPath("image-in-alpha.png"); // https://github.com/lovell/sharp/issues/1597

        public static readonly string InputWebP = GetPath("4.webp"); // http://www.gstatic.com/webp/gallery/4.webp
        public static readonly string InputWebPWithTransparency = GetPath("5_webp_a.webp"); // http://www.gstatic.com/webp/gallery3/5_webp_a.webp
        public static readonly string InputTiff = GetPath("G31D.TIF"); // http://www.fileformat.info/format/tiff/sample/e6c9a6e5253348f4aef6d17b534360ab/index.htm
        public static readonly string InputTiffMultipage = GetPath("G31D_MULTI.TIF"); // gm convert G31D.TIF -resize 50% G31D_2.TIF ; tiffcp G31D.TIF G31D_2.TIF G31D_MULTI.TIF
        public static readonly string InputTiffCielab = GetPath("cielab-dagams.tiff"); // https://github.com/lovell/sharp/issues/646
        public static readonly string InputTiffUncompressed = GetPath("uncompressed_tiff.tiff"); // https://code.google.com/archive/p/imagetestsuite/wikis/TIFFTestSuite.wiki file: 0c84d07e1b22b76f24cccc70d8788e4a.tif
        public static readonly string InputTiff8BitDepth = GetPath("8bit_depth.tiff");
        public static readonly string InputTifftagPhotoshop = GetPath("tifftag-photoshop.tiff"); // https://github.com/lovell/sharp/issues/1600
        public static readonly string InputGif = GetPath("Crash_test.gif"); // http://upload.wikimedia.org/wikipedia/commons/e/e3/Crash_test.gif
        public static readonly string InputGifGreyPlusAlpha = GetPath("grey-plus-alpha.gif"); // http://i.imgur.com/gZ5jlmE.gif
        public static readonly string InputGifAnimated = GetPath("rotating-squares.gif"); // CC0 https://loading.io/spinner/blocks/-rotating-squares-preloader-gif
        public static readonly string InputSvg = GetPath("check.svg"); // http://dev.w3.org/SVG/tools/svgweb/samples/svg-files/check.svg
        public static readonly string InputSvgWithEmbeddedImages = GetPath("struct-image-04-t.svg"); // https://dev.w3.org/SVG/profiles/1.2T/test/svg/struct-image-04-t.svg

        public static readonly string InputJPGBig = GetPath("flowers.jpeg");

        public static readonly string InputPngStripesV = GetPath("stripesV.png");
        public static readonly string InputPngStripesH = GetPath("stripesH.png");

        public static readonly string InputJpgBooleanTest = GetPath("booleanTest.jpg");

        public static readonly string InputV = GetPath("vfile.v");

        //public static readonly string OutputJpg = GetPath("output.jpg");
        //public static readonly string OutputPng = GetPath("output.png");
        //public static readonly string OutputWebP = GetPath("output.webp");
        //public static readonly string OutputV = GetPath("output.v");
        //public static readonly string OutputTiff = GetPath("output.tiff");
        //public static readonly string OutputZoinks = GetPath("output.zoinks"); // an "unknown" file extension

        public static readonly string TestPattern = GetPath("test-pattern.png");

        private static string GetPath(string fileName) => Path.Join(@".\", fileName);

    }
}
