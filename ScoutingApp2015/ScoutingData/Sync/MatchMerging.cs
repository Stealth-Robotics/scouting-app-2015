using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Sync
{
	/// <summary>
	/// Static class for merging recorded match data. Not exactly a whole lot
	/// of methods in this one.
	/// </summary>
	public static class MatchMerging
	{
		/// <summary>
		/// Threshold from which to determine penalties as separate. Penalties with
		/// times less than this amount apart are considered the same penalty.
		/// </summary>
		public const int PENALTY_THRESHOLD = 15;
		
		/// <summary>
		/// Threshold for which to determine there is not enough data to form a match.
		/// If there is only this many match recordings present or less, there is not
		/// enough data to form a match.
		/// </summary>
		public const int MATCH_NOT_ENOUGH_DATA_THRESHOLD = 3;

		/// <summary>
		/// Forms a match from recorded data
		/// </summary>
		/// <param name="frc">Event to load additional data from</param>
		/// <param name="redData">Group of recordings from red's data</param>
		/// <param name="blueData">Group of recordings from blue's data</param>
		/// <returns></returns>
		public static Match FormMatch(FrcEvent frc,
			AllianceGroup<RecordedMatch> redData, 
			AllianceGroup<RecordedMatch> blueData)
		{
			// Team data count (reliability)
			int redTeamsCount = redData.Count((rec) => rec != null);
			int blueTeamsCount = blueData.Count((rec) => rec != null);
			if (redTeamsCount + blueTeamsCount < 3 ||
				redTeamsCount == 0 || blueTeamsCount == 0) // not enough data
			{
				return null; // Give up
			}

			Alliance red = new Alliance(redData.A.TrackedTeam, 
				redData.B.TrackedTeam, redData.C.TrackedTeam);
			Alliance blue = new Alliance(blueData.A.TrackedTeam,
				blueData.B.TrackedTeam, blueData.C.TrackedTeam);
			
			List<RecordedMatch> allData = new List<RecordedMatch>();
			foreach (RecordedMatch rec in redData)
			{
				allData.Add(rec);
			}
			foreach (RecordedMatch rec in blueData)
			{
				allData.Add(rec);
			}

			// MATCH ID (quadruple lambda!)
			int matchID = allData.ConvertAll<int>((rec) => rec.MatchNumber).Mode((modes) =>
			{
				try
				{
					return modes.First((n) =>
					{
						return !(frc.Matches.Exists((m) => m.Number == n && !m.Pregame));
					});
				}
				catch (InvalidOperationException)
				{
					Util.DebugLog(LogLevel.Warning, "SYNC", 
						"Completed match with this ID is already present in event");
					return modes.First();
				}
			});

			Match result = new Match(matchID, red, blue);
			result.Pregame = false;
			
			// GOALS LIST
			List<Goal> goals = new List<Goal>();
			foreach (RecordedMatch rec in allData)
			{
				foreach (Goal addedGoal in rec.ScoredGoals)
				{
					#region add by goal type
					switch (addedGoal.Type)
					{
					case GoalType.RobotSet:
						bool alreadyHasRobotSet = goals.Exists((g) => 
						{
							return g.Type == GoalType.RobotSet &&
								g.ScoringAlliance == addedGoal.ScoringAlliance;
						});

						if (!alreadyHasRobotSet)
						{
							goals.Add(addedGoal);
						}
						break;
					case GoalType.YellowToteSet:
						bool alreadyHasYTS = goals.Exists((g) => 
						{
							return g.Type == GoalType.YellowToteSet &&
								g.ScoringAlliance == addedGoal.ScoringAlliance;
						});

						if (!alreadyHasYTS)
						{
							goals.Add(addedGoal);
						}
						break;
					case GoalType.ContainerSet:
						bool alreadyHasContainerSet = goals.Exists((g) =>
						{
							return g.Type == GoalType.ContainerSet &&
								g.ScoringAlliance == addedGoal.ScoringAlliance;
						});

						if (!alreadyHasContainerSet)
						{
							goals.Add(addedGoal);
						}
						break;
					case GoalType.Coopertition:
						bool alreadyHasCoopertition = goals.Exists((g) =>
						{
							return g.Type == GoalType.Coopertition;
						});

						if (!alreadyHasCoopertition)
						{
							goals.Add(addedGoal);
						}
						break;
					case GoalType.GrayTote:
						goals.Add(addedGoal);
						break;
					case GoalType.ContainerTeleop:
						goals.Add(addedGoal);
						break;
					case GoalType.RecycledLitter:
						goals.Add(addedGoal);
						break;
					case GoalType.LandfillLitter:
						goals.Add(addedGoal);
						break;
					case GoalType.UnprocessedLitter:
						goals.Add(addedGoal);
						break;
					default:
						break;
					}
					#endregion
				}
			}
			result.Goals = goals;

			// PENALTIES
			List<Penalty> penalties = new List<Penalty>();
			foreach (RecordedMatch rec in allData)
			{
				foreach (Penalty pen in rec.AlliancePenalties)
				{
					bool nearbyPenalty = penalties.Exists((p) =>
					{
						return Math.Abs(p.TimeOfPenaltyInt - pen.TimeOfPenaltyInt) < PENALTY_THRESHOLD;
					});

					if (!nearbyPenalty)
					{
						penalties.Add(pen);
					}
				}
			}
			result.Penalties = penalties;

			int redGoalScore = result.RedGoals.Aggregate(0, (total, g) => total + g.PointValue());
			redGoalScore = result.RedPenalties.Aggregate(redGoalScore, (total, p) => total + p.ScoreChange());
			int blueGoalScore = result.BlueGoals.Aggregate(0, (total, g) => total + g.PointValue());
			blueGoalScore = result.BluePenalties.Aggregate(blueGoalScore, (total, p) => total + p.ScoreChange());
			
			// WINNER
			List<AllianceColor> winnerRecords = allData.ConvertAll<AllianceColor>((rec) => rec.Winner);
			AllianceColor winner = winnerRecords.Mode((modes) => redGoalScore > blueGoalScore ? 
				AllianceColor.Red : AllianceColor.Blue); // Incredibly unlikely
			result.Winner = winner;

			// WORKING
			result.RedWorking.A = redData.A != null ? redData.A.Working : true;
			result.RedWorking.B = redData.B != null ? redData.B.Working : true;
			result.RedWorking.C = redData.C != null ? redData.C.Working : true;
			result.BlueWorking.A = blueData.A != null ? blueData.A.Working : true;
			result.BlueWorking.B = blueData.B != null ? blueData.B.Working : true;
			result.BlueWorking.C = blueData.C != null ? blueData.C.Working : true;

			// DEFENSE
			result.RedDefense.A = redData.A != null ? redData.A.Defense : 5;
			result.RedDefense.B = redData.B != null ? redData.B.Defense : 5;
			result.RedDefense.C = redData.C != null ? redData.C.Defense : 5;
			result.BlueDefense.A = blueData.A != null ? blueData.A.Defense : 5;
			result.BlueDefense.B = blueData.B != null ? blueData.B.Defense : 5;
			result.BlueDefense.C = blueData.C != null ? blueData.C.Defense : 5;

			// DISCREPANCY POINTS
			int finalScoreRed = (int)redData.ToList().ConvertAll<int>((rec) => rec.AllianceFinalScore).Mean();
			int finalScoreBlue = (int)blueData.ToList().ConvertAll<int>((rec) => rec.AllianceFinalScore).Mean(); // round down
			result.RedFinalScore = finalScoreRed;
			result.BlueFinalScore = finalScoreBlue;

			int dpRed = finalScoreRed - redGoalScore;
			int dpBlue = finalScoreBlue - blueGoalScore;
			result.RedDiscrepancyPoints = dpRed;
			result.BlueDiscrepancyPoints = dpBlue;

			return result;
		}

		/// <summary>
		/// Merges match recordings together after deserialization
		/// </summary>
		/// <param name="frc">Event to load data from</param>
		/// <param name="redA">Team A on Red</param>
		/// <param name="redB">Team B on Red</param>
		/// <param name="redC">Team C on Red</param>
		/// <param name="blueA">Team A on Blue</param>
		/// <param name="blueB">Team B on Blue</param>
		/// <param name="blueC">Team C on Blue</param>
		/// <returns></returns>
		public static Match Merge(FrcEvent frc, RecordedMatch redA, RecordedMatch redB, RecordedMatch redC,
			RecordedMatch blueA, RecordedMatch blueB, RecordedMatch blueC)
		{
			redA.PostJsonLoading(frc);
			redB.PostJsonLoading(frc);
			redC.PostJsonLoading(frc);
			blueA.PostJsonLoading(frc);
			blueB.PostJsonLoading(frc);
			blueC.PostJsonLoading(frc);

			AllianceGroup<RecordedMatch> red = new AllianceGroup<RecordedMatch>(redA, redB, redC);
			AllianceGroup<RecordedMatch> blue = new AllianceGroup<RecordedMatch>(blueA, blueB, blueC);

			return FormMatch(frc, red, blue);
		}
	}
}
