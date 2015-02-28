using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData.Data;

namespace ScoutingData.Sync
{
	/// <summary>
	/// Recording for a match. This data is serialized and sent to a USB drive.
	/// </summary>
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
		{ get; set; }

		/// <summary>
		/// Objective rating of robot's defensive capabilities, out of 10
		/// </summary>
		[JsonProperty]
		public int Defense
		{ get; set; }

		/// <summary>
		/// Winner of the match
		/// </summary>
		[JsonProperty]
		public AllianceColor Winner
		{ get; set; }

		/// <summary>
		/// Final score of team's alliance
		/// </summary>
		[JsonProperty]
		public int AllianceFinalScore
		{ get; set; }

		/// <summary>
		/// New team description provided by scout
		/// </summary>
		[JsonProperty]
		public string TeamDescription
		{ get; set; }

		/// <summary>
		/// New team expectations provided by scout
		/// </summary>
		[JsonProperty]
		public string TeamExpectations
		{ get; set; }

		/// <summary>
		/// Instantiates a pregame recording of a match
		/// </summary>
		/// <param name="number">Match number</param>
		/// <param name="team">Team number</param>
		/// <param name="alliance">Alliance number</param>
		public RecordedMatch(int number, Team team, AllianceColor alliance)
		{
			MatchNumber = number;
			TrackedTeam = team;
			TrackedTeamID = team.Number;
			TeamDescription = team.Description;
			TeamExpectations = team.Expectations;
			Alliance = alliance;
			ScoredGoals = new List<Goal>();
			AlliancePenalties = new List<Penalty>();
			Working = true; // default
			Defense = 10; // default
			Winner = AllianceColor.Indeterminate; // indeterminate
			AllianceFinalScore = 0; // starting value
		}

		/// <summary>
		/// Adds a goal to the match recording
		/// </summary>
		/// <param name="g">Goal to record</param>
		public void AddGoal(Goal g)
		{
			ScoredGoals.Add(g);
		}

		/// <summary>
		/// Adds a penalty to the match recording
		/// </summary>
		/// <param name="p">Penalty to record</param>
		public void AddPenalty(Penalty p)
		{
			AlliancePenalties.Add(p);
		}

		/// <summary>
		/// Additional loading after deserialization
		/// </summary>
		/// <param name="e">Event to load data from</param>
		public void PostJsonLoading(FrcEvent e)
		{
			TrackedTeam = e.LoadTeam(TrackedTeamID);
		}

		/// <summary>
		/// Serializes the recording to a JSON string
		/// </summary>
		/// <returns>String containing the recording in the form of JSON</returns>
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
