using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Analysis
{
	public struct NormalModel
	{
		public double Mean
		{ get; private set; }

		public double SD
		{ get; private set; }

		public NormalModel(double mean, double sd) : this()
		{
			Mean = mean;
			SD = sd;
		}

		public double ScaledCdf(double start, double end)
		{
			return 0.0; // TODO: do this
		}

		public static double Cdf(double start, double end)
		{
			return 0.0; // TODO: program NormalCdf() in C#
		}

		public double zScore(double val)
		{
			return (val - Mean) / SD;
		}

		public override string ToString()
		{
			return "N(" + Mean.ToString() + ", " + SD.ToString() + ")";
		}
	}
}
