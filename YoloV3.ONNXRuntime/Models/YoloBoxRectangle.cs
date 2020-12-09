using System;
using System.Collections.Generic;
using System.Text;

namespace YoloV3.ONNXRuntime.Models
{
    public class YoloBoxRectangle
    {
        public static YoloBoxRectangle Empty => new YoloBoxRectangle(0, 0, 0, 0);
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Point TopLeft => new Point(X, Y);
        public Point TopRight => new Point(X + Width, Y);
        public Point BottomLeft => new Point(X, Y + Height);
        public Point BottomRight => new Point(X + Width, Y + Height);

        public YoloBoxRectangle() { }

        public YoloBoxRectangle(int X, int Y, int Width, int Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

    }
}
