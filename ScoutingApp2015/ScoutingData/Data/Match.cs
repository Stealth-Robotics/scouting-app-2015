using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScoutingData.Data
{
	/// <summary>
	/// Match object used to track all non-analysis data from a match
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Match : IPostJson
	{
		/// <summary>
		/// Match Number within the competition event [pregame]
		/// </summary>
		[JsonProperty]
		public int Number
		{ get; set; }

		/// <summary>
		/// Whether the match is completed with appropriate data [pregame=false]
		/// </summary>
		[JsonProperty]
		public bool Pregame
		{ get; set; }

		/// <summary>
		/// Red Alliance [pregame]
		/// </summary>
		[JsonProperty]
		public Alliance RedAlliance
		{ get; set; }

		/// <summary>
		/// Blue Alliance [pregame]
		/// </summary>
		[JsonProperty]
		public Alliance BlueAlliance
		{ get; set; }

		/// <summary>
		/// List of goals scored by all teams
		/// </summary>
		[JsonProperty]
		public List<Goal> Goals
		{ get; set; }

		/// <summary>
		/// All of Blue's goals
		/// </summary>
		public List<Goal> BlueGoals
		{
			get
			{
				return Goals.FindAll((g) => g.GetScoringAlliance(this) == AllianceColor.Blue || 
					g.GetScoringAlliance(this) == AllianceColor.Indeterminate);
			}
		}

		/// <summary>
		/// All of Red's goals
		/// </summary>
		public List<Goal> RedGoals
		{
			get
			{
				return Goals.FindAll((g) => g.GetScoringAlliance(this) == AllianceColor.Red ||
					g.GetScoringAlliance(this) == AllianceColor.Indeterminate);
			}
		}

		/// <summary>
		/// List of penalties on both sides
		/// </summary>
		[JsonProperty]
		public List<Penalty> Penalties
		{ get; set; }

		/// <summary>
		/// All penalties AGAINST Blue
		/// </summary>
		public List<Penalty> BluePenalties
		{
			get
			{
				return Penalties.FindAll((p) => p.PenalizedAlliance == AllianceColor.Blue);
			}
		}

		/// <summary>
		/// All penalties AGAINST Red
		/// </summary>
		public List<Penalty> RedPenalties
		{
			get
			{
				return Penalties.FindAll((p) => p.PenalizedAlliance == AllianceColor.Red);
			}
		}

		/// <summary>
		/// Calculated as the mean of recorded match final scores for red.
		/// </summary>
		public int RedFinalScore
		{ get; set; }

		/// <summary>
		/// Calculated as the mean of recorded match final scores for blue.
		/// </summary>
		public int BlueFinalScore
		{ get; set; }

		/// <summary>
		/// Discrepancy points for when scouting data is inaccurate or missing.
		/// </summary>
		[JsonProperty]
		public int RedDiscrepancyPoints
		{ get; set; }

		/// <summary>
		/// Discrepancy points for when scouting data is inaccurate or missing.
		/// </summary>
		[JsonProperty]
		public int BlueDiscrepancyPoints
		{ get; set; }

		/// <summary>
		/// Input from all collected match data. Recorded from majority.
		/// Used to calculate discrepancy points.
		/// </summary>
		[JsonProperty]
		public AllianceColor Winner
		{ get; set; }

		/// <summary>
		/// Used to determine which bots are working (red)
		/// </summary>
		[JsonProperty]
		public AllianceGroup<bool> RedWorking
		{ get; set; }

		/// <summary>
		/// Used to determine which bots are working (blue)
		/// </summary>
		[JsonProperty]
		public AllianceGroup<bool> BlueWorking
		{ get; set; }

		/// <summary>
		/// Defense ratings for red, from 0-10
		/// </summary>
		[JsonProperty]
		public AllianceGroup<int> RedDefense
		{ get; set; }

		/// <summary>
		/// Defense ratings for blue, from 0-10
		/// </summary>
		[JsonProperty]
		public AllianceGroup<int> BlueDefense
		{ get; set; }

		/// <summary>
		/// Creates a (pregame) match setup. All postgame data is put in later.
		/// </summary>
		/// <param name="num">Match number</param>
		/// <param name="red">Red alliance</param>
		/// <param name="blue">Blue alliance</param>
		public Match(int num, Alliance red, Alliance blue)
		{
			Number = num;
			RedAlliance = red;
			BlueAlliance = blue;
			Pregame = true;
		}

		/// <summary>
		/// A more DataGrid-friendly constructor
		/// </summary>
		public Match() : this(0, new Alliance(), new Alliance())
		{ }

		/// <summary>
		/// Additional loading once deserialization is complete
		/// </summary>
		/// <param name="e">Event to load data from</param>
		public void PostJsonLoading(FrcEvent e)
		{
			RedAlliance.PostJsonLoading(e);
			BlueAlliance.PostJsonLoading(e);

			if (!Pregame)
			{
				foreach (Goal g in Goals)
				{
					g.PostJsonLoading(e);
				}
			}
		}

		/// <summary>
		/// Gets the alliance based on color
		/// </summary>
		/// <param name="color">Color of the resulting alliance</param>
		/// <returns>Alliance based on color</returns>
		public Alliance GetAlliance(AllianceColor color)
		{
			switch (color)
			{
			case AllianceColor.Red:
				return RedAlliance;
			case AllianceColor.Blue:
				return BlueAlliance;
			default:
				return null;
			}
		}

		/// <summary>
		/// Get alliance color of a team.
		/// </summary>
		/// <param name="team">Team whose color to get</param>
		/// <returns>Color of team, Indeterminate if team isn't found</returns>
		public AllianceColor GetTeamColor(Team team)
		{
			if (team == null)
			{
				Util.DebugLog(LogLevel.Critical, "Team is null.");
				return AllianceColor.Indeterminate;
			}

			if (BlueAlliance.Contains(team))
			{
				return AllianceColor.Blue;
			}
			else if (RedAlliance.Contains(team))
			{
				return AllianceColor.Red;
			}
			else
			{
				Util.DebugLog(LogLevel.Error, "Neither alliance contains team " + team.Number.ToString());
				return AllianceColor.Indeterminate;
			}
		}

		/// <summary>
		/// Gets all goals scored by a team, including those scored by the
		/// whole alliance and global goals.
		/// </summary>
		/// <param name="team">Team whose goals to retrieve</param>
		/// <returns>List of goals scored by the team</returns>
		public List<Goal> GetGoalsByTeam(Team team)
		{
			AllianceColor color = GetTeamColor(team);

			return Goals.FindAll((g) =>
			{
				if (g.Global)
				{
					return true;
				}

				if (g.FullAlliance)
				{
					if (g.ScoringAlliance.HasValue)
					{
						return g.ScoringAlliance == color;
					}

					return false; // INVALID
				}

				return g.ScoringTeam == team;
			});
		}

		/// <summary>
		/// Gets whether a team was functioning properly in this match.
		/// </summary>
		/// <param name="team">Team in question</param>
		/// <returns>True if working, false if malfunctioning</returns>
		public bool GetWorking(Team team)
		{
			AllianceColor color = GetTeamColor(team);
			if (color == AllianceColor.Red)
			{
				AlliancePosition pos = RedAlliance.GetPositionOf(team);
				return RedWorking[pos];
			}
			else if (color == AllianceColor.Blue)
			{
				AlliancePosition pos = BlueAlliance.GetPositionOf(team);
				return BlueWorking[pos];
			}
			else
			{
				Util.DebugLog(LogLevel.Critical, "Invalid alliance color: " +
					color.ToString());
				throw new IndexOutOfRangeException("Invalid alliance color: " + 
					color.ToString());
			}
		}

		/// <summary>
		/// Gets the defense rating of a team.
		/// </summary>
		/// <param name="team">Team in question</param>
		/// <returns>Defense rating of team</returns>
		public int GetDefense(Team team)
		{
			AllianceColor color = GetTeamColor(team);
			if (color == AllianceColor.Red)
			{
				AlliancePosition pos = RedAlliance.GetPositionOf(team);
				return RedDefense[pos];
			}
			else if (color == AllianceColor.Blue)
			{
				AlliancePosition pos = BlueAlliance.GetPositionOf(team);
				return BlueDefense[pos];
			}
			else
			{
				Util.DebugLog(LogLevel.Error, "Invalid alliance color: " +
					color.ToString());
				return -1;
			}
		}

		public Team GetTeamByInclusiveIndex(int index)
		{
			switch (index)
			{
			case 0:
				return RedAlliance.A;
			case 1:
				return RedAlliance.B;
			case 2:
				return RedAlliance.C;
			case 3:
				return BlueAlliance.A;
			case 4:
				return BlueAlliance.B;
			case 5:
				return BlueAlliance.C;
			default:
				return null;
			}
		}

		public override string ToString()
		{
			return RedAlliance.TeamA_ID.ToString() + "-" +
				RedAlliance.TeamB_ID.ToString() + "-" +
				RedAlliance.TeamC_ID.ToString() + " vs " +
				BlueAlliance.TeamA_ID.ToString() + "-" +
				BlueAlliance.TeamB_ID.ToString() + "-" +
				BlueAlliance.TeamC_ID.ToString();
		}
	}
}
