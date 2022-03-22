using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JPEG.Utilities;

namespace JPEG
{
	public class DCT
	{
        private static readonly Vector<double> pi =
            new Vector<double>(new[] { Math.PI, Math.PI, 0, 0 });
        private static readonly Vector<double> vDivider =
            new Vector<double>(new[] { 8d, 8d, 0, 0 });

        /// <summary>
        /// Input Must be square array with width and height == 8
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double[,] DCT2D(double[,] input)
        {
			var width = input.GetLength(1);

            var coeffs = new double[width, width];
            MathEx.LoopByTwoVariables(
				width, (u, v) =>
                {
                    var sum = MathEx
                        .SumByTwoVariables(
                            width,
                            (x, y) => BasisFunction(input[x, y], u, v, x, y));

                    coeffs[u, v] =beta8 * Alpha(u) * Alpha(v) * sum;
                });
			return coeffs;
		}

        /// <summary>
        /// Coeffs Must be square array with width and height == 8
        /// </summary>
        /// <returns></returns>
		public static void IDCT2D(double[,] coeffs, double[,] output)
		{
            var len0 = coeffs.GetLength(0);

            var tasks = new List<Task>();

            for (var x = 0; x < len0; x++)
            {
                for (var y = 0; y < len0; y++)
                {
                    var x1 = x;
                    var y1 = y;
                    var t = Task.Run(() => SumByTwoVariables(output, coeffs, len0, x1, y1));
                    tasks.Add(t);
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static void SumByTwoVariables
            (double[,] output, double[,] coeffs, int len0, int x, int y)
        {
                var sum = MathEx
                    .SumByTwoVariables(
                        len0,
                        (u, v) => BasisFunction(coeffs[u, v], u, v, x, y) * Alpha(u) * Alpha(v));

                output[x, y] = sum * beta8;
        }

        public static double BasisFunction(double a, double u, double v, double x, double y)
        {
            var v1 = new Vector<double>(new []{ x + 0.5, y + 0.5, 0, 0 });
            var v2 = new Vector<double>(new[] {u, v, 0, 0 });

            var v5 = Vector.Multiply(v1, Vector.Multiply(v2, pi));
            var res = Vector.Divide(v5, vDivider);
            var b = Math.Cos(res[0]);
            var c = Math.Cos(res[1]);

            return a * b * c;
		}   

        private const double beta8 = 2d / 8;
        private static readonly double alpha = 1 / Math.Sqrt(2);

        private static double Alpha(int u)
		{
			if(u == 0)
				return alpha;
			return 1;
		}
    }
}