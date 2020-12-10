using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SkiaSharp;
using YoloV3.ONNXRuntime;
using YoloV3.ONNXRuntime.Models;

namespace Tests
{
    [TestFixture]
    public class BoundingBoxesTests
    {
        private readonly YoloV3Onnx _yoloV3Onnx = new YoloV3Onnx(Path.Combine("Model", "yolov3.onnx"), Path.Combine("Model", "labels.txt"));
        [Test,TestCaseSource(typeof(BoundingBoxesTests), nameof(ImageCases))]
        public void ShouldDetectBoundingBoxes_ReasonablyWell(string image, List<YoloBoxRectangle> expectedRectangles)
        {
            var skImage = SKImage.FromEncodedData(new MemoryStream(File.ReadAllBytes(image)));
            var result = _yoloV3Onnx.Detect(skImage);

            var averageScore = CalculateAverageScore(result, expectedRectangles);

            Assert.GreaterOrEqual(averageScore,0.8);
        }

        private float CalculateAverageScore(IList<YoloBoxRectangle> result, List<YoloBoxRectangle> expectedRectangles)
        {
            var scores = new List<float>();
            for (int i = 0; i < Math.Abs(result.Count - expectedRectangles.Count); i++)
            {
                scores.Add(0);
            }

            foreach (var rect in expectedRectangles)
            {
                scores.Add(result.Max(c => c.IntersectionOverUnion(rect)));
            }

            return scores.Average();
        }


        private static IEnumerable ImageCases()
        {
            yield return new TestCaseData(Path.Combine("images", "sample1.jpg"), 
                new List<YoloBoxRectangle>()
                {
                    new YoloBoxRectangle(275,324,7,19),
                    new YoloBoxRectangle(87,343,15,43),
                    new YoloBoxRectangle(265,299,11,46),
                    new YoloBoxRectangle(366,305,9,31),
                    new YoloBoxRectangle(197,311,11,57),
                    new YoloBoxRectangle(411,294,4,43)
                    ,
                } );
        }
    }
}