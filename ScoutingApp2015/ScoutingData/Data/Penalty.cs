using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	/// <summary>
	/// Penalty object containing all data required for penalties
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Penalty : IPostJson
	{
		/// <summary>
		/// Time the penalty was scored, in seconds
		/// </summary>
		[JsonProperty]
		public int TimeOfPenaltyInt
		{ get; set; }

		/// <summary>
		/// Time the penalty was scored. Derived from int form.
		/// </summary>
		[JsonIgnore]
		public TimeSpan TimeOfPenalty
		{
			get
			{
				return TimeSpan.FromSeconds((double)TimeOfPenaltyInt);
			}
			set
			{
				TimeOfPenaltyInt = value.CountedSeconds();
			}
		}

		/// <summary>
		/// Reason given for the penalty.
		/// </summary>
		[JsonProperty]
		public string Reasoning
		{ get; set; }

		/// <summary>
		/// Color of alliance penalized.
		/// </summary>
		[JsonProperty]
		public AllianceColor PenalizedAlliance
		{ get; set; }

		/// <summary>
		/// ID of team at fault.
		/// </summary>
		[JsonProperty]
		public int BlamedTeamID
		{ get; set; }

		/// <summary>
		/// Team who committed the foul.
		/// </summary>
		[JsonIgnore]
		public Team BlamedTeam
		{ get; private set; }

		/// <summary>
		/// Used for data binding. Sorta like ToString() but more concise.
		/// </summary>
		[JsonIgnore]
		public string UIForm
		{
			get
			{
				string res = string.Format(@"[{0:m\:ss}]", TimeOfPenalty);
				res += string.Format(@"[{0:D2}]", ScoreChange()) + ": ";
				res += Reasoning;
				return res;
			}
		}

		/// <summary>
		/// Instantiates a new penalty object
		/// </summary>
		/// <param name="time">Time the penalty was scored</param>
		/// <param name="reason">Reason given for the penalty</param>
		/// <param name="penalizedAlliance">Alliance penalized</param>
		/// <param name="teamID">ID of team at fault</param>
		public Penalty(int time, string reason, AllianceColor penalizedAlliance, 
			Team team)
		{
			TimeOfPenaltyInt = time;
			Reasoning = reason;
			PenalizedAlliance = penalizedAlliance;
			BlamedTeam = team;
			if (BlamedTeam != null)
			{
				BlamedTeamID = team.Number;
			}
		}

		/// <summary>
		/// Simple function provided for posterity and compatibility. Should
		/// always be negative, unless points are given.
		/// </summary>
		/// <returns>For now, negative six.</returns>
		public int ScoreChange()
		{
			return -6;
		}

		/// <summary>
		/// Additional loading to perform after deserialization from JSON
		/// </summary>
		/// <param name="e">FRC event to load from</param>
		public void PostJsonLoading(FrcEvent e)
		{
			BlamedTeam = e.LoadTeam(BlamedTeamID);
		}
	}
}
