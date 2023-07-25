using SkiaSharp;

namespace ThumbHashes.Tests;
using static Resources;

public class UtilitiesTests
{
    public static IEnumerable<object[]> TestImages
    {
        get
        {
            yield return new object[] { FlowerBitmap, FlowerThumbHash };
            yield return new object[] { TuxBitmap, TuxThumbHash };
        }
    }

    public static IEnumerable<object[]> TestThumbHashes
    {
        get
        {
            yield return new object[] { FlowerThumbHash, FlowerThumbHashRendered };
            yield return new object[] { TuxThumbHash, TuxThumbHashRendered };
        }
    }

    public static IEnumerable<object[]> TestThumbHashAverages
    {
        get
        {
            yield return new object[] { FlowerThumbHash, FlowerThumbHashAverages };
            yield return new object[] { TuxThumbHash, TuxThumbHashAverages };
        }
    }

    public static IEnumerable<object[]> TestThumbHashRatios
    {
        get
        {
            yield return new object[] { FlowerThumbHash, FlowerAspectRatio };
            yield return new object[] { TuxThumbHash, TuxAspectRatio };
        }
    }

    [Theory]
    [MemberData(nameof(TestImages))]
    public void RgbaToThumbHash(SKBitmap expected_img, byte[] expected_thumbhash)
    {
        using var img = expected_img;
        var thumbhash = Utilities.RgbaToThumbHash(img.Width, img.Height, img.GetPixelSpan());
        Assert.Equal(thumbhash, expected_thumbhash);
    }

    [Fact]
    public void RgbaToThumbHash_ThrowsOnBadDimensions()
    {
        Assert.Throws<ArgumentOutOfRangeException>("w", () => Utilities.RgbaToThumbHash(101, 1, stackalloc byte[101 * 1 * 4]));
        Assert.Throws<ArgumentOutOfRangeException>("h", () => Utilities.RgbaToThumbHash(1, 101, stackalloc byte[1 * 101 * 4]));
    }

    [Fact]
    public void RgbaToThumbHash_ThrowsOnBadPixelSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>("rgba_bytes.Length", () => Utilities.RgbaToThumbHash(1, 1, stackalloc byte[3]));
        Assert.Throws<ArgumentOutOfRangeException>("rgba_bytes.Length", () => Utilities.RgbaToThumbHash(1, 1, stackalloc byte[5]));
    }

    [Theory]
    [MemberData(nameof(TestThumbHashes))]
    public void ThumbHashToRgba(byte[] thumbhash, SKBitmap thumbhash_rendered)
    {
        var (w, h, hash_rgba) = Utilities.ThumbHashToRgba(thumbhash);

        //some pixels are zeroed out in the png form so we round-trip this to make it match the expected png
        using var hash_img = SKImage.FromPixelCopy(new(w, h, SKColorType.Rgba8888, SKAlphaType.Unpremul), hash_rgba);
        using var hash_data_png = hash_img.Encode(SKEncodedImageFormat.Png, 100);
        using var hash_bmp = SKBitmap.Decode(hash_data_png);

        //
        using var expected_hash_bmp = thumbhash_rendered;
 
        //if(expected_hash_img.AlphaType is SKAlphaType.Unpremul)
        //{
        //    var th_rend_rgba = File.ReadAllBytes(@"examples\rust\tux_thumbhash.rgba");
        //    hash_rgba.SequenceEqual(th_rend_rgba);
        //}

        //{
        //    using var fs = File.Create("hash.png");
        //    hash_data_png.SaveTo(fs);
        //}

        Assert.Equal(expected_hash_bmp.Pixels, hash_bmp.Pixels);
    }

    [Fact]
    public void ThumbHashToRgba_ThrowsOnBadHashSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>("hash.Length", () => Utilities.ThumbHashToRgba(stackalloc byte[4]));
    }

    [Fact]
    public void ThumbHashToRgba_ThrowsOnBadRgbaSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>("rgba.Length", () => Utilities.ThumbHashToRgba(FlowerThumbHash, stackalloc byte[4]));
    }

    [Theory]
    [MemberData(nameof(TestThumbHashAverages))]
    public void ThumbHashToAverageRgba(byte[] thumbhash, (float r, float g, float b, float a) averages)
    {
        var expected = averages;
        var actual = Utilities.ThumbHashToAverageRgba(thumbhash);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ThumbHashToAverageRgba_ThrowsOnBadHashSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.ThumbHashToAverageRgba(stackalloc byte[4]));
    }

    [Theory]
    [MemberData(nameof(TestThumbHashRatios))]
    public void ThumbHashToApproximateAspectRatio(byte[] thumbhash, float aspectRatio)
    {
        var expected = aspectRatio;
        var actual = Utilities.ThumbHashToApproximateAspectRatio(thumbhash);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ThumbHashToApproximateAspectRatio_ThrowsOnBadHashSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.ThumbHashToApproximateAspectRatio(stackalloc byte[4]));
    }
}