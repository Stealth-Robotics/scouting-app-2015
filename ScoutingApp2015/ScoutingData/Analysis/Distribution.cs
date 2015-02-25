using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	/// <summary>
	/// Display Mode for Distributions.
	/// </summary>
	public enum DistributionDisplayMode
	{
		MeanSD,
		FiveNumSummary
	}

	/// <summary>
	/// Overall Distribution for data. Consists of a Normal Model and a 
	/// Five-Number Sumary.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Distribution
	{
		/// <summary>
		/// Preferred display mode as set by the dev. Defaults to Mean & SD
		/// </summary>
		[JsonIgnore]
		public static DistributionDisplayMode DisplayMode = DistributionDisplayMode.MeanSD;

		/// <summary>
		/// Normal Model of this distribution
		/// </summary>
		[JsonProperty]
		public NormalModel Model
		{ get; set; }

		/// <summary>
		/// Five-Number Summary of this distribution
		/// </summary>
		[JsonIgnore]
		public FiveNumberSummary Summary
		{ get; set; }

		/// <summary>
		/// Creates a new instance of Distribution
		/// </summary>
		/// <param name="norm">Normal Model</param>
		/// <param name="fiveNum">Five-Number Summary</param>
		public Distribution(NormalModel norm, FiveNumberSummary fiveNum)
		{
			Model = norm;
			Summary = fiveNum;
		}

		/// <summary>
		/// Summarizes the distribution in the form of a string
		/// </summary>
		/// <returns>String based on the preferred display mode for distributions</returns>
		public override string ToString()
		{
			switch (DisplayMode)
			{
			case DistributionDisplayMode.MeanSD:
				return Model.ToString();
			case DistributionDisplayMode.FiveNumSummary:
				return Summary.ToString();
			default:
				return base.ToString();
			}
		}
	}
}
