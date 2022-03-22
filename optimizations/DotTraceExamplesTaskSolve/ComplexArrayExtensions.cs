﻿using System;
using System.Numerics;

namespace DotTraceExamplesTaskSolve
{
	public static class ComplexArrayExtensions
	{
		public static Complex[] DivideByNumber(this Complex[] data, double divisor)
		{
			var result = new Complex[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				var number = data[i];
				var re = number.Real;
				var im = number.Imaginary;
				result[i] = new Complex(re / divisor, im / divisor);
			}

			return result;
		}

		public static double SumModules(this Complex[] data)
		{
			var sum = 0.0;
			foreach (var number in data)
			{
				var im = number.Imaginary;
				var re = number.Real;
				sum += Math.Sqrt(re * re + im * im);
			}

			return sum;
		}
	}
}