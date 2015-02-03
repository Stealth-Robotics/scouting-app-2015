using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ScoutingData.Data;

namespace ScoutingData
{
	public static class ScoutingJson
	{
		public static bool IsInitialized
		{ get; set; }
		
		/// <summary>
		/// FRC Events
		/// </summary>
		public static List<FrcEvent> Events
		{ get; set; }

		public static void Initialize()
		{
			if (IsInitialized)
			{
				return;
			}

			// init
		}

		public static void LoadEvents(Uri basePath)
		{

		}
	}
}
