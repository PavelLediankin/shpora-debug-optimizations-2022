using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;

namespace JPEG.Utilities
{
    public static class MathEx
    {
        public static double SumByTwoVariables(int length, Func<int, int, double> function)
        {
            var arr = new double[length * length];
            for(var x = 0; x < length ; x++)
            for (var y = 0; y < length; y++)
                arr[x * length + y] = function(x, y);
            return GetSum(arr);
        }

        private static double GetSum(double[] arr)
        {
            var vector = new Vector<double>();
            var vectorLength = Vector<double>.Count / 2;
            var count = arr.Length - vectorLength;
            for (var i = 0; i < count; i += vectorLength)
            {
                vector = Vector.Add(vector, new Vector<double>(arr, i));
            }
            var sum = 0d;
            for (var i = 0; i < vectorLength; i++)
            {
                sum += vector[i];
            }

            sum += arr[^2] + arr[^1];
            return sum;
        }

        public static void LoopByTwoVariables(int len, Action<int, int> function)
        {
            var tasks = new List<Task>();

            for (var x = 0; x < len; x++)
            {
                var x1 = x;
                tasks.Add(Task.Run(() =>
                {
                    for (var y = 0; y < len; y+=2)
                    {
                        function(x1, y);
                        function(x1, y + 1);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}