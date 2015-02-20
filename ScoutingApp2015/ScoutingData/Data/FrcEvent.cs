using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	/// <summary>
	/// Main object from which all non-analysis data is stored
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class FrcEvent
	{
		/// <summary>
		/// List of all matches, complete and incomplete, within the event
		/// </summary>
		[JsonProperty]
		public List<Match> Matches
		{ get; set; }

		/// <summary>
		/// List of all competing teams within the event
		/// </summary>
		[JsonProperty]
		public List<Team> CompetingTeams
		{ get; set; }

		/// <summary>
		/// Name of the event. Must be identical to event analysis name.
		/// </summary>
		[JsonProperty]
		public string EventName
		{ get; set; }

		/// <summary>
		/// Loads a team from an ID
		/// </summary>
		/// <param name="teamID">ID of team to load (4089 for example)</param>
		/// <returns>Reference to team object with that ID</returns>
		public Team LoadTeam(int teamID)
		{
			return CompetingTeams.Find((t) => t.Number == teamID);
		}

		/// <summary>
		/// Performs additional loading of matches once loaded from JSON
		/// </summary>
		public void PostJsonLoading()
		{
			foreach (Match m in Matches)
			{
				m.PostJsonLoading(this);
			}
		}

		/// <summary>
		/// Loads a match from a match ID
		/// </summary>
		/// <param name="matchID">ID of match to load</param>
		/// <returns>Reference to match object with that ID</returns>
		public Match LoadMatch(int matchID)
		{
			return Matches.Find((m) => m.Number == matchID);
		}
	}
}
