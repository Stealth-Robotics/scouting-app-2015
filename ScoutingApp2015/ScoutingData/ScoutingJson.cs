using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ScoutingData.Data;
using System.IO;

namespace ScoutingData
{
	public static class ScoutingJson
	{
		public static string RootPath
		{ get; set; }
		public static string DataPath
		{
			get
			{
				return RootPath + @"Data\";
			}
		}

		internal static DirectoryInfo RootDir
		{ get; set; }
		internal static DirectoryInfo DataDir
		{ get; set; }

		public static bool IsInitialized
		{ get; set; }
		
		public static List<FrcEvent> Events
		{ get; set; }

		public static void Initialize()
		{
			if (IsInitialized)
			{
				return;
			}

			RootPath = Util.USERPROFILE + @"\ScoutingApp2015\";
		}

		public static void InitFiles()
		{
			if (!Directory.Exists(RootPath))
			{
				RootDir = Directory.CreateDirectory(RootPath);
			}
			if (!Directory.Exists(DataPath))
			{
				DataDir = Directory.CreateDirectory(DataPath);
			}


		}
	}
}
