using ScoutingData.Analysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

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
		public static readonly string USERPROFILE =
			Environment.GetEnvironmentVariable("USERPROFILE");

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
		/// Returns StatisticsFormula object for special stats functions
		/// </summary>
		public static StatisticFormula Stats
		{
			get
			{
				Chart chart = new Chart();
				return chart.DataManipulator.Statistics;

			}
		}

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

		/// <summary>
		/// Adds a value to the dictionary, overwriting if the key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type in the Dictionary</typeparam>
		/// <typeparam name="TVal">Value type in the Dictionary</typeparam>
		/// <param name="dict">IDictionary to add stuff to</param>
		/// <param name="key">Key to set or add</param>
		/// <param name="val">Value the key is set to</param>
		public static void AddSet<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key, TVal val)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = val;
			}
			else
			{
				dict.Add(key, val);
			}
		}

		/// <summary>
		/// Adds a value to the dictionary, incrementing by an amount if the key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type of dictionary</typeparam>
		/// <typeparam name="TVal">Value type of dictionary. Must be numeric.</typeparam>
		/// <param name="dict">Dictionary to do this with</param>
		/// <param name="key">Key for whose value to increment</param>
		/// <param name="incVal">Amount to increment by</param>
		/// <param name="startVal">Value to start at when adding new pairs</param>
		public static void AddIncrement<TKey, TVal>(this IDictionary<TKey, TVal> dict, 
			TKey key, double incVal, double startVal) where TVal : IConvertible, new()
		{
			if (dict.ContainsKey(key))
			{
				// this is how you increment generic types
				IConvertible conv = dict[key].ToDouble(DEF_FORMAT) + 1.0;
				dict[key] = (TVal)conv.ToType(typeof(TVal), DEF_FORMAT);
			}
			else
			{
				// this is how you instantiate generic types (that implement IConvertible)
				IConvertible start = (IConvertible)startVal;
				dict.Add(key, (TVal)start.ToType(typeof(TVal), DEF_FORMAT));
			}
		}

		/// <summary>
		/// Adds a value to the dictionary with a key of zero, incrementing by one if the 
		/// key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type of dictionary</typeparam>
		/// <typeparam name="TVal">Value type of dictionary. Must be numeric.</typeparam>
		/// <param name="dict">Dictionary to do this with</param>
		/// <param name="key">Key for whose value to increment</param>
		public static void AddIncrement<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key)
			where TVal : IConvertible, new()
		{
			dict.AddIncrement(key, 1, 0);
		}

		#region STATISTICS

		public static double Sum<T>(this IList<T> list)
			where T : IConvertible
		{
			double total = 0;
			foreach (T t in list)
			{
				total += t.ToDouble(DEF_FORMAT);
			}

			return total;
		}

		/// <summary>
		/// Calculates the mean value in the list and converts it to a double
		/// </summary>
		/// <param name="list">list the data is taken from</param>
		/// <returns>mean of the values</returns>
		public static double Mean<T>(this IList<T> list)
			where T : IConvertible
		{
			double total = list.Sum();

			total /= (double)(list.Count);
			return (double)total;
		}

		/// <summary>
		/// Calculates the standard deviation of the list of data
		/// </summary>
		/// <param name="list">list the data is taken from</param>
		/// <returns>standard deviation of the values</returns>
		public static double StandardDeviation<T>(this IList<T> list)
			where T : IConvertible
		{
			return list.StandardDeviation(list.Mean());
		}
		/// <summary>
		/// Calculates the standard deviation of the list of data
		/// </summary>
		/// <param name="list">list the data is taken from</param>
		/// <param name="mean">mean of the data, for faster calculation</param>
		/// <returns>standard deviation of the values</returns>
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

		/// <summary>
		/// Calculates the 5-number summary of the data (min-Q1-med-Q3-max)
		/// </summary>
		/// <param name="list">list the data is taken from</param>
		/// <returns>struct containing the values from the 5-number summary</returns>
		public static FiveNumberSummary Get5NS<T>(this IList<T> list)
			where T : IConvertible
		{
			T[] sortedArr = list.OrderBy(t => t).ToArray();
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

		/// <summary>
		/// Used to calculate the median of an array
		/// </summary>
		/// <param name="arr">array the data is taken from</param>
		/// <returns>median value in the array</returns>
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
		/// <summary>
		/// Used to calculate the median of an array
		/// </summary>
		/// <param name="arr">list the data is taken from</param>
		/// <returns>median value in the array</returns>
		public static double Median<T>(this IList<T> list)
			where T : IConvertible
		{
			return list.ToArray().Median();
		}

		/// <summary>
		/// Creates a Distribution object by analyzing the distribution of the list.
		/// </summary>
		/// <param name="list">List to be analyzed</param>
		/// <returns>Distribution object about the list's distribution</returns>
		public static Distribution MakeDistribution<T>(this IList<T> list)
			where T : IConvertible
		{
			double mean = list.Mean();
			NormalModel norm = new NormalModel(mean, list.StandardDeviation(mean));
			FiveNumberSummary sum = list.Get5NS();

			return new Distribution(norm, sum);
		}

		public static Dictionary<T, int> Frequencies<T>(this IList<T> list)
		{
			Dictionary<T, int> res = new Dictionary<T, int>();

			foreach (T t in list)
			{
				res.AddIncrement(t);
			}

			return res;
		}

		public static T Mode<T>(this IList<T> list, Func<List<T>, T> inCaseOfTie)
		{
			Dictionary<T, int> frequencies = list.Frequencies();

			int maxCount = 0;
			List<T> maxes = new List<T>();
			foreach (KeyValuePair<T, int> kvp in frequencies)
			{
				if (maxes.Count == 0)
				{
					maxes.Add(kvp.Key);
					maxCount = kvp.Value;
					continue; // NEXT!
				}

				if (kvp.Value >= maxCount)
				{
					if (kvp.Value > maxCount)
					{
						maxes.Clear();
						maxCount = kvp.Value;
					}

					maxes.Add(kvp.Key);
				}
			}

			return inCaseOfTie(maxes);
		}

		#endregion

		/// <summary>
		/// Quick function for converting numbers from 0 to 1 into percentages
		/// </summary>
		/// <param name="n">number to convert</param>
		/// <returns>percentage string</returns>
		public static string ToStringPct(this double n)
		{
			return (n * 100).ToString() + "%";
		}
	}
}
