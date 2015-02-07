using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	[JsonObject(MemberSerialization.OptIn)]
	public struct NormalModel
	{
		[JsonProperty]
		public double Mean
		{ get; private set; }

		[JsonProperty]
		public double SD
		{ get; private set; }

		public NormalModel(double mean, double sd) : this()
		{
			Mean = mean;
			SD = sd;
		}

		public double ScaledCdf(double start, double end)
		{
			double zs = ZScore(start);
			double ze = ZScore(end);

			return Cdf(zs, ze);
		}

		public static double Cdf(double start, double end)
		{
			StatisticFormula formula = new StatisticFormula();

			if (start == double.NegativeInfinity && end == double.PositiveInfinity)
			{
				return 1.0;
			}
			else if (start == double.NegativeInfinity)
			{
				return formula.NormalDistribution(end);
			}
			else if (end == double.PositiveInfinity)
			{
				return formula.InverseNormalDistribution(start);
			}

			double lower = formula.NormalDistribution(start);
			double upper = formula.NormalDistribution(end);
			return upper - lower;
		}

		public double ZScore(double val)
		{
			return (val - Mean) / SD;
		}

		public override string ToString()
		{
			return "N(" + Mean.ToString() + ", " + SD.ToString() + ")";
		}
	}
}
