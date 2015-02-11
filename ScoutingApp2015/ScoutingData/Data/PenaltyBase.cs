using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject]
	public abstract class PenaltyBase : IPostJson
	{
		[JsonProperty]
		public int TimeOfPenaltyInt
		{ get; set; }

		[JsonIgnore]
		public TimeSpan TimeOfPenalty
		{
			get
			{
				return TimeSpan.FromSeconds((double)TimeOfPenaltyInt);
			}
			set
			{
				TimeOfPenaltyInt = value.CountedSeconds();
			}
		}

		public PenaltyBase(int time)
		{
			TimeOfPenaltyInt = time;
		}

		public abstract int ScoreChange();
		public abstract AllianceColor AffectedAlliance();
		public abstract string Reason();
		public abstract Team TeamAtFault();

		public abstract void PostJsonLoading(FrcEvent e);
	}
}
