using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Newtonsoft.Json;

using ScoutingData;

namespace ScoutingData.Data
{
	/// <summary>
	/// Type of goal scored.
	/// </summary>
	public enum GoalType
	{
		[Description("Robot Set")]
		RobotSet, // Auto
		[Description("Tote Set (Auto)")]
		YellowToteSet, // Auto [stackable]
		[Description("Container Set (Auto)")]
		ContainerSet, // Auto
		Coopertition, // Sorry FIRST, but that's not a word. [stackable]
		[Description("Tote (Teleop)")]
		GrayTote,
		[Description("Container (Teleop)")]
		ContainerTeleop,
		[Description("Recycled Litter")]
		RecycledLitter, 
		[Description("Landfill Litter")]
		LandfillLitter, 
		[Description("Unprocessed Litter")]
		UnprocessedLitter, // Bonus to opposing team, not a penalty.
	}

	/// <summary>
	/// Goals scored, and all possible attributes
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Goal : IPostJson
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

		/// <summary>
		/// Used for data binding. Sorta like ToString() but more concise.
		/// </summary>
		[JsonIgnore]
		public string UIForm
		{
			get
			{
				string res = string.Format(@"[{0:m\:ss}]", TimeScored);
				res += string.Format(@"[{0:D2}]", PointValue()) + ": ";
				res += Type.GetDescription();	// Alternative to ToString(). See
				return res;						// extension method for details.
			}
		}

		/// <summary>
		/// More data binding. Elaborates upon UIForm.
		/// </summary>
		[JsonIgnore]
		public string UITooltip
		{
			get
			{
				switch (Type)
				{
				case GoalType.RobotSet:
					return "Robot Set";
				case GoalType.YellowToteSet:
					return Stack == true ? "Stacked Tote Set" : "Unstacked Tote Set";
				case GoalType.ContainerSet:
					return "Container Set";
				case GoalType.Coopertition:
					return Stack == true ? "Stacked Coopertition" : "Unstacked Coopertition";
				case GoalType.GrayTote:
					return "Gray Tote Set";
				case GoalType.ContainerTeleop:
					return "Level " + Level.Value.ToString() + " Container";
				case GoalType.RecycledLitter:
					return "Recycled Litter";
				case GoalType.LandfillLitter:
					return "Landfill Litter";
				case GoalType.UnprocessedLitter:
					return "Unprocessed Litter";
				default:
					return null;
				}
			}
		}

		/// <summary>
		/// Time in which the goal was scored. Derived from int form.
		/// </summary>
		[JsonIgnore]
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

		/// <summary>
		/// Reference to scoring team.
		/// </summary>
		[JsonIgnore]
		public Team ScoringTeam
		{ get; set; }

		/// <summary>
		/// Creates an instance of goal. Using the static methods is preferrable.
		/// </summary>
		/// <param name="type">Type of goal scored</param>
		/// <param name="timeScored">Time at which the goal was scored, in seconds</param>
		/// <param name="scorer">Scoring team, null if an alliance or all teams</param>
		/// <param name="scorerAl">Alliance color of scoring team/alliance, null if all teams</param>
		/// <param name="auto">True if scored in autonomous, false if in teleop</param>
		/// <param name="stack">True if a stacked form of the goal, null if inapplicable</param>
		/// <param name="level">
		/// Level the container was at when scored, null for all other
		///	goal types.
		///	</param>
		/// <param name="alliance">True if scored by the entire alliance</param>
		/// <param name="global">True if scored by all participating teams (coopertition)</param>
		internal Goal(GoalType type, int timeScored, Team scorer, AllianceColor? scorerAl, 
			bool auto, bool? stack, int? level, bool alliance, bool global)
		{
			Type = type;
			TimeScoredInt = timeScored;
			ScoringTeam = scorer;
			if (scorer != null)
			{
				ScoringTeamID = scorer.Number;
			}
			ScoringAlliance = scorerAl;
			Autonomous = auto;
			Stack = stack;
			FullAlliance = alliance;
			Global = global;
		}

		public override string ToString()
		{
			return UIForm;
		}

		/// <summary>
		/// Performs additional loading once deserialized from JSON
		/// </summary>
		/// <param name="e">Event to create references from</param>
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

		/// <summary>
		/// Gets the color for which the points from the goal went to.
		/// </summary>
		/// <param name="match">Match to extract data from</param>
		/// <returns>Color for whom the goal's points went to</returns>
		public AllianceColor GetScoringAlliance(Match match)
		{
			if (Global)
			{
				return AllianceColor.Indeterminate;
			}

			if (FullAlliance)
			{
				return ScoringAlliance.Value;
			}

			return match.GetTeamColor(ScoringTeam);
		}

