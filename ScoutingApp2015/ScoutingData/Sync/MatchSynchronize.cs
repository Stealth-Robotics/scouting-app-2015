using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Sync
{
	public static class MatchSynchronize
	{
		public static Match FormMatch(FrcEvent e,
			AllianceGroup<RecordedMatch> redData, 
			AllianceGroup<RecordedMatch> blueData)
		{
			Alliance red = new Alliance(redData.A.TrackedTeam, 
				redData.B.TrackedTeam, redData.C.TrackedTeam);
			Alliance blue = new Alliance(blueData.A.TrackedTeam,
				blueData.B.TrackedTeam, blueData.C.TrackedTeam);
			
			List<RecordedMatch> allData = new List<RecordedMatch>();
			foreach (RecordedMatch rec in redData)
			{
				allData.Add(rec);
			}
			foreach (RecordedMatch rec in blueData)
			{
				allData.Add(rec);
			}

			return null; // TODO: matchmaking
		}
	}
}
