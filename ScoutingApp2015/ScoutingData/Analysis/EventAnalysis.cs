using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData.Data;

namespace ScoutingData.Analysis
{
	/// <summary>
	/// Root object for the analysis of FRC events
	/// </summary>
	[JsonObject]
	public class EventAnalysis
	{
		/// <summary>
		/// Name of the FRC Event. Should match the name of the FRC Event
		/// object.
		/// </summary>
		[JsonProperty]
		public string EventName
		{ get; set; }

		/// <summary>
		/// Link to FRC Event.
		/// </summary>
		[JsonIgnore]
		public FrcEvent Event
		{ get; set; }

		/// <summary>
		/// List of analyses of the teams
		/// </summary>
		[JsonProperty]
		public List<TeamAnalysis> TeamAnalyses
		{ get; private set; }

		/// <summary>
		/// List of analyses of the matches
		/// </summary>
		[JsonProperty]
		public List<MatchAnalysis> MatchAnalyses
		{ get; private set; }

		public override string ToString()
		{
			return "Analysis for " + EventName;
		}
	}
}
