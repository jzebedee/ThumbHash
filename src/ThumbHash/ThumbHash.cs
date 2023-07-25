namespace ThumbHashes;

using static Utilities;

public readonly record struct ThumbHash(ReadOnlyMemory<byte> Hash)
{
    /// <summary>
    /// Extracts the approximate aspect ratio of the original image.
    /// </summary>
    public float ApproximateAspectRatio => ThumbHashToApproximateAspectRatio(Hash.Span);

    /// <summary>
    /// Extracts the average color from a ThumbHash.
    /// </summary>
    /// <returns>Unpremultiplied RGBA values where each value ranges from 0 to 1. </returns>
    public (float r, float g, float b, float a) AverageColor => ThumbHashToAverageRgba(Hash.Span);

    /// <summary>
    /// Decodes a ThumbHash to an RGBA image.
    /// </summary>
    /// <returns>Width, height, and unpremultiplied RGBA8 pixels of the rendered ThumbHash.</returns>
    public (int width, int height, byte[] rgba) ToImage() => ThumbHashToRgba(Hash.Span);

    /// <summary>
    /// Encodes an RGBA image to a ThumbHash.
    /// </summary>
    /// <param name="width">The width of the input image. Must be ≤100px.</param>
    /// <param name="height">The height of the input image. Must be ≤100px.</param>
    /// <param name="rgba">The pixels in the input image, row-by-row. RGB should not be premultiplied by A. Must have `w*h*4` elements.</param>
    public static ThumbHash FromImage(int width, int height, ReadOnlySpan<byte> rgba)
        => new(RgbaToThumbHash(width, height, rgba));
}
