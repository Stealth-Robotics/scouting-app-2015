using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	/// <summary>
	/// Normal model for the distribution of a dataset
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public struct NormalModel
	{
		/// <summary>
		/// Mean of the data
		/// </summary>
		[JsonProperty]
		public double Mean
		{ get; private set; }

		/// <summary>
		/// Standard deviation of the data
		/// </summary>
		[JsonProperty]
		public double SD
		{ get; private set; }

		/// <summary>
		/// Instantiates a Normal Model object
		/// </summary>
		/// <param name="mean">Mean of the dataset</param>
		/// <param name="sd">Standard deviation of the dataset</param>
		public NormalModel(double mean, double sd) : this()
		{
			Mean = mean;
			SD = sd;
		}

		/// <summary>
		/// Scaled cumulative density function using the model
		/// </summary>
		/// <param name="start">Start point of the accumulation</param>
		/// <param name="end">End point of the accumulation</param>
		/// <returns>Percent of the data predicted to fall within the range</returns>
		public double ScaledCdf(double start, double end)
		{
			double zs = ZScore(start);
			double ze = ZScore(end);

			return Cdf(zs, ze);
		}

		/// <summary>
		/// Unscaled cumulative density function based on z-scores
		/// </summary>
		/// <param name="start">Start point of the accumulation</param>
		/// <param name="end">End point of the accumulation</param>
		/// <returns>Percent of the data predicted to fall between the two z-scores</returns>
		public static double Cdf(double start, double end)
		{
			if (start == double.NegativeInfinity && end == double.PositiveInfinity)
			{
				return 1.0;
			}
			else if (start == double.NegativeInfinity)
			{
				return Util.Stats.NormalDistribution(end);
			}
			else if (end == double.PositiveInfinity)
			{
				return Util.Stats.InverseNormalDistribution(start);
			}

			double lower = Util.Stats.NormalDistribution(start);
			double upper = Util.Stats.NormalDistribution(end);
			return upper - lower;
		}

		/// <summary>
		/// Gets z-score for a value using the model
		/// </summary>
		/// <param name="val">Value to get the z-score for</param>
		/// <returns>Z-score of the value</returns>
		public double ZScore(double val)
		{
			return (val - Mean) / SD;
		}

		/// <summary>
		/// Converts the model to a string in the format of "N(mean, sd)", as is
		/// standard mathematical notation.
		/// </summary>
		/// <returns>String form of model</returns>
		public override string ToString()
		{
			return "N(" + Mean.ToString() + ", " + SD.ToString() + ")";
		}
	}
}
