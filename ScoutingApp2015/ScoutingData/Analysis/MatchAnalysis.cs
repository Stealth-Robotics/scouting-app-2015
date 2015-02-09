using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Analysis
{
	public class MatchAnalysis
	{
		public FrcEvent Event
		{ get; private set; }

		public Match Match
		{ get; private set; }

		public List<TeamAnalysis> TeamAnalyses
		{ get; private set; }

		#region analysis

		

		#endregion

		public MatchAnalysis(FrcEvent e) : this(e, null, null)
		{ }

		public MatchAnalysis(FrcEvent e, Match m, List<TeamAnalysis> analyses)
		{
			Event = e;
			Match = m;
			TeamAnalyses = analyses;
		}
	}
}
