using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Sync
{
	public static class MatchSynchronize
	{
		public const int PENALTY_THRESHOLD = 15;
		public const int MATCH_NOT_ENOUGH_DATA_THRESHOLD = 3;

		public static Match FormMatch(FrcEvent e,
			AllianceGroup<RecordedMatch> redData, 
			AllianceGroup<RecordedMatch> blueData)
		{
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
						return !(e.Matches.Exists((m) => m.Number == n && !m.Pregame));
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
			List<PenaltyBase> penalties = new List<PenaltyBase>();
			foreach (RecordedMatch rec in allData)
			{
				foreach (PenaltyBase pen in rec.AlliancePenalties)
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
			int redTeamsCount = redData.Count((rec) => rec != null);
			int blueTeamsCount = blueData.Count((rec) => rec != null);
			if (redTeamsCount + blueTeamsCount < 3 ||
				redTeamsCount == 0 || blueTeamsCount == 0)
			{
				return null; // Give up
			}
			// TODO: More discrepancy calculations

			return result; // TODO: matchmaking
		}
	}
}
