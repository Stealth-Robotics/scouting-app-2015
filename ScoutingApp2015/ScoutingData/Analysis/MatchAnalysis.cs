using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	public class MatchAnalysis
	{
		[JsonIgnore]
		public FrcEvent Event
		{ get; private set; }

		[JsonIgnore]
		public Match Match
		{ get; private set; }

		[JsonIgnore]
		public List<TeamAnalysis> TeamAnalyses
		{ get; private set; }

		[JsonProperty]
		public bool Pregame
		{ get; set; }

		#region analysis

		/// <summary>
		/// Mean of red's defense ratings
		/// </summary>
		[JsonProperty]
		public double RedDefenseMean
		{ get; private set; }
		/// <summary>
		/// Mean of blue's defense ratings
		/// </summary>
		[JsonProperty]
		public double BlueDefenseMean
		{ get; private set; }

		/// <summary>
		/// Total count of red's goals
		/// </summary>
		[JsonProperty]
		public int RedGoalCount
		{ get; private set; }
		/// <summary>
		/// Total count of blue's goals
		/// </summary>
		[JsonProperty]
		public int BlueGoalCount
		{ get; private set; }

		/// <summary>
		/// Mean of red's team winrates [pregame]
		/// </summary>
		[JsonProperty]
		public double RedWinRateMean
		{ get; private set; }
		/// <summary>
		/// Mean of blue's team winrates [pregame]
		/// </summary>
		[JsonProperty]
		public double BlueWinRateMean
		{ get; private set; }

		/// <summary>
		/// Expected winner from winrates [pregame]
		/// </summary>
		[JsonProperty]
		public AllianceColor ExpectedWinner
		{ get; private set; }
		/// <summary>
		/// Expected closeness of the game [pregame]
		/// </summary>
		[JsonProperty]
		public double Advantage
		{ get; private set; }

		/// <summary>
		/// Expected final score for red [pregame]
		/// </summary>
		[JsonProperty]
		public double RedExpectedFinalScore
		{ get; private set; }
		/// <summary>
		/// Expected final score for blue [pregame]
		/// </summary>
		[JsonProperty]
		public double BlueExpectedFinalScore
		{ get; private set; }

		/// <summary>
		/// How high-profile the match is [pregame]
		/// </summary>
		[JsonProperty]
		public double GameProfileValue
		{ get; private set; }

		/// <summary>
		/// Total violations in the match
		/// </summary>
		[JsonProperty]
		public int TotalViolations
		{ get; private set; }

		#endregion

		public MatchAnalysis(FrcEvent e) : this(e, null, null)
		{ }

		public MatchAnalysis(FrcEvent e, Match m, List<TeamAnalysis> analyses)
		{
			Event = e;
			Match = m;
			TeamAnalyses = analyses;
		}

		public void CalculateSafe()
		{
			if (Event == null || Match == null || TeamAnalyses.Count == 0)
			{
				Util.DebugLog(LogLevel.Error, "Required Property is null.");
				return;
			}

			CalculatePregame();

			if (!Pregame)
			{
				Calculate();
			}
		}

		public void CalculatePregame()
		{
			// Means of Winrates
			RedWinRateMean = Match.RedAlliance.ToList().ConvertAll<double>((t) =>
			{
				return TeamAnalyses.Find((a) => a.Team == t).WinRate;
			}).Mean();

			BlueWinRateMean = Match.BlueAlliance.ToList().ConvertAll<double>((t) =>
			{
				return TeamAnalyses.Find((a) => a.Team == t).WinRate;
			}).Mean();

			// Expected Winner and Advantage
			double rawAdv = RedWinRateMean - BlueWinRateMean;
			if (rawAdv ==  0)
			{
				ExpectedWinner = AllianceColor.NULL;
			}
			else
			{
				ExpectedWinner = (rawAdv > 0) ? AllianceColor.Red : AllianceColor.Blue;
			}
			Advantage = (rawAdv < 0) ? -rawAdv : rawAdv;

			// Expected Final Scores
			RedExpectedFinalScore = Match.RedAlliance.ToList().ConvertAll<double>((t) =>
			{
				return TeamAnalyses.Find((a) => a.Team == t).FinalScore.Model.Mean;
			}).Sum();

			BlueExpectedFinalScore = Match.BlueAlliance.ToList().ConvertAll<double>((t) =>
			{
				return TeamAnalyses.Find((a) => a.Team == t).FinalScore.Model.Mean;
			}).Sum();

			// Game Profile Value
			double meanWinRateAll = Match.RedAlliance.ToList().Concat(
				Match.BlueAlliance.ToList()).ToList().ConvertAll((t) =>
			{
				return TeamAnalyses.Find((a) => a.Team == t).WinRate;
			}).Mean();
			double inverted = 1 - meanWinRateAll;
			GameProfileValue = 1 / inverted;
		}

		public void Calculate()
		{
			// Defense
			RedDefenseMean = Match.RedDefense.ToList().Mean();
			BlueDefenseMean = Match.BlueDefense.ToList().Mean();

			// Goal Counts
			RedGoalCount = Match.Goals.Count((g) => g.GetScoringAlliance(Match) == AllianceColor.Red);
			BlueGoalCount = Match.Goals.Count((g) => g.GetScoringAlliance(Match) == AllianceColor.Blue);

			// Violations
			TotalViolations = Match.Penalties.Count((g) => g is PenaltyViolation);
		}
	}
}
