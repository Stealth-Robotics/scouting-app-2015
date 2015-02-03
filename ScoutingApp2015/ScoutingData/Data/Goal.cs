using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ScoutingData;

namespace ScoutingData.Data
{
	public enum GoalType
	{
		RobotSet, // Auto
		YellowToteSet, // Auto [stackable]
		ContainerSet, // Auto
		Coopertition, // That's not a word. [stackable]
		GrayTote, 
		ContainerTeleop,
		RecycledLitter, 
		LandfillLitter, 
		UnprocessedLitter, // Bonus to opposing team, not a penalty.
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class Goal
	{
		/// <summary>
		/// Primary taxonomy of goals
		/// </summary>
		[JsonProperty]
		public GoalType Type
		{ get; set; }

		/// <summary>
		/// Time Scored (integer form for JSON)
		/// </summary>
		[JsonProperty]
		public int TimeScoredInt
		{ get; set; }

		/// <summary>
		/// Team number scored. Null if FullAlliance is true.
		/// </summary>
		[JsonProperty]
		public int? ScoringTeamID
		{ get; set; }

		/// <summary>
		/// Alliance that scored the alliance-wide goal. Null if FullAlliance is false.
		/// </summary>
		[JsonProperty]
		public AllianceColor? ScoringAlliance
		{ get; set; }

		/// <summary>
		/// Section of the match the goal is scored in. True for autonomous, false for teleop.
		/// </summary>
		[JsonProperty]
		public bool Autonomous
		{ get; set; }

		/// <summary>
		/// Additional points for stacked formation rather than unorganized formation.
		/// </summary>
		[JsonProperty]
		public bool? Stack
		{ get; set; }

		/// <summary>
		/// Height level for recycling containers. Null for all else.
		/// </summary>
		[JsonProperty]
		public int? Level
		{ get; set; }

		/// <summary>
		/// True if the goal is once per game per alliance. False if Global is true.
		/// </summary>
		[JsonProperty]
		public bool FullAlliance
		{ get; set; }

		/// <summary>
		/// True if goal applies to both alliances, once per match.
		/// </summary>
		[JsonProperty]
		public bool Global
		{ get; set; }

		public TimeSpan TimeScored
		{
			get
			{
				return TimeSpan.FromSeconds((double)TimeScoredInt);
			}
			set
			{
				TimeScoredInt = value.CountedSeconds();
			}
		}

		public Team ScoringTeam
		{ get; set; }

		internal Goal(GoalType type, int timeScored, Team scorer, AllianceColor? scorerAl, 
			bool auto, bool? stack, int? level, bool alliance, bool global)
		{
			Type = type;
			TimeScoredInt = timeScored;
			ScoringTeam = scorer;
			ScoringTeamID = scorer.Number;
			ScoringAlliance = scorerAl;
			Autonomous = auto;
			Stack = stack;
			FullAlliance = alliance;
			Global = global;
		}

		public void PostJsonLoading(FrcEvent e)
		{
			if (ScoringTeamID.HasValue)
			{
				ScoringTeam = e.LoadTeam(ScoringTeamID.Value);
			}
			else
			{
				ScoringTeam = null;
			}
		}

		public int PointValue()
		{
			try
			{
				switch (Type)
				{
				case GoalType.RobotSet:
					return 4;
				case GoalType.YellowToteSet:
					return Stack.Value ? 20 : 6;
				case GoalType.ContainerSet:
					return 8;
				case GoalType.Coopertition:
					return Stack.Value ? 40 : 20;
				case GoalType.GrayTote:
					return 2;
				case GoalType.ContainerTeleop:
					return 4 * Level.Value;
				case GoalType.RecycledLitter:
					return 6;
				case GoalType.LandfillLitter:
					return 1;
				case GoalType.UnprocessedLitter:
					return 4;
				default:
					return 0;
				}
			}
			catch (NullReferenceException e)
			{
				Util.DebugLog(LogLevel.Error, "DATA", e.Message);
				return 0;
			}
		}

		#region wizards
		public static Goal MakeRobotSet(AllianceColor alliance)
		{
			return new Goal(GoalType.RobotSet, Util.TELEOP.CountedSeconds(), null, alliance, 
				true, null, null, true, false);
		}
		public static Goal MakeYellowToteSet(bool stacked, AllianceColor alliance)
		{
			return new Goal(GoalType.YellowToteSet, Util.TELEOP.CountedSeconds(), null, alliance, 
				true, stacked, null, true, false);
		}
		public static Goal MakeContainerSet(AllianceColor alliance)
		{
			return new Goal(GoalType.YellowToteSet, Util.TELEOP.CountedSeconds(), null, alliance, 
				true, null, null, true, false);
		}
		public static Goal MakeCoopertition(bool stacked, int time)
		{
			return new Goal(GoalType.Coopertition, time, null, null, time < Util.TELEOP.CountedSeconds(), 
				stacked, null, false, true);
		}
		public static Goal MakeGrayTote(Team team, int time)
		{
			return new Goal(GoalType.GrayTote, time, team, null, false, null, null, false, false);
		}
		public static Goal MakeContainerTeleop(int level, Team team, int time)
		{
			return new Goal(GoalType.ContainerTeleop, time, team, null, false, null, level, false, false);
		}
		public static Goal MakeRecycledLitter(Team team, int time)
		{
			return new Goal(GoalType.RecycledLitter, time, team, null, false, null, null, false, false);
		}
		public static Goal MakeLandfillLitter(AllianceColor alliance)
		{
			return new Goal(GoalType.LandfillLitter, 0, null, alliance, false, null, null, true, false);
		}
		public static Goal MakeUnprocessedLitter(AllianceColor alliance)
		{
			return new Goal(GoalType.UnprocessedLitter, 0, null, alliance, false, null, null, true, false);
		}
		#endregion
	}
}
