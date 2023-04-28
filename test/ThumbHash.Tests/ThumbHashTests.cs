namespace ThumbHash.Tests;

public class ThumbHashTests
{
    [Fact]
    public void RgbaToThumbHash()
    {
        var th = new ThumbHash();
        th.RgbaToThumbHash();
    }

    [Fact]
    public void ThumbHashToRgba()
    {
        var th = new ThumbHash();
        th.ThumbHashToRgba();
    }

    [Fact]
    public void ThumbHashToAverageRba()
    {
        var th = new ThumbHash();
        th.ThumbHashToAverageRba();
    }

    [Fact]
    public void ThumbHashToApproximateAspectRatio()
    {
        var th = new ThumbHash();
        th.ThumbHashToApproximateAspectRatio();
    }
}