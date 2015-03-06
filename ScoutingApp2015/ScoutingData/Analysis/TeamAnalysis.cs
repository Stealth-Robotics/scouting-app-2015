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
	/// Base analysis object for team.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class TeamAnalysis : IPostJson
	{
		/// <summary>
		/// Reference to hosting event
		/// </summary>
		public FrcEvent Event
		{ get; private set; }

		/// <summary>
		/// Reference to corresponding team
		/// </summary>
		public Team Team
		{ get; set; }

		/// <summary>
		/// ID of corresponding team
		/// </summary>
		[JsonProperty]
		public int TeamID
		{ get; set; }

		#region analysis
		
		/// <summary>
		/// Ratio of wins out of total games. [= Wins / Matches.Count]
		/// </summary>
		[JsonProperty]
		public double WinRate
		{ get; private set; }

		/// <summary>
		/// Z-score of winrate compared to other teams
		/// </summary>
		[JsonProperty]
		public double WinRateZ
		{ get; private set; }

		/// <summary>
		/// Distribution of Scored Points over matches so far. [= Matches.Points.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution ScoredPoints
		{ get; private set; }

		/// <summary>
		/// Distribution of the final scores over matches so far. [= Matches.FinalScore.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution FinalScore
		{ get; private set; }

		/// <summary>
		/// Distribution of the penalties given over matches so far. [= Matches.PenaltyPoints.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution Penalties
		{ get; private set; }

		/// <summary>
		/// Responsiveness in the bot's latest game [= Matches.Last.Responsiveness]
		/// </summary>
		[JsonProperty]
		public bool WorkingCurrently
		{ get; private set; }

		/// <summary>
		/// Responsiveness rate over matches so far [= Matches.CountAll(Responsive) / Matches.Count]
		/// </summary>
		[JsonProperty]
		public double ResponsivenessRate
		{ get; private set; }

		/// <summary>
		/// Z-score of responsiveness rate compared to other teams
		/// </summary>
		[JsonProperty]
		public double ResponsivenessRateZ
		{ get; private set; }

		/// <summary>
		/// Distribution of the defense ratings over matches so far [= Matches.Defense.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution Defense
		{ get; private set; }

		#endregion

		/// <summary>
		/// A bit more json-friendly
		/// </summary>
		internal TeamAnalysis() : this(null, null)
		{ }

		/// <summary>
		/// Do not use. May be deleted in the future.
		/// </summary>
		/// <param name="e">Analysis for teams</param>
		public TeamAnalysis(FrcEvent e) : this(e, null)
		{ }

		/// <summary>
		/// Instantiates an analysis object
		/// </summary>
		/// <param name="e">Host event</param>
		/// <param name="linked">Corresponding team</param>
		public TeamAnalysis(FrcEvent e, Team linked)
		{
			Event = e;
			Team = linked;
		}

		/// <summary>
		/// Completes any references left out when loaded by JSON
		/// </summary>
		/// <param name="e"></param>
		public void PostJsonLoading(FrcEvent e)
		{
			Event = e;
			Team = e.LoadTeam(TeamID);
		}

		/// <summary>
		/// Calculates analysis if necessary references are in place
		/// </summary>
		public void CalculateSafe()
		{
			if (Event == null)
			{
				Util.DebugLog(LogLevel.Error, "Could not calculate team analysis, as ParentEvent is null.");
				return;
			}
			if (Team == null)
			{
				Util.DebugLog(LogLevel.Error, "Could not calculate team analysis, as LinkedTeam is null.");
				return;
			}

			Calculate();
		}

		/// <summary>
		/// Calculates analysis.
		/// </summary>
		public void Calculate()
		{
			// List of matches the team is in
			List<Match> matches = Event.Matches.FindAll((m) =>
			{
				return (int)m.GetTeamColor(Team) != -1;
			});

			// Empty
			if (matches.Count == 0)
			{
				WinRate = -1;
				WinRateZ = -99;
				ScoredPoints = new Distribution(true);
				FinalScore = new Distribution(true);
				Penalties = new Distribution(true);
				WorkingCurrently = true;
				ResponsivenessRate = 1;
				ResponsivenessRateZ = 99;
				Defense = new Distribution(true);
				return;
			}

			// ================================================================== //

			// Winrate
			int wins = matches.Count((m) =>
			{
				return m.Winner == m.GetTeamColor(Team);
			});
			WinRate = (double)wins / (double)(matches.Count);

			// Scored Points
			List<int> scoredPoints = matches.ConvertAll<int>((m) =>
			{
				List<Goal> goals = m.GetGoalsByTeam(Team);
				return goals.Aggregate(0, (total, g) => total + g.PointValue());
			});
			ScoredPoints = scoredPoints.MakeDistribution();

			// Final Score
			List<int> finalScores = matches.ConvertAll<int>((m) =>
			{
				if (m.BlueAlliance.Contains(Team))
				{
					return m.BlueFinalScore;
				}

				if (m.RedAlliance.Contains(Team))
				{
					return m.RedFinalScore;
				}

				return -1;
			});
			finalScores.RemoveAll((n) => n == -1);
			FinalScore = finalScores.MakeDistribution();

			// Penalties
			List<int> penalties = matches.ConvertAll<int>((m) =>
			{
				if (m.Pregame)
				{
					return -1;
				}

				if (m.BlueAlliance.Contains(Team))
				{
					return m.BluePenalties.Count((p) => p.BlamedTeam == Team);
				}

				if (m.RedAlliance.Contains(Team))
				{
					return m.RedPenalties.Count((p) => p.BlamedTeam == Team);
				}

				return -1;
			});
			penalties.RemoveAll((n) => n == -1);
			Penalties = penalties.MakeDistribution();

			// Working Currently
			int maxID = -1;
			Match last = null;
			foreach (Match m in matches)
			{
				if (m.Number > maxID)
				{
					maxID = m.Number;
					last = m;
				}
			}
			if (last != null && !last.Pregame)
			{
				WorkingCurrently = last.GetWorking(Team);
			}
			else if (last != null)
			{
				WorkingCurrently = true;
			}
			
			// Responsiveness Rate
			List<Match> notPregame = (from m in matches
									  where !m.Pregame
									  select m).ToList();
			int responding = notPregame.Count((m) => m.GetWorking(Team));
			ResponsivenessRate = (double)responding / (double)matches.Count;

			// Defense
			List<int> defense = notPregame.ConvertAll<int>((m) => m.GetDefense(Team));
			Defense = defense.MakeDistribution();
		}

		/// <summary>
		/// Calculates z-scores for certain analytics compared to the rest of the data.
		/// </summary>
		/// <param name="all">Other team analyses to compare to</param>
		public void CalculateZScores(IEnumerable<TeamAnalysis> all)
		{
			IEnumerable<double> winrates = from ta in all
										   select ta.WinRate;
			Distribution distWins = winrates.ToList().MakeDistribution();
			WinRateZ = distWins.Model.ZScore(WinRate);

			IEnumerable<double> respRates = from ta in all
										   select ta.ResponsivenessRate;
			Distribution distResp = winrates.ToList().MakeDistribution();
			ResponsivenessRateZ = distResp.Model.ZScore(ResponsivenessRate);

			IEnumerable<Distribution> bigData = from ta in all
												select ta.ScoredPoints;
			ScoredPoints.CalculateZ(bigData);

			bigData = from ta in all select ta.FinalScore;
			FinalScore.CalculateZ(bigData);

			bigData = from ta in all select ta.Penalties;
			Penalties.CalculateZ(bigData);

			bigData = from ta in all select ta.Defense;
			Defense.CalculateZ(bigData);
		}
	}
}
