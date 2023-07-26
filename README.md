# ThumbHash

[![ThumbHash nuget package](https://img.shields.io/nuget/v/ThumbHash.svg?style=flat)](https://www.nuget.org/packages/ThumbHash)
[![CI build-test-pack](https://github.com/jzebedee/ThumbHash/actions/workflows/ci.yml/badge.svg)](https://github.com/jzebedee/ThumbHash/actions/workflows/ci.yml)

This library is a .NET implementation of [ThumbHash](https://github.com/evanw/thumbhash).

## What is a thumbhash?

A very compact representation of a placeholder for an image. Store it inline with your data and show it while the real image is loading for a smoother loading experience.

|Image|ThumbHash|ThumbHash Image|
|-----|---------|---------------|
|![Flower](assets/flower.jpg)|<p>`93 4A 06 2D 06 92 56 C3 74 05 58 67 DA 8A B6 67 94 90 51 07 19`</p>21 bytes|<img alt="Flower ThumbHash" src="/assets/flower_thumbhash_rust.png" width=75 height=100>|
|![Tux](assets/tux.png)|<p>`A1 19 8A 1C 02 38 3A 25 D7 27 F6 8B 97 1F F7 F9 71 7F 80 37 67 58 98 79 06`</p>25 bytes|<img alt="Tux ThumbHash" src="/assets/tux_thumbhash_rust.png" width=84 height=100>|

[See a demo of ThumbHash in action _here_](https://evanw.github.io/thumbhash/).

## Usage

The ThumbHash library has no external dependencies and can be used on its own, or with your choice of image library (e.g., [SkiaSharp](https://github.com/mono/SkiaSharp), [ImageSharp](https://github.com/SixLabors/ImageSharp)) for rendering.

Images must be in the format of unpremultiplied RGBA8888 before working with ThumbHash.

### Convert an image into a thumbhash (SkiaSharp)
```csharp
using SkiaSharp;
using ThumbHashes;

using var original = SKBitmap.Decode("original.png");
using var rgba8888 = original.Copy(SKColorType.Rgba8888);
var thumbhash = ThumbHash.FromImage(rgba8888.Width, rgba8888.Height, rgba8888.GetPixelSpan());
```

### Load a thumbhash and render it to RGBA
```csharp
using ThumbHashes;

byte[] hash = Convert.FromHexString("934A062D069256C374055867DA8AB6679490510719");
var thumbhash = new ThumbHash(hash);

var (width, height, rgba) = thumbhash.ToImage();
```

### Save a rendered RGBA to a PNG (SkiaSharp)
```csharp
using SkiaSharp;

var (width, height, rgba) = thumbhash.ToImage();
var image_info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
using var sk_img = SKImage.FromPixelCopy(image_info, rgba);
using var sk_png_data = sk_img.Encode(SKEncodedImageFormat.Png, 100);

using var fs_png = System.IO.File.Create("test.png");
sk_png_data.SaveTo(fs_png);
```

## Support

[![Discord](https://img.shields.io/discord/359127425558249482)](https://discord.gg/FkRPyz6kcD)

Discussion and technical support is available on Discord.
