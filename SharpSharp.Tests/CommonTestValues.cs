using System;

namespace SharpSharp {
    public static class CommonTestValues {
        private const string Murray = @"https://www.fillmurray.com/300/300";
        public const string ImageUrl = Murray;
        public static readonly Uri ImageUri = new Uri(Murray);
        public static readonly ImageLoadOptions DefaultImageLoadOptions = new ImageLoadOptions();




        public static readonly string InputJpgWithLandscapeExif1: getPath('Landscape_1.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif2: getPath('Landscape_2.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif3: getPath('Landscape_3.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif4: getPath('Landscape_4.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif5: getPath('Landscape_5.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif6: getPath('Landscape_6.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif7: getPath('Landscape_7.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithLandscapeExif8: getPath('Landscape_8.jpg'), // https://github.com/recurser/exif-orientation-examples

        public static readonly string InputJpgWithPortraitExif1: getPath('Portrait_1.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif2: getPath('Portrait_2.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif3: getPath('Portrait_3.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif4: getPath('Portrait_4.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif5: getPath('Portrait_5.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif6: getPath('Portrait_6.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif7: getPath('Portrait_7.jpg'), // https://github.com/recurser/exif-orientation-examples
        public static readonly string InputJpgWithPortraitExif8: getPath('Portrait_8.jpg'), // https://github.com/recurser/exif-orientation-examples

        public static readonly string InputJpg: getPath('2569067123_aca715a2ee_o.jpg'), // http://www.flickr.com/photos/grizdave/2569067123/
        public static readonly string InputJpgWithExif: getPath('Landscape_8.jpg'), // https://github.com/recurser/exif-orientation-examples/blob/master/Landscape_8.jpg
        public static readonly string InputJpgWithIptcAndXmp: getPath('Landscape_9.jpg'), // https://unsplash.com/photos/RWAIyGmgHTQ
        public static readonly string InputJpgWithExifMirroring: getPath('Landscape_5.jpg'), // https://github.com/recurser/exif-orientation-examples/blob/master/Landscape_5.jpg
        public static readonly string InputJpgWithGammaHoliness: getPath('gamma_dalai_lama_gray.jpg'), // http://www.4p8.com/eric.brasseur/gamma.html
        public static readonly string InputJpgWithCmykProfile: getPath('Channel_digital_image_CMYK_color.jpg'), // http://en.wikipedia.org/wiki/File:Channel_digital_image_CMYK_color.jpg
        public static readonly string InputJpgWithCmykNoProfile: getPath('Channel_digital_image_CMYK_color_no_profile.jpg'),
        public static readonly string InputJpgWithCorruptHeader: getPath('corrupt-header.jpg'),
        public static readonly string InputJpgWithLowContrast: getPath('low-contrast.jpg'), // http://www.flickr.com/photos/grizdave/2569067123/
        public static readonly string InputJpgLarge: getPath('giant-image.jpg'),
        public static readonly string InputJpg320x240: getPath('320x240.jpg'), // http://www.andrewault.net/2010/01/26/create-a-test-pattern-video-with-perl/
        public static readonly string InputJpgOverlayLayer2: getPath('alpha-layer-2-ink.jpg'),
        public static readonly string InputJpgTruncated: getPath('truncated.jpg'), // head -c 10000 2569067123_aca715a2ee_o.jpg > truncated.jpg
        public static readonly string InputJpgCenteredImage: getPath('centered_image.jpeg'),
        public static readonly string InputJpgRandom: getPath('random.jpg'), // convert -size 200x200 xc:   +noise Random   random.jpg
        public static readonly string InputJpgThRandom: getPath('thRandom.jpg'), // convert random.jpg  -channel G -threshold 5% -separate +channel -negate thRandom.jpg
        public static readonly string InputJpgLossless: getPath('testimgl.jpg'), // Lossless JPEG from ftp://ftp.fu-berlin.de/unix/X11/graphics/ImageMagick/delegates/ljpeg-6b.tar.gz

        public static readonly string InputPng: getPath('50020484-00001.png'), // http://c.searspartsdirect.com/lis_png/PLDM/50020484-00001.png
        public static readonly string InputPngWithTransparency: getPath('blackbug.png'), // public domain
        public static readonly string InputPngCompleteTransparency: getPath('full-transparent.png'),
        public static readonly string InputPngWithGreyAlpha: getPath('grey-8bit-alpha.png'),
        public static readonly string InputPngWithOneColor: getPath('2x2_fdcce6.png'),
        public static readonly string InputPngWithTransparency16bit: getPath('tbgn2c16.png'), // http://www.schaik.com/pngsuite/tbgn2c16.png
        public static readonly string InputPngOverlayLayer0: getPath('alpha-layer-0-background.png'),
        public static readonly string InputPngOverlayLayer1: getPath('alpha-layer-1-fill.png'),
        public static readonly string InputPngAlphaPremultiplicationSmall: getPath('alpha-premultiply-1024x768-paper.png'),
        public static readonly string InputPngAlphaPremultiplicationLarge: getPath('alpha-premultiply-2048x1536-paper.png'),
        public static readonly string InputPngBooleanNoAlpha: getPath('bandbool.png'),
        public static readonly string InputPngTestJoinChannel: getPath('testJoinChannel.png'),
        public static readonly string InputPngTruncated: getPath('truncated.png'), // gm convert 2569067123_aca715a2ee_o.jpg -resize 320x240 saw.png ; head -c 10000 saw.png > truncated.png
        public static readonly string InputPngEmbed: getPath('embedgravitybird.png'), // Released to sharp under a CC BY 4.0
        public static readonly string InputPngRGBWithAlpha: getPath('2569067123_aca715a2ee_o.png'), // http://www.flickr.com/photos/grizdave/2569067123/ (same as inputJpg)
        public static readonly string InputPngImageInAlpha: getPath('image-in-alpha.png'), // https://github.com/lovell/sharp/issues/1597

        public static readonly string InputWebP: getPath('4.webp'), // http://www.gstatic.com/webp/gallery/4.webp
        public static readonly string InputWebPWithTransparency: getPath('5_webp_a.webp'), // http://www.gstatic.com/webp/gallery3/5_webp_a.webp
        public static readonly string InputTiff: getPath('G31D.TIF'), // http://www.fileformat.info/format/tiff/sample/e6c9a6e5253348f4aef6d17b534360ab/index.htm
        public static readonly string InputTiffMultipage: getPath('G31D_MULTI.TIF'), // gm convert G31D.TIF -resize 50% G31D_2.TIF ; tiffcp G31D.TIF G31D_2.TIF G31D_MULTI.TIF
        public static readonly string InputTiffCielab: getPath('cielab-dagams.tiff'), // https://github.com/lovell/sharp/issues/646
        public static readonly string InputTiffUncompressed: getPath('uncompressed_tiff.tiff'), // https://code.google.com/archive/p/imagetestsuite/wikis/TIFFTestSuite.wiki file: 0c84d07e1b22b76f24cccc70d8788e4a.tif
        public static readonly string InputTiff8BitDepth: getPath('8bit_depth.tiff'),
        public static readonly string InputTifftagPhotoshop: getPath('tifftag-photoshop.tiff'), // https://github.com/lovell/sharp/issues/1600
        public static readonly string InputGif: getPath('Crash_test.gif'), // http://upload.wikimedia.org/wikipedia/commons/e/e3/Crash_test.gif
        public static readonly string InputGifGreyPlusAlpha: getPath('grey-plus-alpha.gif'), // http://i.imgur.com/gZ5jlmE.gif
        public static readonly string InputGifAnimated: getPath('rotating-squares.gif'), // CC0 https://loading.io/spinner/blocks/-rotating-squares-preloader-gif
        public static readonly string InputSvg: getPath('check.svg'), // http://dev.w3.org/SVG/tools/svgweb/samples/svg-files/check.svg
        public static readonly string InputSvgWithEmbeddedImages: getPath('struct-image-04-t.svg'), // https://dev.w3.org/SVG/profiles/1.2T/test/svg/struct-image-04-t.svg

        public static readonly string InputJPGBig: getPath('flowers.jpeg'),

        public static readonly string InputPngStripesV: getPath('stripesV.png'),
        public static readonly string InputPngStripesH: getPath('stripesH.png'),

        public static readonly string InputJpgBooleanTest: getPath('booleanTest.jpg'),

        public static readonly string InputV: getPath('vfile.v'),

        public static readonly string OutputJpg: getPath('output.jpg'),
        public static readonly string OutputPng: getPath('output.png'),
        public static readonly string OutputWebP: getPath('output.webp'),
        public static readonly string OutputV: getPath('output.v'),
        public static readonly string OutputTiff: getPath('output.tiff'),
        public static readonly string OutputZoinks: getPath('output.zoinks'), // an 'unknown' file extension

        public static readonly string TestPattern: getPath('test-pattern.png'),
    }
}
