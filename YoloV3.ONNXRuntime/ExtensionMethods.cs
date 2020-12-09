using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
