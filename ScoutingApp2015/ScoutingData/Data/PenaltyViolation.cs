using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class PenaltyViolation : PenaltyBase
	{
		/// <summary>
		/// Amount of points deducted from the alliance's score (recorded as positive)
		/// </summary>
		[JsonProperty]
		public int PenaltyValue
		{ get; set; }

		[JsonProperty]
		public string Reasoning
		{ get; set; }

		[JsonProperty]
		public AllianceColor PenalizedAlliance
		{ get; set; }

		[JsonProperty]
		public int ViolatingTeamID
		{ get; set; }

		[JsonIgnore]
		public Team ViolatingTeam
		{ get; private set; }

		public PenaltyViolation(int points, string cause, AllianceColor color, int teamID)
		{
			PenaltyValue = points;
			Reasoning = cause;
			PenalizedAlliance = color;
			ViolatingTeamID = teamID;
		}

		public override int ScoreChange()
		{
			return -PenaltyValue;
		}

		public override AllianceColor AffectedAlliance()
		{
			return PenalizedAlliance;
		}

		public override string Reason()
		{
			return Reasoning;
		}

		public override void PostJsonLoading(FrcEvent e)
		{
			ViolatingTeam = e.LoadTeam(ViolatingTeamID);
		}
	}
}
