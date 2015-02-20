using ScoutingData.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ScoutingData
{
	/// <summary>
	/// Additional statistics-related functions and extension methods
	/// </summary>
	public static class Stats
	{
		/// <summary>
		/// Returns StatisticsFormula object for special stats functions
		/// </summary>
		public static StatisticFormula StatsFormula
		{
			get
			{
				Chart chart = new Chart();
				return chart.DataManipulator.Statistics;

			}
		}

		public static double Sum<T>(this IList<T> list)
			where T : IConvertible
		{
			double total = 0;
			foreach (T t in list)
			{
				total += t.ToDouble(Util.DEF_FORMAT);
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
				decimal val = t.ToDecimal(Util.DEF_FORMAT);
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

			double min = sortedArr[0].ToDouble(Util.DEF_FORMAT);
			double max = sortedArr[sortedArr.Length - 1].ToDouble(Util.DEF_FORMAT);

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
			double lowMed = arr[lowerId].ToDouble(Util.DEF_FORMAT);
			double upMed = arr[upperId].ToDouble(Util.DEF_FORMAT);
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
	}
}
