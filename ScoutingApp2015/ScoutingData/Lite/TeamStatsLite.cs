using Newtonsoft.Json;
using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Lite
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TeamStatsLite : IPostJson
	{
		public Team Team
		{ get; private set; }

		[JsonProperty]
		public int TeamID
		{ get; set; }

		[JsonProperty]
		public List<RecordLite> AllRecords
		{ get; private set; }

		public TeamStatsLite()
		{
			AllRecords = new List<RecordLite>();
		}

		public void PostJsonLoading(FrcEvent e)
		{
			Team = e.LoadTeam(TeamID);
		}
	}
}
