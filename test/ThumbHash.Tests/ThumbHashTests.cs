using SkiaSharp;

namespace ThumbHashes.Tests;
using static Resources;

public class ThumbHashTests
{
    public static IEnumerable<object[]> TestThumbHashes
    {
        get
        {
            yield return new object[] { FlowerThumbHash, FlowerThumbHashRendered, FlowerBitmap, FlowerAspectRatio, FlowerThumbHashAverages };
            yield return new object[] { TuxThumbHash, TuxThumbHashRendered, TuxBitmap, TuxAspectRatio, TuxThumbHashAverages };
        }
    }

    [Theory]
    [MemberData(nameof(TestThumbHashes))]
    public void ValidateThumbHash(byte[] hash, SKBitmap rendered, SKBitmap original, float aspectRatio, (float r, float g, float b, float a) averages)
    {
        using (rendered)
        using (original)
        {
            var th = new ThumbHash(hash);
            Assert.Equal(hash, th.Hash);
            Assert.Equal(aspectRatio, th.ApproximateAspectRatio);
            Assert.Equal(averages, th.AverageColor);

            var (w, h, thRenderedBytes) = th.ToImage();
            Assert.Equal(rendered.Width, w);
            Assert.Equal(rendered.Height, h);

            var spanBytes = new byte[w * h * 4];
            _ = th.ToImage(spanBytes);
            Assert.Equal(thRenderedBytes, spanBytes);

            {
                using var hash_img = SKImage.FromPixelCopy(new(w, h, SKColorType.Rgba8888, SKAlphaType.Unpremul), thRenderedBytes);
                using var hash_data_png = hash_img.Encode(SKEncodedImageFormat.Png, 100);
                using var hash_bmp = SKBitmap.Decode(hash_data_png);

                Assert.Equal(rendered.Pixels, hash_bmp.Pixels);
            }

            {
                var thRecreated = ThumbHash.FromImage(original.Width, original.Height, original.GetPixelSpan());
                //can't just compare th <-> thRecreated because
                //ROM/ROS's equality does not check contents
                // "This tests if two ReadOnlySpan<T> instances point to the same starting memory location,
                // and have the same Length values. This does not compare the contents of two ReadOnlySpan<T> instances."
                // https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1.op_equality?view=net-7.0#remarks
                Assert.True(th.Hash.Span.SequenceEqual(thRecreated.Hash.Span));
            }
        }
    }
}
