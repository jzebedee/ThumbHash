using SkiaSharp;

namespace ThumbHashes.Tests;

internal static class Resources
{
    private static byte[] FromHexString(string s)
#if NET6_0_OR_GREATER
        => Convert.FromHexString(s);
#else
    {
        //this is only used for tests, so we can do it the slow and awful way
        var ret = new byte[s.Length >> 1];
        for (int i = 0; i < s.Length; i += 2)
        {
            ret[i >> 1] = Convert.ToByte(s.Substring(i, 2), 16);
        }
        return ret;
    }
#endif

    internal static SKBitmap FlowerBitmap => GetBitmap("Resources/flower.jpg");

    internal static byte[] FlowerThumbHash => FromHexString("934A062D069256C374055867DA8AB6679490510719");

    internal static (float r, float g, float b, float a) FlowerThumbHashAverages => (r: 0.484127015f, g: 0.341269821f, b: 0.0793650597f, a: 1f);

    internal static SKBitmap FlowerThumbHashRendered => GetBitmap("Resources/flower_thumbhash_rust.png");

    internal const float FlowerAspectRatio = 0.714285731f;

    internal static SKBitmap TuxBitmap => GetBitmap("Resources/tux.png");

    internal static byte[] TuxThumbHash => FromHexString("A1198A1C02383A25D727F68B971FF7F9717F80376758987906");

    internal static (float r, float g, float b, float a) TuxThumbHashAverages => (r: 0.616402208f, g: 0.568783104f, b: 0.386243373f, a: 0.533333361f);

    internal static SKBitmap TuxThumbHashRendered => GetBitmap("Resources/tux_thumbhash_rust.png");

    internal const float TuxAspectRatio = 0.800000011f;

    private static SKBitmap GetBitmap(string path)
    {
        using var skbmp = SKBitmap.Decode(path);
        return skbmp.Copy(SKColorType.Rgba8888);
    }
}
