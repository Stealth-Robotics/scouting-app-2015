using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData.Data;

namespace ScoutingData.Sync
{
	[JsonObject(MemberSerialization.OptIn)]
	public class RecordedMatch : IPostJson
	{
		/// <summary>
		/// Match number as listed in the event data [pregame]
		/// </summary>
		[JsonProperty]
		public int MatchNumber
		{ get; private set; }

		/// <summary>
		/// Team tracked by scout [pregame]
		/// </summary>
		[JsonIgnore]
		public Team TrackedTeam
		{ get; private set; }

		/// <summary>
		/// ID of tracked team [pregame]
		/// </summary>
		[JsonProperty]
		public int TrackedTeamID
		{ get; private set; }

		/// <summary>
		/// Alliance tracked team is on [pregame]
		/// </summary>
		[JsonProperty]
		public AllianceColor Alliance
		{ get; private set; }

		/// <summary>
		/// List of goals scored by this team
		/// </summary>
		[JsonProperty]
		public List<Goal> ScoredGoals
		{ get; private set; }

		/// <summary>
		/// List of penalties scored by this team
		/// </summary>
		[JsonProperty]
		public List<Penalty> AlliancePenalties
		{ get; private set; }

		/// <summary>
		/// Whether the robot is functioning correctly
		/// </summary>
		[JsonProperty]
		public bool Working
		{ get; private set; }

		/// <summary>
		/// Objective rating of robot's defensive capabilities, out of 10
		/// </summary>
		[JsonProperty]
		public int Defense
		{ get; private set; }

		/// <summary>
		/// Winner of the match
		/// </summary>
		[JsonProperty]
		public AllianceColor Winner
		{ get; private set; }

		/// <summary>
		/// Final score of team's alliance
		/// </summary>
		[JsonProperty]
		public int AllianceFinalScore
		{ get; private set; }

		public RecordedMatch(int number, Team team, AllianceColor alliance)
		{
			MatchNumber = number;
			TrackedTeam = team;
			TrackedTeamID = team.Number;
			Alliance = alliance;
			ScoredGoals = new List<Goal>();
			AlliancePenalties = new List<Penalty>();
			Working = true; // default
			Defense = 10; // default
			Winner = AllianceColor.NULL; // indeterminate
			AllianceFinalScore = 0; // starting value
		}

		public void AddGoal(Goal g)
		{
			ScoredGoals.Add(g);
		}

		public void AddPenalty(Penalty p)
		{
			AlliancePenalties.Add(p);
		}

		public void PostJsonLoading(FrcEvent e)
		{
			TrackedTeam = e.LoadTeam(TrackedTeamID);
		}
	}
}
