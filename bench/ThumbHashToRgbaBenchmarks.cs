using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using System;
using System.Collections.Generic;

namespace ThumbHashes.Benchmarks;

[MemoryDiagnoser]
[HardwareCounters(HardwareCounter.BranchInstructions, HardwareCounter.BranchMispredictions)]
public class ThumbHashToRgbaBenchmarks
{
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
    [ArgumentsSource(nameof(ThumbHashes_NoAlpha))]
    public (int,int) ThumbHashToRgba_NoAlpha(byte[] thumbhash) => Utilities.ThumbHashToRgba(thumbhash, stackalloc byte[32 * 32 * 4]);

    [Benchmark]
    [ArgumentsSource(nameof(ThumbHashes_Alpha))]
    public (int, int) ThumbHashToRgba_Alpha(byte[] thumbhash) => Utilities.ThumbHashToRgba(thumbhash, stackalloc byte[32 * 32 * 4]);
}
