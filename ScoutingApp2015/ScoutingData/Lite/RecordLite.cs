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
		public Team LinkedTeam
		{ get; private set; }

		[JsonProperty]
		public int TeamID
		{ get; private set; }

		public Match LinkedMatch
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
			throw new NotImplementedException();
		}
	}
}
