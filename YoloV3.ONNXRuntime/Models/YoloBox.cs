using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoloV3.ONNXRuntime.Models
{
    public class YoloBox
    {

        public string Label { get; private set; }
        public float BoxConfidence { get; private set; }


        private float[] anchors = new float[]
        {
            10.13F,  16.30F,  33.23F,  30.61F,  62.45F,  59.119F,  116.90F,  156.198F,  373.326F
        };
        private float modelX { get; set; }
        private float modelY { get; set; }
        private float modelWidth { get; set; }
        private float modelHeight { get; set; }


        public YoloBoxRectangle Rectangle
        {
            get
            {
                var cellWidth = (int)Constants.YoloImage.Width / columns;
                var cellHeight = (int)Constants.YoloImage.Height / rows;

                var w = (int)(Math.Exp(modelWidth) * cellWidth * anchors[box * 2]);
                var h = (int)(Math.Exp(modelHeight) * cellHeight * anchors[box * 2 + 1]);
                return new YoloBoxRectangle()
                {
                    X = (int)((column + Utils.Sigmoid(modelX)) * cellWidth) - (w / 2),
                    Y = (int)((row + Utils.Sigmoid(modelY)) * cellHeight) - (h / 2),
                    Width = (int)w,
                    Height = (int)h,
                };
            }
        }



        private float[] classesConfidences { get; set; }
        private float topScoreClassConfidence { get; set; }
        private int column { get; set; }
        private int row { get; set; }
        private int rows { get; set; }
        private int columns { get; set; }
        private int box { get; set; }
        public YoloBox(float modelX, float modelY, float modelWidth, float modelHeight, int box, int columns, int rows, int column, int row, float boxConfidence, float[] classesConfidences, string label)
        {
            this.modelX = modelX;
            this.modelY = modelY;
            this.modelWidth = modelWidth;
            this.modelHeight = modelHeight;
            this.BoxConfidence = boxConfidence;
            this.classesConfidences = classesConfidences;
            this.topScoreClassConfidence = classesConfidences.Max();
            this.Label = label;

            this.rows = rows;
            this.columns = columns;
            this.column = column;
            this.row = row;
        }

    }
}
