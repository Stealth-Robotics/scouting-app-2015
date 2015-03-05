using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	/// <summary>
	/// Five-number summary of a dataset
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	[Obsolete]
	public struct FiveNumberSummary
	{
		/// <summary>
		/// Minimum datum
		/// </summary>
		[JsonProperty]
		public double Min
		{ get; private set; }

		/// <summary>
		/// First Quartile
		/// </summary>
		[JsonProperty]
		public double Q1
		{ get; private set; }

		/// <summary>
		/// Median datum
		/// </summary>
		[JsonProperty]
		public double Med
		{ get; private set; }

		/// <summary>
		/// Third Quartile
		/// </summary>
		[JsonProperty]
		public double Q3
		{ get; private set; }

		/// <summary>
		/// Maximum datum
		/// </summary>
		[JsonProperty]
		public double Max
		{ get; private set; }

		/// <summary>
		/// Creates a new instance of FiveNumberSummary
		/// </summary>
		/// <param name="min">Minumum</param>
		/// <param name="q1">First quartile</param>
		/// <param name="med">Median</param>
		/// <param name="q3">Third quartile</param>
		/// <param name="max">Maximum</param>
		public FiveNumberSummary(double min, double q1, double med, double q3, double max)
			: this()
		{
			Min = min;
			Q1 = q1;
			Med = med;
			Q3 = q3;
			Max = max;
		}

		/// <summary>
		/// Converts the Five-Number Summary to a string
		/// </summary>
		/// <returns>
		/// A string in the form of "[Min Q1 Med Q3 Max]", using values from 
		/// the summary.
		/// </returns>
		public override string ToString()
		{
			return "[" + Min.ToString() + " " + Q1.ToString() + " " + Med.ToString() + 
				" " + Q3.ToString() + " " + Max.ToString() + "]";
		}
	}
}
