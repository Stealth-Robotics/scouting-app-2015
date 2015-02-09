using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject]
	public abstract class PenaltyBase
	{
		public abstract int ScoreChange();
		public abstract AllianceColor AffectedAlliance();
		public abstract string Reason();
		public abstract Team TeamAtFault();

		public abstract void PostJsonLoading(FrcEvent e);
	}
}
