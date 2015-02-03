using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class PenaltyFoul : PenaltyBase
	{
		[JsonProperty]
		public string Reasoning
		{ get; set; }

		[JsonProperty]
		public AllianceColor PenalizedAlliance
		{ get; set; }

		public override int ScoreChange()
		{
			return -6;
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
			return;
		}
	}
}
