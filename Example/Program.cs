using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkiaSharp;
using YoloV3.ONNXRuntime;

namespace Example
{
    class Program
    {
        private static readonly YoloV3Onnx _yoloV3Onnx = new YoloV3Onnx(Path.Combine("Model","yolov3.onnx"),Path.Combine("Model","labels.txt"));
        static void Main(string[] args)
        {
            foreach (var image in Directory.GetFiles("images"))
            {
                var skImage = SKImage.FromEncodedData(new MemoryStream(File.ReadAllBytes(image)));
                var result = _yoloV3Onnx.Detect(skImage);

                var skImageResult = skImage.Resize(416,416).DrawRectangles(result);
                skImageResult.SaveToFile($"{Guid.NewGuid()}.png");
            }
        }
    }
}
