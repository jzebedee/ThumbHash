using SkiaSharp;

namespace ThumbHash.Tests;

public class ThumbHashTests
{
    private static byte[] Flower { get; } = File.ReadAllBytes("Resources/flower.jpg");

    private static SKBitmap FlowerBitmap
    {
        get
        {
            using var skbmp = SKBitmap.Decode(Flower);
            return skbmp.Copy(SKColorType.Rgba8888);
        }
    }

    private static byte[] FlowerThumbHash => new byte[] { 147, 74, 6, 45, 6, 146, 86, 195, 116, 5, 88, 103, 218, 138, 182, 103, 148, 144, 81, 7, 25 };

    private const float FlowerRatio = 0.714285731f;

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

    private static SKImage FromPixels(int w, int h, ReadOnlySpan<byte> pixels)
        => SKImage.FromPixelCopy(new SKImageInfo(w, h, SKColorType.Rgba8888), pixels);

    [Fact]
    public void RgbaToThumbHash()
    {
        using var img = FlowerBitmap;
        var thumbhash = ThumbHash.RgbaToThumbHash(img.Width, img.Height, img.GetPixelSpan());
        Assert.Equal(thumbhash, FlowerThumbHash);
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
    }

    [Fact]
    public void ThumbHashToRgba_ThrowsOnBadHashSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ThumbHash.ThumbHashToRgba(stackalloc byte[4]));
    }

    [Fact]
    public void ThumbHashToAverageRba()
    {
        ThumbHash.ThumbHashToAverageRba();
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