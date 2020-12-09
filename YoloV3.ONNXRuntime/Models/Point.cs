using System;
using System.Collections.Generic;
using System.Text;

namespace YoloV3.ONNXRuntime.Models
{
    public class Point
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double Distance(Point p2)
        {
            var distance = (Math.Pow(this.X - p2.X, 2) + Math.Pow(this.Y - p2.Y, 2));
            return distance;
        }

        public bool Intersect(Point[] points)
        {
            if (this.X >= points[0].X && this.X <= points[1].X)
            {
                if (this.Y >= points[0].Y && this.Y <= points[2].Y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
