using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingApp
{
	public static class ScoutingAppSettings
	{
		public static bool PauseOnTeleop
		{ get; set; }

		public static void Initialize()
		{
			PauseOnTeleop = true;
		}
	}
}