		/// <summary>
		/// Calculates point value of goal
		/// </summary>
		/// <returns>Points for scored goal</returns>
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
		/// <summary>
		/// Instantiates a Robot Set goal (ROBOT SET). [AUTO]
		/// </summary>
		/// <param name="alliance">Alliance who made the robot set</param>
		/// <returns>New Goal of the type Robot Set</returns>
		public static Goal MakeRobotSet(AllianceColor alliance)
		{
			return new Goal(GoalType.RobotSet, Util.TELEOP.CountedSeconds(), null, alliance, 
				true, null, null, true, false);
		}
		/// <summary>
		/// Instantiates a (Yellow) Tote Set goal (TOTE SET). Can be stacked. [AUTO]
		/// </summary>
		/// <param name="stacked">True if totes are stacked (STACKED TOTE SET)</param>
		/// <param name="alliance">Alliance who transported the totes</param>
		/// <returns>New Goal of type Yellow Tote Set</returns>
		public static Goal MakeYellowToteSet(bool stacked, AllianceColor alliance)
		{
			return new Goal(GoalType.YellowToteSet, Util.TELEOP.CountedSeconds(), null, alliance, 
				true, stacked, null, true, false);
		}
		/// <summary>
		/// Instantiates a Container Set goal (CONTAINER SET). [AUTO]
		/// </summary>
		/// <param name="alliance">Alliance who transported the containers</param>
		/// <returns>New Goal of type Container Set</returns>
		public static Goal MakeContainerSet(AllianceColor alliance)
		{
			return new Goal(GoalType.ContainerSet, Util.TELEOP.CountedSeconds(), null, alliance, 
				true, null, null, true, false);
		}
		/// <summary>
		/// Instantiates a Coopertition goal (COOPERTITION).
		/// </summary>
		/// <param name="stacked">True if yellow totes were stacked in coopertition.</param>
		/// <param name="time">Time at which the totes were arranged</param>
		/// <returns>New Goal of type Coopertition</returns>
		public static Goal MakeCoopertition(bool stacked, int time)
		{
			return new Goal(GoalType.Coopertition, time, null, null, time > Util.TELEOP.CountedSeconds(), 
				stacked, null, false, true);
		}
		/// <summary>
		/// Instantiates a Tote goal (TOTE).
		/// </summary>
		/// <param name="team">Team who placed the tote</param>
		/// <param name="time">Time at which the tote was placed</param>
		/// <returns>New Goal of type Gray Tote</returns>
		public static Goal MakeGrayTote(Team team, int time)
		{
			return new Goal(GoalType.GrayTote, time, team, null, false, null, null, false, false);
		}
		/// <summary>
		/// Instantiates a Contanier goal (CONTAINER).
		/// </summary>
		/// <param name="level">Vertical level of the container</param>
		/// <param name="team">Team who placed the container</param>
		/// <param name="time">Time at which the container was placed</param>
		/// <returns>New Goal of type Container</returns>
		public static Goal MakeContainerTeleop(int level, Team team, int time)
		{
			return new Goal(GoalType.ContainerTeleop, time, team, null, false, null, level, false, false);
		}
		/// <summary>
		/// Instantiates a Recycled Litter goal (RECYCLED LITTER).
		/// </summary>
		/// <param name="team">Team who recycled the litter</param>
		/// <param name="time">Time at which the litter was recycled</param>
		/// <returns>New Goal of type Recycled Litter</returns>
		public static Goal MakeRecycledLitter(Team team, int time)
		{
			return new Goal(GoalType.RecycledLitter, time, team, null, false, null, null, false, false);
		}
		/// <summary>
		/// Instantiates a Landfill Litter goal (LANDFILL LITTER).
		/// </summary>
		/// <param name="alliance">Alliance who recycled the litter.</param>
		/// <returns>New Goal of type Landfill Litter</returns>
		public static Goal MakeLandfillLitter(AllianceColor alliance)
		{
			return new Goal(GoalType.LandfillLitter, Util.MATCH_LENGTH.CountedSeconds(), null, alliance, 
				false, null, null, true, false);
		}
		/// <summary>
		/// Instantiates an Unprocessed Litter goal (UNPROCESSED LITTER).
		/// </summary>
		/// <param name="alliance">
		/// Alliance who threw the litter, opposite of alliance whose 
		/// side contains the litter.
		/// </param>
		/// <returns>New Goal of type Unprocessed Litter</returns>
		public static Goal MakeUnprocessedLitter(AllianceColor alliance)
		{
			return new Goal(GoalType.UnprocessedLitter, Util.MATCH_LENGTH.CountedSeconds(), null, alliance, 
				false, null, null, true, false);
		}
		#endregion
	}
}
