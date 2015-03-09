using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData.Data;

namespace ScoutingData.Lite
{
	[JsonObject(MemberSerialization.OptIn)]
	public class RecordLite : IPostJson
	{
		public Team Team
		{ get; private set; }

		[JsonProperty]
		public int TeamID
		{ get; private set; }

		public Match Match
		{ get; private set; }

		[JsonProperty]
		public int MatchID
		{ get; private set; }

		[JsonProperty]
		public AllianceColor Color
		{ get; set; }

		[JsonProperty]
		public AlliancePosition Position
		{ get; set; }

		[JsonProperty]
		public RatingSet Ratings
		{ get; set; }

		public void PostJsonLoading(FrcEvent e)
		{
			Team = e.LoadTeam(TeamID);
			Match = e.LoadMatch(MatchID);
		}
	}
}
