using SkiaSharp;

namespace ThumbHashes.Tests;

internal static class Resources
{
    internal static SKBitmap FlowerBitmap => GetBitmap("Resources/flower.jpg");

    internal static byte[] FlowerThumbHash => Convert.FromHexString("934A062D069256C374055867DA8AB6679490510719");

    internal static (float r, float g, float b, float a) FlowerThumbHashAverages => (r: 0.484127015f, g: 0.341269821f, b: 0.0793650597f, a: 1f);

    internal static SKBitmap FlowerThumbHashRendered => GetBitmap("Resources/flower_thumbhash_rust.png");

    internal const float FlowerAspectRatio = 0.714285731f;

    internal static SKBitmap TuxBitmap => GetBitmap("Resources/tux.png", fixPremul: true);

    internal static byte[] TuxThumbHash => Convert.FromHexString("A1198A1C02383A25D727F68B971FF7F9717F80376758987906");

    internal static (float r, float g, float b, float a) TuxThumbHashAverages => (r: 0.616402208f, g: 0.568783104f, b: 0.386243373f, a: 0.533333361f);

    internal static SKBitmap TuxThumbHashRendered => GetBitmap("Resources/tux_thumbhash_rust.png");

    internal const float TuxAspectRatio = 0.800000011f;

    private static SKBitmap GetBitmap(string path, bool fixPremul = false)
    {
        using var skbmp = fixPremul
            ? SKBitmap.Decode(path, SKBitmap.DecodeBounds(path).WithAlphaType(SKAlphaType.Unpremul))
            : SKBitmap.Decode(path);
        var result = skbmp.Copy(SKColorType.Rgba8888);
        return result;
    }
}
