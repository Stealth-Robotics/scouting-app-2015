using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Penalty : IPostJson
	{
		[JsonProperty]
		public int TimeOfPenaltyInt
		{ get; set; }

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

		[JsonProperty]
		public string Reasoning
		{ get; set; }

		[JsonProperty]
		public AllianceColor PenalizedAlliance
		{ get; set; }

		[JsonProperty]
		public int BlamedTeamID
		{ get; set; }

		[JsonIgnore]
		public Team BlamedTeam
		{ get; private set; }

		public Penalty(int time, string reason, AllianceColor penalizedAlliance, 
			int teamID)
		{
			TimeOfPenaltyInt = time;
			Reasoning = reason;
			PenalizedAlliance = penalizedAlliance;
			BlamedTeamID = teamID;
		}

		public int ScoreChange()
		{
			return -6;
		}

		public AllianceColor AffectedAlliance()
		{
			return PenalizedAlliance;
		}

		public string Reason()
		{
			return Reasoning;
		}

		public Team TeamAtFault()
		{
			return BlamedTeam;
		}

		public void PostJsonLoading(FrcEvent e)
		{
			BlamedTeam = e.LoadTeam(BlamedTeamID);
		}
	}
}
