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
	public class EventAnalysis : IPostJson
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

		public EventAnalysis(FrcEvent frc) : this()
		{
			Event = frc;
			EventName = frc.EventName;

			foreach (Team t in frc.AllTeams)
			{
				TeamAnalyses.Add(new TeamAnalysis(Event, t));
			}

			foreach (Match m in frc.Matches)
			{
				MatchAnalyses.Add(new MatchAnalysis(Event, m, TeamAnalyses));
			}
		}

		[JsonConstructor]
		public EventAnalysis()
		{
			TeamAnalyses = new List<TeamAnalysis>();
			MatchAnalyses = new List<MatchAnalysis>();
			EventName = "";
		}

		public override string ToString()
		{
			return "Analysis for " + EventName;
		}

		public void PostJsonLoading(FrcEvent e)
		{
			Event = e;

			foreach (TeamAnalysis ta in TeamAnalyses)
			{
				ta.PostJsonLoading(e);
			}

			foreach (MatchAnalysis ma in MatchAnalyses)
			{
				ma.PostJsonLoading(e);
			}
		}

		public void Calculate()
		{
			foreach (TeamAnalysis ta in TeamAnalyses)
			{
				ta.CalculateSafe();
			}

			foreach (MatchAnalysis ma in MatchAnalyses)
			{
				bool worked = ma.CalculateSafe();

				if (!worked)
				{
					Util.DebugLog(LogLevel.Error, "Match analysis for match " +
						ma.MatchID + "calculation failed.");
				}
			}
		}
	}
}
