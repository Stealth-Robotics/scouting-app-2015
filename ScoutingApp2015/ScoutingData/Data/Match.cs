using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Match
	{
		/// <summary>
		/// Match Number within the competition event
		/// </summary>
		[JsonProperty]
		public int Number
		{ get; set; }

		/// <summary>
		/// Red Alliance
		/// </summary>
		[JsonProperty]
		public Alliance RedAlliance
		{ get; set; }

		/// <summary>
		/// Blue Alliance
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
		/// List of penalties on both sides
		/// </summary>
		[JsonProperty]
		public List<PenaltyBase> Penalties
		{ get; set; }

		/// <summary>
		/// All penalties AGAINST Blue
		/// </summary>
		public List<PenaltyBase> BluePenalties
		{
			get
			{
				return Penalties.FindAll((p) => p.AffectedAlliance() == AllianceColor.Blue);
			}
		}

		/// <summary>
		/// All penalties AGAINST Red
		/// </summary>
		public List<PenaltyBase> RedPenalties
		{
			get
			{
				return Penalties.FindAll((p) => p.AffectedAlliance() == AllianceColor.Red);
			}
		}

		/// <summary>
		/// Highly unreliable. Calculated from goals and discrepancy points.
		/// </summary>
		public int RedFinalScore
		{ get; set; }

		/// <summary>
		/// Highly unreliable. Calculated from goals and discrepancy points.
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
		/// Defense ratings for red
		/// </summary>
		[JsonProperty]
		public AllianceGroup<int> RedDefense
		{ get; set; }

		/// <summary>
		/// Defense ratings for blue
		/// </summary>
		[JsonProperty]
		public AllianceGroup<int> BlueDefense
		{ get; set; }

		internal void PostJsonLoading(FrcEvent e)
		{
			RedAlliance.PostJsonLoading(e);
			BlueAlliance.PostJsonLoading(e);

			foreach (Goal g in Goals)
			{
				g.PostJsonLoading(e);
			}
		}

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

		public AllianceColor GetTeamColor(Team team)
		{
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
				return (AllianceColor)(-1); // Make some errors!
			}
		}

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
	}
}
