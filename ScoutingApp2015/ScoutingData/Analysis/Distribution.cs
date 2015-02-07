using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	public enum DistributionDisplayMode
	{
		MeanSD,
		FiveNumSummary
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class Distribution
	{
		[JsonIgnore]
		public static DistributionDisplayMode DisplayMode = DistributionDisplayMode.MeanSD;

		[JsonProperty]
		public NormalModel Model
		{ get; set; }

		[JsonIgnore]
		public FiveNumberSummary Summary
		{ get; set; }

		public Distribution(NormalModel norm, FiveNumberSummary fiveNum)
		{
			Model = norm;
			Summary = fiveNum;
		}

		public override string ToString()
		{
			switch (DisplayMode)
			{
			case DistributionDisplayMode.MeanSD:
				return Model.ToString();
			case DistributionDisplayMode.FiveNumSummary:
				return Summary.ToString();
			default:
				throw new ArgumentException("DisplayMode is not a valid state: " + DisplayMode.ToString());
			}
		}
	}
}
