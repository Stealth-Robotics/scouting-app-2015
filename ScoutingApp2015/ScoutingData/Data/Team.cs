using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Team
	{
		/// <summary>
		/// Team number [PRIMARY KEY]
		/// </summary>
		[JsonProperty]
		public int Number
		{ get; set; }

		/// <summary>
		/// Team name
		/// </summary>
		[JsonProperty]
		public string Name
		{ get; set; }

		/// <summary>
		/// School of origin
		/// </summary>
		[JsonProperty]
		public string School
		{ get; set; }

		/// <summary>
		/// Description of physical characteristics, for easy identification
		/// </summary>
		[JsonProperty]
		public string Description
		{ get; set; }

		/// <summary>
		/// What to expect in a match from this bot
		/// </summary>
		[JsonProperty]
		public string Expectations
		{ get; set; }

		public override bool Equals(object obj)
		{
			Team other = obj as Team;
			if (other == null)
			{
				return false;
			}

			return other.Number == this.Number;
		}
		public override int GetHashCode()
		{
			return Number.GetHashCode();
		}

		public override string ToString()
		{
			return "Team " + Number.ToString() + ": " + Name;
		}

		public static bool operator==(Team a, Team b)
		{
			object objA = a as object;
			object objB = b as object;

			if (objA == null && objB == null)
			{
				return true;
			}

			return a.Equals(b);
		}

		public static bool operator!=(Team a, Team b)
		{
			return !(a == b);
		}
	}
}
