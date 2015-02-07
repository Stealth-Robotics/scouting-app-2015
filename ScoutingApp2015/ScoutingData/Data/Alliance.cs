using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Alliance : AllianceGroup<Team>
	{
		[JsonProperty]
		public int TeamA_ID
		{ get; set; }

		[JsonProperty]
		public int TeamB_ID
		{ get; set; }

		[JsonProperty]
		public int TeamC_ID
		{ get; set; }

		[JsonIgnore]
		public override Team A
		{
			get
			{
				return _teamA;
			}
			set
			{
				_teamA = value;
				TeamA_ID = value.Number;
			}
		}
		Team _teamA;

		[JsonIgnore]
		public override Team B
		{
			get
			{
				return _teamB;
			}
			set
			{
				_teamB = value;
				TeamB_ID = value.Number;
			}
		}
		Team _teamB;

		[JsonIgnore]
		public override Team C
		{
			get
			{
				return _teamC;
			}
			set
			{
				_teamC = value;
				TeamC_ID = value.Number;
			}
		}
		Team _teamC;

		public Alliance(Team a, Team b, Team c) : base(a, b, c)
		{ }

		public void PostJsonLoading(FrcEvent e)
		{
			A = e.LoadTeam(TeamA_ID);
			B = e.LoadTeam(TeamB_ID);
			C = e.LoadTeam(TeamC_ID);
		}
	}
}
