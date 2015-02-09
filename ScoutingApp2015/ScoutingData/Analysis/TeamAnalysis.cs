using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData.Data;

namespace ScoutingData.Analysis
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TeamAnalysis
	{
		public FrcEvent ParentEvent
		{ get; private set; }

		public Team LinkedTeam
		{ get; set; }

		#region analysis
		
		/// <summary>
		/// Ratio of wins out of total games. [= Wins / Matches.Count]
		/// </summary>
		[JsonProperty]
		public double WinRate
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
		/// Average violations per game. [= Violations / Matches.Count]
		/// </summary>
		[JsonProperty]
		public double ViolationsPerGame
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
		/// Distribution of the defense ratings over matches so far [= Matches.Defense.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution Defense
		{ get; private set; }

		#endregion

		public TeamAnalysis(FrcEvent e) : this(e, null)
		{ }

		public TeamAnalysis(FrcEvent e, Team linked)
		{
			ParentEvent = e;
			LinkedTeam = linked;
		}

		public void CalculateSafe()
		{
			if (ParentEvent == null)
			{
				Util.DebugLog(LogLevel.Error, "Could not calculate team analysis, as ParentEvent is null.");
				return;
			}
			if (LinkedTeam == null)
			{
				Util.DebugLog(LogLevel.Error, "Could not calculate team analysis, as LinkedTeam is null.");
				return;
			}

			Calculate();
		}

		public void Calculate()
		{

			// List of matches the team is in
			List<Match> matches = ParentEvent.Matches.FindAll((m) =>
			{
				return (int)m.GetTeamColor(LinkedTeam) != -1;
			});

			// ================================================================== //

			// Winrate
			int wins = matches.Count((m) =>
			{
				return m.Winner == m.GetTeamColor(LinkedTeam);
			});
			WinRate = (double)wins / (double)(matches.Count);

			// Scored Points
			List<int> scoredPoints = matches.ConvertAll<int>((m) =>
			{
				List<Goal> goals = m.GetGoalsByTeam(LinkedTeam);
				return goals.Aggregate(0, (total, g) => total + g.PointValue());
			});
			ScoredPoints = scoredPoints.MakeDistribution();

			// Final Score
			List<int> finalScores = matches.ConvertAll<int>((m) =>
			{
				if (m.BlueAlliance.Contains(LinkedTeam))
				{
					return m.BlueFinalScore;
				}

				if (m.RedAlliance.Contains(LinkedTeam))
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
				if (m.BlueAlliance.Contains(LinkedTeam))
				{
					return m.BluePenalties.Count((p) => p.TeamAtFault() == LinkedTeam);
				}

				if (m.RedAlliance.Contains(LinkedTeam))
				{
					return m.RedPenalties.Count((p) => p.TeamAtFault() == LinkedTeam);
				}

				return -1;
			});
			penalties.RemoveAll((n) => n == -1);
			Penalties = penalties.MakeDistribution();

			// Violations Per Game
			int violations = matches.Aggregate(0, (total, m) =>
			{
				List<PenaltyBase> localPenal = (m.GetTeamColor(LinkedTeam) == AllianceColor.Red)
					? m.RedPenalties : m.BluePenalties;

				return total + localPenal.Count((p) => p is PenaltyViolation);
			});
			ViolationsPerGame = (double)violations / (double)matches.Count;

			// Working Currently
			WorkingCurrently = matches.Last().GetWorking(LinkedTeam);
			
			// Responsiveness Rate
			int responding = matches.Count((m) => m.GetWorking(LinkedTeam));
			ResponsivenessRate = (double)responding / (double)matches.Count;

			// Defense
			List<int> defense = matches.ConvertAll<int>((m) => m.GetDefense(LinkedTeam));
			Defense = defense.MakeDistribution();
		}
	}
}
