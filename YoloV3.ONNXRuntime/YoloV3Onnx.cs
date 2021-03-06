﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using YoloV3.ONNXRuntime.Models;

namespace YoloV3.ONNXRuntime
{
    public class YoloV3Onnx
    {
        private readonly InferenceSession _session;
        private string[] _labels = null;

        public YoloV3Onnx(string modelFile, string labelFile)
        {
            _session = new InferenceSession(File.ReadAllBytes(modelFile));
            _labels = File.ReadAllLines(labelFile);
        }

        public IList<YoloBoxRectangle> Detect(SKImage image)
        {
            if (image.Width != Constants.YoloImage.Width || image.Height != Constants.YoloImage.Height)
            {
                image = image.Resize(Constants.YoloImage.Width, Constants.YoloImage.Height);
            }
            var imageData = ImageToFloats(image);
            var tensor1 = new DenseTensor<float>(imageData, new int[] { 1, Constants.YoloImage.Width, Constants.YoloImage.Height, 3 });
            var container = new List<NamedOnnxValue>();
            container.Add(NamedOnnxValue.CreateFromTensor<float>("input_1", tensor1));

            var yolov3Output = _session.Run(container);
            var x13 = yolov3Output.ElementAt(0).AsTensor<float>().ToArray();
            var x26 = yolov3Output.ElementAt(1).AsTensor<float>().ToArray();
            var x52 = yolov3Output.ElementAt(2).AsTensor<float>().ToArray();

            var allBoxes = new List<YoloBox>();
            allBoxes.AddRange(GetBoxes(x13, _labels, 13, 13));
            allBoxes.AddRange(GetBoxes(x26, _labels, 26, 26));
            allBoxes.AddRange(GetBoxes(x52, _labels, 52, 52));

            var rects = allBoxes.Select(x => x.Rectangle).ToList();
            var filtered = FilterBoundingBoxes(rects, 30, 0.2f);

            return filtered;
        }
        public IList<YoloBoxRectangle> Detect(byte[] image)
        {
            return Detect(SKImage.FromEncodedData(new MemoryStream(image)));
        }
        public IList<YoloBoxRectangle> Detect(string imageFile)
        {
            var image = SKImage.FromEncodedData(new MemoryStream(File.ReadAllBytes(imageFile)));
            return Detect(image);
        }


        private IList<YoloBoxRectangle> FilterBoundingBoxes(IList<YoloBoxRectangle> boxes, int limit, float threshold)
        {
            var activeCount = boxes.Count;
            var isActiveBoxes = new bool[boxes.Count];

            for (int i = 0; i < isActiveBoxes.Length; i++)
                isActiveBoxes[i] = true;

            var sortedBoxes = boxes.Select((b, i) => new { Box = b, Index = i })
                .ToList();

            var results = new List<YoloBoxRectangle>();

            for (int i = 0; i < boxes.Count; i++)
            {
                if (isActiveBoxes[i])
                {
                    var boxA = sortedBoxes[i].Box;
                    results.Add(boxA);

                    if (results.Count >= limit)
                        break;

                    for (var j = i + 1; j < boxes.Count; j++)
                    {
                        if (isActiveBoxes[j])
                        {
                            var boxB = sortedBoxes[j].Box;
                            var intersection = boxA.IntersectionOverUnion(boxB);
                            if (intersection > threshold)
                            {
                                isActiveBoxes[j] = false;
                                activeCount--;

                                if (activeCount <= 0)
                                    break;
                            }
                        }
                    }

                    if (activeCount <= 0)
                        break;
                }
            }
            return results;
        }

        private static List<YoloBox> GetBoxes(float[] data, string[] labels, int rows, int columns)
        {
            var chunks = data.AsChunks(85).ToArray();

            int chunkCounter = 0;
            var boxes = new List<YoloBox>();
            for (int box = 0; box < 3; box++)
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {

                        var currentData = chunks[chunkCounter];

                        var boxConfidence = Utils.Sigmoid(currentData[4]);


                        if (boxConfidence > 0.5)
                        {
                            var classConfidences = new List<float>();
                            for (int i = 5; i < labels.Length+5; i++)
                            {
                                var confidence = Utils.Sigmoid(currentData[i]);
                                classConfidences.Add(confidence);
                            }

                            var topScore = classConfidences.Max();
                            var topScoreIndex = classConfidences.IndexOf(topScore);
                            var label = labels[topScoreIndex];
                            if (topScore > 0.5)
                            {
                                boxes.Add(new YoloBox(currentData[0], currentData[1], currentData[2], currentData[3],
                                    box, columns, rows, x, y, boxConfidence, classConfidences.ToArray(), label));
                            }
                        }

                        chunkCounter++;
                    }

                }

            }

            return boxes;
        }


        private static float[] ImageToFloats(SKImage image)
        {
            int width = Constants.YoloImage.Width;
            int height = Constants.YoloImage.Height;
            var imageInfo = new SKImageInfo(width, height);
            var bytes = new byte[imageInfo.BytesSize];
            var pixelBuffer = IntPtr.Zero;
            try
            {
                pixelBuffer = Marshal.AllocHGlobal(imageInfo.BytesSize);
                image.ReadPixels(imageInfo, pixelBuffer, imageInfo.RowBytes, 0, 0);
                Marshal.Copy(pixelBuffer, bytes, 0, imageInfo.BytesSize);
            }
            finally
            {
                Marshal.FreeHGlobal(pixelBuffer);
            }

            // Loop over every pixel, RGB -> BGR
            var floats = new float[3 * width * height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    for (var channel = 2; channel >= 0; channel--)
                    {
                        var destIndex = channel * height * width + y * width + x;
                        var sourceIndex = y * imageInfo.RowBytes + x * imageInfo.BytesPerPixel + channel;
                        floats[destIndex] = bytes[sourceIndex] / 255.0f;
                    }
                }
            }
            return floats;
        }

    }
}
