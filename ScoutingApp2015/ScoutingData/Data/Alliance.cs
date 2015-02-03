using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScoutingData.Data
{
	public enum AllianceColor
	{
		Red,
		Blue
	}

	public enum AlliancePosition
	{
		A,
		B,
		C
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class Alliance : IEnumerable<Team>
	{
		[JsonProperty]
		public AllianceColor Color
		{ get; set; }

		[JsonProperty]
		public int TeamA_ID
		{ get; set; }

		[JsonProperty]
		public int TeamB_ID
		{ get; set; }

		[JsonProperty]
		public int TeamC_ID
		{ get; set; }

		public Team TeamA
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

		public Team TeamB
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

		public Team TeamC
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

		public Alliance(Team a, Team b, Team c)
		{
			TeamA = a;
			TeamB = b;
			TeamC = c;
		}

		public void PostJsonLoading(FrcEvent e)
		{
			TeamA = e.LoadTeam(TeamA_ID);
			TeamB = e.LoadTeam(TeamB_ID);
			TeamC = e.LoadTeam(TeamC_ID);
		}

		#region Interfaces

		public IEnumerator<Team> GetEnumerator()
		{
			return new AllianceEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal class AllianceEnumerator : IEnumerator<Team>
		{
			int position;
			Alliance alliance;

			public AllianceEnumerator(Alliance al)
			{
				alliance = al;
				position = 0;
			}

			public Team Current
			{
				get 
				{
					switch (position)
					{
					case 0:
						return alliance.TeamA;
					case 1:
						return alliance.TeamB;
					case 2:
						return alliance.TeamC;
					default:
						return alliance.TeamA;
					}
				}
			}

			public void Dispose()
			{ }

			object IEnumerator.Current
			{
				get 
				{
					return this.Current; 
				}
			}

			public bool MoveNext()
			{
				position++;

				if (position > 2)
				{
					position = 2;
					return false;
				}

				return true;
			}

			public void Reset()
			{
				position = 0;
			}
		}
		
		#endregion
	}
}
