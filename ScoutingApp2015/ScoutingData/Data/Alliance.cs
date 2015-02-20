using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScoutingData.Data
{
	/// <summary>
	/// Alliance Group of teams, specially serialized due to the way teams are stored
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Alliance : AllianceGroup<Team>, IPostJson
	{
		/// <summary>
		/// Team ID for Team A
		/// </summary>
		[JsonProperty]
		public int TeamA_ID
		{ get; set; }

		/// <summary>
		/// Team ID for Team B
		/// </summary>
		[JsonProperty]
		public int TeamB_ID
		{ get; set; }

		/// <summary>
		/// Team ID for Team C
		/// </summary>
		[JsonProperty]
		public int TeamC_ID
		{ get; set; }

		/// <summary>
		/// Team A
		/// </summary>
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

		/// <summary>
		/// Team B
		/// </summary>
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

		/// <summary>
		/// Team C
		/// </summary>
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

		/// <summary>
		/// Instantiates an Alliance 
		/// </summary>
		/// <param name="a">Team A</param>
		/// <param name="b">Team B</param>
		/// <param name="c">Team C</param>
		public Alliance(Team a, Team b, Team c) : base(a, b, c)
		{ }

		/// <summary>
		/// Loading done after deserialized from JSON
		/// </summary>
		/// <param name="e">FRC event linked to</param>
		public void PostJsonLoading(FrcEvent e)
		{
			A = e.LoadTeam(TeamA_ID);
			B = e.LoadTeam(TeamB_ID);
			C = e.LoadTeam(TeamC_ID);
		}
	}
}
