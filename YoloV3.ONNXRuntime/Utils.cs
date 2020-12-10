using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SkiaSharp;
using YoloV3.ONNXRuntime.Models;

namespace YoloV3.ONNXRuntime
{
    public static class Utils
    {
        public static float Sigmoid(float value)
        {
            var k = (float)Math.Exp(value);
            return k / (1.0f + k);
        }
    }
}
