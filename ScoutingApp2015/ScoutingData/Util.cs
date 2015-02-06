﻿using ScoutingData.Analysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData
{
	public enum LogLevel
	{
		Info,
		Warning,
		Error,
		Critical
	}

	public static class Util
	{
		internal static readonly IFormatProvider DEF_FORMAT = 
			CultureInfo.CurrentCulture.NumberFormat;

		/// <summary>
		/// Length of time for an FRC 2015 match, in the form of a TimeSpan object
		/// </summary>
		public static readonly TimeSpan MATCH_LENGTH = new TimeSpan(0, 1, 30);
		/// <summary>
		/// Time left when teleop starts
		/// </summary>
		public static readonly TimeSpan TELEOP = new TimeSpan(0, 1, 15);

		/// <summary>
		/// Clamps a value between a min and max (inclusively), and returns if clamping was necessary.
		/// </summary>
		/// <typeparam name="T">Type being compared when clamping. Must extend IComparable</typeparam>
		/// <param name="val">Value to clamp</param>
		/// <param name="min">Minimum</param>
		/// <param name="max">Maximum</param>
		/// <returns></returns>
		public static bool Clamp<T>(T val, T min, T max) where T : IComparable<T>
		{
			bool needed = false;

			if (val.CompareTo(min) < 0)
			{
				needed = true;
				val = min;
			}
			if (val.CompareTo(max) > 0)
			{
				needed = true;
				val = max;
			}

			return needed;
		}

		/// <summary>
		/// Shortcut for Debug Logging
		/// </summary>
		/// <param name="level">Seriousness of message</param>
		/// <param name="message">Message logged</param>
		public static void DebugLog(LogLevel level, string message)
		{
			System.Diagnostics.Debugger.Log((int)level, "SCOUTING", 
				"\n[" + level.ToString().ToUpper() + "] " + message);
		}
		/// <summary>
		/// Shortcut for Debug Logging, with category
		/// </summary>
		/// <param name="level">Seriousness of message</param>
		/// <param name="category">Section of app with log</param>
		/// <param name="message">Message logged</param>
		public static void DebugLog(LogLevel level, string category, string message)
		{
			DebugLog(level, "[" + category + "] " + message);
		}

		///////////////////////
		// Extension Methods //
		///////////////////////

		/// <summary>
		/// Extension method, converting timespans into seconds left, for convenience
		/// </summary>
		/// <param name="ts">Object to convert</param>
		/// <returns>Total seconds left in the timespan</returns>
		public static int CountedSeconds(this TimeSpan ts)
		{
			return (3600 * ts.Hours) + (60 * ts.Minutes) + ts.Seconds;
		}

		public static double Mean<T>(this IList<T> list) 
			where T : IConvertible
		{
			decimal total = 0;
			foreach (T t in list)
			{
				total += t.ToDecimal(DEF_FORMAT);
			}

			total /= (decimal)(list.Count);
			return (double)total;
		}

		public static double StandardDeviation<T>(this IList<T> list) 
			where T : IConvertible
		{
			return list.StandardDeviation(list.Mean());
		}
		public static double StandardDeviation<T>(this IList<T> list, double mean)
			where T : IConvertible
		{
			decimal sigmaDeviations = 0;
			foreach (T t in list)
			{
				decimal val = t.ToDecimal(DEF_FORMAT);
				decimal deviation = val - (decimal)mean;
				decimal devSq = deviation * deviation;
				sigmaDeviations += devSq;
			}

			int nMinusOne = list.Count - 1;
			double variance = (double)sigmaDeviations / (double)nMinusOne;
			return Math.Sqrt(variance);
		}

		public static FiveNumberSummary Get5NS<T>(this IList<T> list)
			where T : IConvertible
		{
			T[] sortedArr = list.OrderBy((t) => t).ToArray();
			double med = sortedArr.Median();
			
			int half = sortedArr.Length / 2;
			T[] lower = new T[half];
			for (int i = 0; i < half; i++)
			{
				lower[i] = sortedArr[i];
			}
			T[] upper = new T[half];
			sortedArr.CopyTo(upper, half);

			double q1 = lower.Median();
			double q3 = upper.Median();

			double min = sortedArr[0].ToDouble(DEF_FORMAT);
			double max = sortedArr[sortedArr.Length - 1].ToDouble(DEF_FORMAT);

			return new FiveNumberSummary(min, q1, med, q3, max);
		}

		public static double Median<T>(this T[] arr)
			where T : IConvertible
		{
			int count = arr.Count();
			bool evenLen = count % 2 == 0;
			int lowerId = count / 2;
			int upperId = evenLen ? lowerId + 1 : lowerId;
			double lowMed = arr[lowerId].ToDouble(DEF_FORMAT);
			double upMed = arr[upperId].ToDouble(DEF_FORMAT);
			double meanMed = (lowMed + upMed) / 2.0;
			return meanMed;
		}
		public static double Median<T>(this IList<T> list)
			where T : IConvertible
		{
			return list.ToArray().Median();
		}
	}
}
