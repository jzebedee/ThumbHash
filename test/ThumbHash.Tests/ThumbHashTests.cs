using SkiaSharp;

namespace ThumbHash.Tests;

public class ThumbHashTests
{
    private static SKBitmap FlowerBitmap => GetBitmap("Resources/flower.jpg");

    private static byte[] FlowerThumbHash => new byte[] { 147, 74, 6, 45, 6, 146, 86, 195, 116, 5, 88, 103, 218, 138, 182, 103, 148, 144, 81, 7, 25 };

    private static (float r, float g, float b, float a) FlowerThumbhashAverages => (r: 0.484127015f, g: 0.341269821f, b: 0.0793650597f, a: 1f);

    private static SKBitmap FlowerThumbhashRendered => GetBitmap("Resources/flower_thumbhash_rust.png");

    private const float FlowerRatio = 0.714285731f;

    private static SKBitmap TuxBitmap => GetBitmap("Resources/tux.png", fixPremul: true);

    private static byte[] TuxThumbHash => Convert.FromHexString("A1 19 8A 1C 02 38 3A 25 D7 27 F6 8B 97 1F F7 F9 71 7F 80 37 67 58 98 79 06".Replace(" ", ""));

    //private static unsafe void SaveImage(Stream output, int w, int h, ReadOnlySpan<byte> pixels)
    //{
    //    fixed (byte* ptr = pixels)
    //    {
    //        using var pixmap = new SKPixmap(new SKImageInfo(w, h, SKColorType.Rgba8888), (nint)ptr);
    //        using var hash_img = SKImage.FromPixels(pixmap);
    //        using var data = hash_img.Encode();
    //        data.SaveTo(output);
    //    }
    //}

    private static SKBitmap GetBitmap(string path, bool fixPremul = false)
    {
        using var skbmp = fixPremul
            ? SKBitmap.Decode(path, SKBitmap.DecodeBounds(path).WithAlphaType(SKAlphaType.Unpremul))
            : SKBitmap.Decode(path);
        var result = skbmp.Copy(SKColorType.Rgba8888);
        return result;
    }

    private static SKImage FromPixels(int w, int h, ReadOnlySpan<byte> pixels)
        => SKImage.FromPixelCopy(new SKImageInfo(w, h, SKColorType.Rgba8888), pixels);

    public static IEnumerable<object[]> TestImages
    {
        get
        {
            yield return new object[] { FlowerBitmap, FlowerThumbHash };
            yield return new object[] { TuxBitmap, TuxThumbHash };
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

    [Fact]
    public void ThumbHashToRgba()
    {
        var (w, h, hash_rgba) = ThumbHash.ThumbHashToRgba(FlowerThumbHash);
        using var hash_img = FromPixels(w, h, hash_rgba);
        using var hash_bmp = SKBitmap.FromImage(hash_img);

        using var expected_hash_img = FlowerThumbhashRendered;

        Assert.Equal(expected_hash_img.Pixels, hash_bmp.Pixels);
    }

    [Fact]
    public void ThumbHashToRgba_ThrowsOnBadHashSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ThumbHash.ThumbHashToRgba(stackalloc byte[4]));
    }

    [Fact]
    public void ThumbHashToAverageRgba()
    {
        var expected = FlowerThumbhashAverages;
        var actual = ThumbHash.ThumbHashToAverageRgba(FlowerThumbHash);

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