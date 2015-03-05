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
	/// Analytical object for matches. Can be instanced even if match 
	/// is not completed yet.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class MatchAnalysis : IPostJson
	{
		/// <summary>
		/// Reference to the host event
		/// </summary>
		[JsonIgnore]
		public FrcEvent Event
		{ get; private set; }

		/// <summary>
		/// Reference to the corresponding match
		/// </summary>
		[JsonIgnore]
		public Match Match
		{ get; private set; }

		/// <summary>
		/// ID of corresponding match
		/// </summary>
		[JsonProperty]
		public int MatchID
		{ get; private set; }

		/// <summary>
		/// Reference to the list of analyses of the event's teams
		/// </summary>
		[JsonIgnore]
		public List<TeamAnalysis> TeamAnalyses
		{ get; private set; }

		/// <summary>
		/// True if the match has not been completed, in which case this 
		/// analysis is only predictive.
		/// </summary>
		[JsonProperty]
		public bool Pregame
		{ get; set; }

		#region analysis

		/// <summary>
		/// Mean of red's defense ratings
		/// </summary>
		[JsonProperty]
		public double? RedDefenseMean
		{ get; private set; }
		/// <summary>
		/// Mean of blue's defense ratings
		/// </summary>
		[JsonProperty]
		public double? BlueDefenseMean
		{ get; private set; }

		/// <summary>
		/// Total count of red's goals
		/// </summary>
		[JsonProperty]
		public int? RedGoalCount
		{ get; private set; }
		/// <summary>
		/// Total count of blue's goals
		/// </summary>
		[JsonProperty]
		public int? BlueGoalCount
		{ get; private set; }

		#region pregame
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
		#endregion

		#endregion

		/// <summary>
		/// Instantiates a match analysis object with pregame analysis done.
		/// </summary>
		/// <param name="frc">Host event</param>
		/// <param name="m">Corresponding match</param>
		/// <param name="analyses">List of team analyses</param>
		public MatchAnalysis(FrcEvent frc, Match m, List<TeamAnalysis> analyses)
		{
			Event = frc;
			Match = m;
			TeamAnalyses = analyses;
			Pregame = m.Pregame;
			CalculatePregame();
		}

		/// <summary>
		/// A more json-friendly constructor. Doesn't do anything.
		/// </summary>
		[JsonConstructor]
		internal MatchAnalysis()
		{ }

		/// <summary>
		/// Calculates all analysis if required references are ready.
		/// </summary>
		public bool CalculateSafe()
		{
			if (Event == null || Match == null || TeamAnalyses.Count == 0)
			{
				Util.DebugLog(LogLevel.Error, "Required Property is null.");
				return false;
			}

			CalculatePregame();

			if (!Pregame)
			{
				Calculate();
			}

			return true;
		}

		/// <summary>
		/// Calculates all pregame analysis for the match
		/// </summary>
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
				ExpectedWinner = AllianceColor.Indeterminate;
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

		/// <summary>
		/// Calculates all post-game analysis for the match
		/// </summary>
		public void Calculate()
		{
			// Defense
			RedDefenseMean = Match.RedDefense.ToList().Mean();
			BlueDefenseMean = Match.BlueDefense.ToList().Mean();

			// Goal Counts
			RedGoalCount = Match.Goals.Count<Goal>((g) => g.GetScoringAlliance(Match) == AllianceColor.Red);
			BlueGoalCount = Match.Goals.Count<Goal>((g) => g.GetScoringAlliance(Match) == AllianceColor.Blue);
		}

		/// <summary>
		/// Loads the match after instantiated by JSON.
		/// </summary>
		/// <param name="e">Event to load the rest of the match from</param>
		public void PostJsonLoading(FrcEvent e)
		{
			Event = e;
			Match = e.LoadMatch(MatchID);
		}
	}
}
