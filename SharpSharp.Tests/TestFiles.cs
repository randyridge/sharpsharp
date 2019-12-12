using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandyRidge.Common;
using RandyRidge.Common.IO;
using RandyRidge.Common.Reflection;
using Shouldly;
using Xunit;

namespace SharpSharp {
    public static class TestFiles {
        public static class AllFilesLoadable {
            [Fact]
            public static void can_locate_all_files() {
                GetInputFilePaths().ForEach(path => File.Exists(path).ShouldBeTrue($"Couldn't find {path}."));

                static IEnumerable<string> GetInputFilePaths() {
                    return typeof(TestFiles).GetPublicStaticProperties(x => x.PropertyType == typeof(string)).Select(property => property?.GetValue(null) as string);
                }
            }
        }

        // @formatter:off
        public static string InputJpgWithLandscapeExif1 { get; } = GetInputFilePath("Landscape_1.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithLandscapeExif2 { get; } = GetInputFilePath("Landscape_2.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithLandscapeExif3 { get; } = GetInputFilePath("Landscape_3.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithLandscapeExif4 { get; } = GetInputFilePath("Landscape_4.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithLandscapeExif5 { get; } = GetInputFilePath("Landscape_5.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithLandscapeExif6 { get; } = GetInputFilePath("Landscape_6.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithLandscapeExif7 { get; } = GetInputFilePath("Landscape_7.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithLandscapeExif8 { get; } = GetInputFilePath("Landscape_8.jpg"); // https://github.com/recurser/exif-orientation-examples

        public static string InputJpgWithPortraitExif1 { get; } = GetInputFilePath("Portrait_1.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithPortraitExif2 { get; } = GetInputFilePath("Portrait_2.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithPortraitExif3 { get; } = GetInputFilePath("Portrait_3.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithPortraitExif4 { get; } = GetInputFilePath("Portrait_4.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithPortraitExif5 { get; } = GetInputFilePath("Portrait_5.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithPortraitExif6 { get; } = GetInputFilePath("Portrait_6.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithPortraitExif7 { get; } = GetInputFilePath("Portrait_7.jpg"); // https://github.com/recurser/exif-orientation-examples
        public static string InputJpgWithPortraitExif8 { get; } = GetInputFilePath("Portrait_8.jpg"); // https://github.com/recurser/exif-orientation-examples

        public static string InputJpg { get; } = GetInputFilePath("2569067123_aca715a2ee_o.jpg"); // http://www.flickr.com/photos/grizdave/2569067123/
        public static string InputJpgWithExif { get; } = GetInputFilePath("Landscape_8.jpg"); // https://github.com/recurser/exif-orientation-examples/blob/master/Landscape_8.jpg
        public static string InputJpgWithIptcAndXmp { get; } = GetInputFilePath("Landscape_9.jpg"); // https://unsplash.com/photos/RWAIyGmgHTQ
        public static string InputJpgWithExifMirroring { get; } = GetInputFilePath("Landscape_5.jpg"); // https://github.com/recurser/exif-orientation-examples/blob/master/Landscape_5.jpg
        public static string InputJpgWithGammaHoliness { get; } = GetInputFilePath("gamma_dalai_lama_gray.jpg"); // http://www.4p8.com/eric.brasseur/gamma.html
        public static string InputJpgWithCmykProfile { get; } = GetInputFilePath("Channel_digital_image_CMYK_color.jpg"); // http://en.wikipedia.org/wiki/File:Channel_digital_image_CMYK_color.jpg
        public static string InputJpgWithCmykNoProfile { get; } = GetInputFilePath("Channel_digital_image_CMYK_color_no_profile.jpg");
        public static string InputJpgWithCorruptHeader { get; } = GetInputFilePath("corrupt-header.jpg");
        public static string InputJpgWithLowContrast { get; } = GetInputFilePath("low-contrast.jpg"); // http://www.flickr.com/photos/grizdave/2569067123/
        public static string InputJpgLarge { get; } = GetInputFilePath("giant-image.jpg");
        public static string InputJpg320x240 { get; } = GetInputFilePath("320x240.jpg"); // http://www.andrewault.net/2010/01/26/create-a-test-pattern-video-with-perl/
        public static string InputJpgOverlayLayer2 { get; } = GetInputFilePath("alpha-layer-2-ink.jpg");
        public static string InputJpgTruncated { get; } = GetInputFilePath("truncated.jpg"); // head -c 10000 2569067123_aca715a2ee_o.jpg > truncated.jpg
        public static string InputJpgCenteredImage { get; } = GetInputFilePath("centered_image.jpeg");
        public static string InputJpgRandom { get; } = GetInputFilePath("random.jpg"); // convert -size 200x200 xc:   +noise Random   random.jpg
        public static string InputJpgThRandom { get; } = GetInputFilePath("thRandom.jpg"); // convert random.jpg  -channel G -threshold 5% -separate +channel -negate thRandom.jpg
        public static string InputJpgLossless { get; } = GetInputFilePath("testimgl.jpg"); // Lossless JPEG from ftp://ftp.fu-berlin.de/unix/X11/graphics/ImageMagick/delegates/ljpeg-6b.tar.gz

        public static string InputPng { get; } = GetInputFilePath("50020484-00001.png"); // http://c.searspartsdirect.com/lis_png/PLDM/50020484-00001.png
        public static string InputPngWithTransparency { get; } = GetInputFilePath("blackbug.png"); // public domain
        public static string InputPngCompleteTransparency { get; } = GetInputFilePath("full-transparent.png");
        public static string InputPngWithGreyAlpha { get; } = GetInputFilePath("grey-8bit-alpha.png");
        public static string InputPngWithOneColor { get; } = GetInputFilePath("2x2_fdcce6.png");
        public static string InputPngWithTransparency16bit { get; } = GetInputFilePath("tbgn2c16.png"); // http://www.schaik.com/pngsuite/tbgn2c16.png
        public static string InputPngOverlayLayer0 { get; } = GetInputFilePath("alpha-layer-0-background.png");
        public static string InputPngOverlayLayer1 { get; } = GetInputFilePath("alpha-layer-1-fill.png");
        public static string InputPngAlphaPremultiplicationSmall { get; } = GetInputFilePath("alpha-premultiply-1024x768-paper.png");
        public static string InputPngAlphaPremultiplicationLarge { get; } = GetInputFilePath("alpha-premultiply-2048x1536-paper.png");
        public static string InputPngBooleanNoAlpha { get; } = GetInputFilePath("bandbool.png");
        public static string InputPngTestJoinChannel { get; } = GetInputFilePath("testJoinChannel.png");
        public static string InputPngTruncated { get; } = GetInputFilePath("truncated.png"); // gm convert 2569067123_aca715a2ee_o.jpg -resize 320x240 saw.png ; head -c 10000 saw.png > truncated.png
        public static string InputPngEmbed { get; } = GetInputFilePath("embedgravitybird.png"); // Released to sharp under a CC BY 4.0
        public static string InputPngRGBWithAlpha { get; } = GetInputFilePath("2569067123_aca715a2ee_o.png"); // http://www.flickr.com/photos/grizdave/2569067123/ (same as inputJpg)
        public static string InputPngImageInAlpha { get; } = GetInputFilePath("image-in-alpha.png"); // https://github.com/lovell/sharp/issues/1597

        public static string InputWebP { get; } = GetInputFilePath("4.webp"); // http://www.gstatic.com/webp/gallery/4.webp
        public static string InputWebPWithTransparency { get; } = GetInputFilePath("5_webp_a.webp"); // http://www.gstatic.com/webp/gallery3/5_webp_a.webp
        public static string InputTiff { get; } = GetInputFilePath("G31D.TIF"); // http://www.fileformat.info/format/tiff/sample/e6c9a6e5253348f4aef6d17b534360ab/index.htm
        public static string InputTiffMultipage { get; } = GetInputFilePath("G31D_MULTI.TIF"); // gm convert G31D.TIF -resize 50% G31D_2.TIF ; tiffcp G31D.TIF G31D_2.TIF G31D_MULTI.TIF
        public static string InputTiffCielab { get; } = GetInputFilePath("cielab-dagams.tiff"); // https://github.com/lovell/sharp/issues/646
        public static string InputTiffUncompressed { get; } = GetInputFilePath("uncompressed_tiff.tiff"); // https://code.google.com/archive/p/imagetestsuite/wikis/TIFFTestSuite.wiki file: 0c84d07e1b22b76f24cccc70d8788e4a.tif
        public static string InputTiff8BitDepth { get; } = GetInputFilePath("8bit_depth.tiff");
        public static string InputTifftagPhotoshop { get; } = GetInputFilePath("tifftag-photoshop.tiff"); // https://github.com/lovell/sharp/issues/1600
        public static string InputGif { get; } = GetInputFilePath("Crash_test.gif"); // http://upload.wikimedia.org/wikipedia/commons/e/e3/Crash_test.gif
        public static string InputGifGreyPlusAlpha { get; } = GetInputFilePath("grey-plus-alpha.gif"); // http://i.imgur.com/gZ5jlmE.gif
        public static string InputGifAnimated { get; } = GetInputFilePath("rotating-squares.gif"); // CC0 https://loading.io/spinner/blocks/-rotating-squares-preloader-gif
        public static string InputSvg { get; } = GetInputFilePath("check.svg"); // http://dev.w3.org/SVG/tools/svgweb/samples/svg-files/check.svg
        public static string InputSvgWithEmbeddedImages { get; } = GetInputFilePath("struct-image-04-t.svg"); // https://dev.w3.org/SVG/profiles/1.2T/test/svg/struct-image-04-t.svg

        public static string InputJPGBig { get; } = GetInputFilePath("flowers.jpeg");

        public static string InputPngStripesV { get; } = GetInputFilePath("stripesV.png");
        public static string InputPngStripesH { get; } = GetInputFilePath("stripesH.png");

        public static string InputJpgBooleanTest { get; } = GetInputFilePath("booleanTest.jpg");

        public static string InputV { get; } = GetInputFilePath("vfile.v");

        public static string TestPattern { get; } = GetInputFilePath("test-pattern.png");

        public static RandomFile OutputJpg { get; } = new RandomFile(".jpg");
        public static RandomFile OutputPng { get; } = new RandomFile(".png");
        public static RandomFile OutputWebP { get; } = new RandomFile(".webp");
        public static RandomFile OutputV { get; } = new RandomFile(".v");
        public static RandomFile OutputTiff { get; } = new RandomFile(".tiff");
        public static RandomFile OutputZoinks { get; } = new RandomFile(".zoinks");

        private static string GetInputFilePath(string fileName) => Path.Join("SharpTestImages", fileName);
        // @formatter:on
    }
}
