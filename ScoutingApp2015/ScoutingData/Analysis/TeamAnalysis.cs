using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData.Data;

namespace ScoutingData.Analysis
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TeamAnalysis
	{
		public FrcEvent ParentEvent
		{ get; private set; }

		public Team LinkedTeam
		{ get; set; }

		#region analysis
		
		/// <summary>
		/// Ratio of wins out of total games. [= Wins / Matches.Count]
		/// </summary>
		[JsonProperty]
		public float WinRate
		{ get; private set; }

		/// <summary>
		/// Distribution of Scored Points over matches so far. [= Matches.Points.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution ScoredPoints
		{ get; private set; }

		/// <summary>
		/// Distribution of the final scores over matches so far. [= Matches.FinalScore.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution FinalScore
		{ get; private set; }

		/// <summary>
		/// Distribution of the penalties given over matches so far. [= Matches.PenaltyPoints.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution Penalties
		{ get; private set; }

		/// <summary>
		/// Average violations per game. [= Violations / Matches.Count]
		/// </summary>
		[JsonProperty]
		public float ViolationsPerGame
		{ get; private set; }

		/// <summary>
		/// Responsiveness in the bot's latest game [= Matches.Last.Responsiveness]
		/// </summary>
		[JsonProperty]
		public bool WorkingCurrently
		{ get; private set; }

		/// <summary>
		/// Responsiveness rate over matches so far [= Matches.CountAll(Responsive) / Matches.Count]
		/// </summary>
		[JsonProperty]
		public float ResponsivenessRate
		{ get; private set; }

		/// <summary>
		/// Distribution of the defense ratings over matches so far [= Matches.Defense.Distribution]
		/// </summary>
		[JsonProperty]
		public Distribution Defense
		{ get; private set; }

		#endregion

		public TeamAnalysis(FrcEvent e) : this(e, null)
		{ }

		public TeamAnalysis(FrcEvent e, Team linked)
		{
			ParentEvent = e;
			LinkedTeam = linked;
		}

		public void CalculateSafe()
		{
			bool invalid = false;
			if (ParentEvent == null)
			{
				invalid = true;
				Util.DebugLog(LogLevel.Error, "Could not calculate team analysis, as ParentEvent is null.");
			}
			if (LinkedTeam == null)
			{
				invalid = true;
				Util.DebugLog(LogLevel.Error, "Could not calculate team analysis, as LinkedTeam is null.");
			}
			if (invalid)
			{
				return;
			}

			Calculate();
		}

		public void Calculate()
		{

		}
	}
}
