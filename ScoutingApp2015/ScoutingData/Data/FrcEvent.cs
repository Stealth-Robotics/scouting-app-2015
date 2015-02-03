using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	public class FrcEvent
	{
		[JsonProperty]
		public List<Match> Matches
		{ get; set; }

		[JsonProperty]
		public List<Team> CompetingTeams
		{ get; set; }

		public Team LoadTeam(int teamID)
		{
			return CompetingTeams.Find((t) => t.Number == teamID);
		}

		public void PostJsonLoading()
		{
			foreach (Match m in Matches)
			{
				m.PostJsonLoading(this);
			}
		}
	}
}
