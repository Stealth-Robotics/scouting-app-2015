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

		public RecordLite() : this(null, null)
		{ }
		public RecordLite(Team team, Match match)
		{
			Team = team;
			TeamID = team != null ? team.Number : -1;
			Match = match;
			MatchID = match != null ? match.Number : -1;

			if (match != null)
			{
				Color = match.GetTeamColor(team);
				Position = match.GetTeamPosition(team);
				Ratings = new RatingSet();
			}
		}

		public void PostJsonLoading(FrcEvent e)
		{
			Team = e.LoadTeam(TeamID);
			Match = e.LoadMatch(MatchID);
		}
	}
}
