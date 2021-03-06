﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using SkiaSharp;
using YoloV3.ONNXRuntime;

namespace Example
{
    class Program
    {
        private static readonly YoloV3Onnx _yoloV3Onnx = new YoloV3Onnx(Path.Combine("Model","yolov3.onnx"),Path.Combine("Model","labels.txt"));
        static void Main(string[] args)
        {
            if (!Directory.Exists("result"))
            {
                Directory.CreateDirectory("result");
            }
            foreach (var file in Directory.GetFiles("result"))
            {
                File.Delete(file);
            }

            foreach (var image in Directory.GetFiles("images"))
            {
                var skImage = SKImage.FromEncodedData(new MemoryStream(File.ReadAllBytes(image)));
                var result = _yoloV3Onnx.Detect(skImage);

                var skImageResult = skImage.Resize(Constants.YoloImage.Width,Constants.YoloImage.Height).DrawRectangles(result);
                skImageResult.SaveToFile(Path.Combine("result",$"{Guid.NewGuid()}.png"));
            }
        }
    }
}
