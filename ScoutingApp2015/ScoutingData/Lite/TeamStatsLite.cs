using Newtonsoft.Json;
using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Lite
{
	public class TeamStatsLite
	{
		public Team Team
		{ get; private set; }

		public List<RecordLite> AllRecords
		{ get; private set; }

		public TeamStatsLite()
		{
			AllRecords = new List<RecordLite>();
		}

		public RatingSet GetMeanRatings()
		{
			RatingSet set = new RatingSet();
			double n = (double)AllRecords.Count;

			if (AllRecords.Count == 0)
			{
				return new RatingSet();
			}

			foreach (RecordLite rec in AllRecords)
			{
				set.Autonomous += rec.Ratings.Autonomous / n;
				set.Stacking += rec.Ratings.Stacking / n;
				set.Coopertition += rec.Ratings.Coopertition / n;
				set.Containers += rec.Ratings.Containers / n;
				set.Mobility += rec.Ratings.Mobility / n;
				set.Efficiency += rec.Ratings.Efficiency / n;
				set.Stability += rec.Ratings.Stability / n;
				set.Grip += rec.Ratings.Grip / n;
				set.HumanPlayerSkill += rec.Ratings.HumanPlayerSkill / n;
			}

			return set;
		}
	}
}
