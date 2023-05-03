using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ThumbHash.Benchmarks;

[MemoryDiagnoser]
[HardwareCounters(HardwareCounter.BranchInstructions, HardwareCounter.BranchMispredictions)]
public class Benchmarks
{
    private static SKBitmap GetBitmap(string path, bool fixPremul = false)
    {
        using var skbmp = fixPremul
            ? SKBitmap.Decode(path, SKBitmap.DecodeBounds(path).WithAlphaType(SKAlphaType.Unpremul))
            : SKBitmap.Decode(path);
        var result = skbmp.Copy(SKColorType.Rgba8888);
        return result;
    }

    private static string AssetBase
    {
        get
        {
            var cwd = Directory.GetCurrentDirectory();

            string assetsDir;
            while (!Directory.Exists(assetsDir = Path.Join(cwd, "assets")))
            {
                cwd = Path.GetDirectoryName(cwd);
            }

            return assetsDir;
        }
    }

    private static readonly SKBitmap Flower = GetBitmap(Path.Join(AssetBase, "flower.jpg"));

    private static readonly SKBitmap Tux = GetBitmap(Path.Join(AssetBase, "tux.png"));

    public static IEnumerable<object> Images_NoAlpha
    {
        get
        {
            yield return Flower;
        }
    }

    public static IEnumerable<object> Images_Alpha
    {
        get
        {
            yield return Tux;
        }
    }

    private static byte[] FlowerThumbHash => Convert.FromHexString("934A062D069256C374055867DA8AB6679490510719");
    private static byte[] TuxThumbHash => Convert.FromHexString("A1198A1C02383A25D727F68B971FF7F9717F80376758987906");

    public static IEnumerable<object> ThumbHashes_NoAlpha
    {
        get
        {
            yield return FlowerThumbHash;
        }
    }

    public static IEnumerable<object> ThumbHashes_Alpha
    {
        get
        {
            yield return TuxThumbHash;
        }
    }


    [Benchmark]
    [ArgumentsSource(nameof(Images_NoAlpha))]
    public int RgbaToThumbHash_NoAlpha(SKBitmap image) => ThumbHash.RgbaToThumbHash(stackalloc byte[25], image.Width, image.Height, image.GetPixelSpan());

    [Benchmark]
    [ArgumentsSource(nameof(Images_Alpha))]
    public int RgbaToThumbHash_Alpha(SKBitmap image) => ThumbHash.RgbaToThumbHash(stackalloc byte[25], image.Width, image.Height, image.GetPixelSpan());

    [Benchmark]
    [ArgumentsSource(nameof(ThumbHashes_NoAlpha))]
    public (int,int,byte[]) ThumbHashToRgba_NoAlpha(byte[] thumbhash) => ThumbHash.ThumbHashToRgba(thumbhash);

    [Benchmark]
    [ArgumentsSource(nameof(ThumbHashes_Alpha))]
    public (int, int, byte[]) ThumbHashToRgba_Alpha(byte[] thumbhash) => ThumbHash.ThumbHashToRgba(thumbhash);
}
