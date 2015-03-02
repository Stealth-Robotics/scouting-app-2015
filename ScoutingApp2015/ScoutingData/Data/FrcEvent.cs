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
		public TeamsList AllTeams
		{ get; set; }

		/// <summary>
		/// Name of the event. Must be identical to event analysis name.
		/// </summary>
		[JsonProperty]
		public string EventName
		{ get; set; }

		/// <summary>
		/// True once the teams list has loaded
		/// </summary>
		public bool HasLoadedTeams
		{ get; private set; }

		public FrcEvent(string name)
		{
			Matches = new List<Match>();
			EventName = name;
		}

		/// <summary>
		/// Loads a team from an ID
		/// </summary>
		/// <param name="teamID">ID of team to load (4089 for example)</param>
		/// <returns>Reference to team object with that ID</returns>
		public Team LoadTeam(int teamID)
		{
			return AllTeams.Find((t) => t.Number == teamID);
		}

		/// <summary>
		/// Performs additional loading of matches once loaded from JSON.
		/// Not to be confused with IPostJson.PostJsonLoading(FrcEvent).
		/// </summary>
		public void PostJsonLoading(TeamsList teams)
		{
			AllTeams = teams;

			foreach (Match m in Matches)
			{
				m.PostJsonLoading(this);
			}

			HasLoadedTeams = true;
		}

		public bool IsCorrectlyLoaded()
		{
			return Matches != null && EventName != null;
		}

		/// <summary>
		/// Loads a match from a match ID
		/// </summary>
		/// <param name="matchID">ID of match to load</param>
		/// <returns>Reference to match object with that ID</returns>
		public Match LoadMatch(int matchID)
		{
			Match res = null;
			if (Matches.Exists((m) => m.Number == matchID))
			{
				res = Matches.Find((m) => m.Number == matchID);
			}

			return res;
		}
	}
}
