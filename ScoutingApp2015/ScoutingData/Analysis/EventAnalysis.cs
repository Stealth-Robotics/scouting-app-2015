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
		public List<TeamAnalysis> TeamData
		{ get; private set; }

		/// <summary>
		/// List of analyses of the matches
		/// </summary>
		[JsonProperty]
		public List<MatchAnalysis> MatchData
		{ get; private set; }

		public EventAnalysis(FrcEvent frc) : this()
		{
			Event = frc;
			EventName = frc.EventName;

			foreach (Team t in frc.AllTeams)
			{
				TeamAnalysis ta = new TeamAnalysis(Event, t);
				ta.CalculateSafe();
				TeamData.Add(ta);
			}

			foreach (Match m in frc.Matches)
			{
				MatchAnalysis ma = new MatchAnalysis(Event, m, TeamData);
				ma.CalculatePregame();
				MatchData.Add(ma);
			}
		}

		[JsonConstructor]
		public EventAnalysis()
		{
			TeamData = new List<TeamAnalysis>();
			MatchData = new List<MatchAnalysis>();
			EventName = "";
		}

		public override string ToString()
		{
			return "Analysis for " + EventName;
		}

		public void PostJsonLoading(FrcEvent e)
		{
			Event = e;

			foreach (TeamAnalysis ta in TeamData)
			{
				ta.PostJsonLoading(e);
			}

			foreach (MatchAnalysis ma in MatchData)
			{
				ma.PostJsonLoading(e);
			}
		}

		public void Calculate()
		{
			foreach (TeamAnalysis ta in TeamData)
			{
				ta.CalculateSafe();
			}

			foreach (MatchAnalysis ma in MatchData)
			{
				bool worked = ma.CalculateSafe();

				if (!worked)
				{
					Util.DebugLog(LogLevel.Error, "Match analysis for match " +
						ma.MatchID + "calculation failed.");
				}
			}

			foreach (TeamAnalysis ta in TeamData)
			{
				ta.CalculateZScores(TeamData);
			}
		}

		public MatchAnalysis LoadMatch(int matchNum)
		{
			foreach (MatchAnalysis ma in MatchData)
			{
				if (ma.MatchID == matchNum)
				{
					return ma;
				}
			}
			return null;
		}
		public MatchAnalysis LoadMatch(Match match)
		{
			return LoadMatch(match.Number);
		}

		public TeamAnalysis LoadTeam(int teamNum)
		{
			foreach (TeamAnalysis ta in TeamData)
			{
				if (ta.TeamID == teamNum)
				{
					return ta;
				}
			}
			return null;
		}
		public TeamAnalysis LoadTeam(Team team)
		{
			return LoadTeam(team.Number);
		}
	}
}
