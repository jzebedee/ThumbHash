using SkiaSharp;
using System.Runtime.CompilerServices;

namespace ThumbHash.Tests;

public class ThumbHashTests
{
    private static SKBitmap FlowerBitmap => GetBitmap("Resources/flower.jpg");

    private static byte[] FlowerThumbHash => new byte[] { 147, 74, 6, 45, 6, 146, 86, 195, 116, 5, 88, 103, 218, 138, 182, 103, 148, 144, 81, 7, 25 };

    private static (float r, float g, float b, float a) FlowerThumbHashAverages => (r: 0.484127015f, g: 0.341269821f, b: 0.0793650597f, a: 1f);

    private static SKBitmap FlowerThumbHashRendered => GetBitmap("Resources/flower_thumbhash_rust.png");

    private const float FlowerRatio = 0.714285731f;

    private static SKBitmap TuxBitmap => GetBitmap("Resources/tux.png", fixPremul: true);

    private static byte[] TuxThumbHash => Convert.FromHexString("A1 19 8A 1C 02 38 3A 25 D7 27 F6 8B 97 1F F7 F9 71 7F 80 37 67 58 98 79 06".Replace(" ", ""));

    private static (float r, float g, float b, float a) TuxThumbHashAverages => (r: 0.616402208f, g: 0.568783104f, b: 0.386243373f, a: 0.533333361f);

    private static SKBitmap TuxThumbHashRendered => GetBitmap("Resources/tux_thumbhash_rust.png");

    private static SKBitmap GetBitmap(string path, bool fixPremul = false)
    {
        using var skbmp = fixPremul
            ? SKBitmap.Decode(path, SKBitmap.DecodeBounds(path).WithAlphaType(SKAlphaType.Unpremul))
            : SKBitmap.Decode(path);
        var result = skbmp.Copy(SKColorType.Rgba8888);
        return result;
    }

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

    [Theory]
    [MemberData(nameof(TestImages))]
    public void RgbaToThumbHash(SKBitmap expected_img, byte[] expected_thumbhash)
    {
        using var img = expected_img;
        var thumbhash = ThumbHash.RgbaToThumbHash(img.Width, img.Height, img.GetPixelSpan());
        Assert.Equal(thumbhash, expected_thumbhash);
    }

    [Fact]
    public void RgbaToThumbHash_ThrowsOnBadDimensions()
    {
        Assert.Throws<ArgumentOutOfRangeException>("w", () => ThumbHash.RgbaToThumbHash(101, 1, stackalloc byte[101 * 1 * 4]));
        Assert.Throws<ArgumentOutOfRangeException>("h", () => ThumbHash.RgbaToThumbHash(1, 101, stackalloc byte[1 * 101 * 4]));
    }

    [Fact]
    public void RgbaToThumbHash_ThrowsOnBadPixelSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>("rgba.Length", () => ThumbHash.RgbaToThumbHash(1, 1, stackalloc byte[3]));
        Assert.Throws<ArgumentOutOfRangeException>("rgba.Length", () => ThumbHash.RgbaToThumbHash(1, 1, stackalloc byte[5]));
    }

    [Theory]
    [MemberData(nameof(TestThumbHashes))]
    public void ThumbHashToRgba(byte[] thumbhash, SKBitmap thumbhash_rendered)
    {
        var (w, h, hash_rgba) = ThumbHash.ThumbHashToRgba(thumbhash);

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
        Assert.Throws<ArgumentOutOfRangeException>(() => ThumbHash.ThumbHashToRgba(stackalloc byte[4]));
    }

    [Theory]
    [MemberData(nameof(TestThumbHashAverages))]
    public void ThumbHashToAverageRgba(byte[] thumbhash, (float r, float g, float b, float a) averages)
    {
        var expected = averages;
        var actual = ThumbHash.ThumbHashToAverageRgba(thumbhash);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ThumbHashToAverageRgba_ThrowsOnBadHashSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ThumbHash.ThumbHashToAverageRgba(stackalloc byte[4]));
    }

    [Fact]
    public void ThumbHashToApproximateAspectRatio()
    {
        Assert.Equal(FlowerRatio, ThumbHash.ThumbHashToApproximateAspectRatio(FlowerThumbHash));
    }

    [Fact]
    public void ThumbHashToApproximateAspectRatio_ThrowsOnBadHashSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ThumbHash.ThumbHashToApproximateAspectRatio(stackalloc byte[4]));
    }
}