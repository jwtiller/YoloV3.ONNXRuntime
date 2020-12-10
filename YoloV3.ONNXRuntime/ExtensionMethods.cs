using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using SkiaSharp;
using YoloV3.ONNXRuntime.Models;

namespace YoloV3.ONNXRuntime
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T[]> AsChunks<T>(
            this T[] source, int chunkMaxSize)
        {
            var chunks = source.Length / chunkMaxSize;
            var result = new List<T[]>(chunks + 1);
            var offset = 0;

            for (var i = 0; i < chunks; i++)
            {
                result.Add(new ArraySegment<T>(source,
                    offset,
                    chunkMaxSize).ToArray());
                offset += chunkMaxSize;
            }

            return result;
        }

        public static SKImage DrawRectangles(this SKImage image, IList<YoloBoxRectangle> rectangles)
        {
            var surface = SKSurface.Create(new SKImageInfo(416, 416));
            var canvas = surface.Canvas;
            canvas.DrawImage(image, 0, 0);
            var paint = new SKPaint {IsAntialias = true, Color = SKColor.Parse("#f42069"), Style = SKPaintStyle.Stroke};
            foreach (var rectangle in rectangles)
            {

                canvas.DrawRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, paint);
            }

            return SKImage.FromEncodedData(surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100));
        }

        public static void SaveToFile(this SKImage image, string filePath)
        {
            using (var fileStream = File.OpenWrite(filePath))
            {
                image.Encode(SKEncodedImageFormat.Png,100).SaveTo(fileStream);
            }
        }

        public static SKImage Resize(this SKImage input, int width, int height)
        {
            var imageInfo = new SKImageInfo(width, height);
            SKImage image = SKImage.Create(imageInfo);
            input.ScalePixels(image.PeekPixels(), SKFilterQuality.High);
            return image;
        }
    }
}
