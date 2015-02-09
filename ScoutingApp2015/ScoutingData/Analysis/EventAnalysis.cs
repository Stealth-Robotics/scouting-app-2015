using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	[JsonObject]
	public class EventAnalysis
	{
		[JsonProperty]
		public string EventName
		{ get; set; }

		[JsonProperty]
		public List<TeamAnalysis> TeamAnalyses
		{ get; private set; }
	}
}
