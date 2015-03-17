using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData;
using System.IO;

namespace ScoutingAppLite
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AppSettings
	{
		static readonly string SETTINGS_PATH = Util.APPDATA + "\\ScoutingApp2015";
		static readonly string SETTINGS_FILE = SETTINGS_PATH + "\\applite.json";

		public static AppSettings Instance
		{ get; private set; }

		public static string EventFile
		{
			get
			{
				return Instance.eventFile;
			}
			set
			{
				Instance.eventFile = value;
				Save();
			}
		}
		[JsonProperty]
		string eventFile;

		public static string TeamsFile
		{
			get
			{
				return Instance.teamsFile;
			}
			set
			{
				Instance.teamsFile = value;
				Save();
			}
		}
		[JsonProperty]
		string teamsFile;

		public static string SavePath
		{
			get
			{
				return Instance.savePath;
			}
			set
			{
				Instance.savePath = value;
				Save();
			}
		}
		[JsonProperty]
		string savePath;

		public AppSettings()
		{
			eventFile = "";
			teamsFile = "";
			savePath = @"H:\";
		}

		public static void Initialize()
		{
			if (!Directory.Exists(SETTINGS_PATH))
			{
				Directory.CreateDirectory(SETTINGS_PATH);
			}

			if (!File.Exists(SETTINGS_FILE))
			{
				Instance = new AppSettings();
				Save();
			}
			else
			{
				string contents = File.ReadAllText(SETTINGS_FILE);
				Instance = JsonConvert.DeserializeObject<AppSettings>(contents);
			}
		}

		public static void Save()
		{
			string contents = JsonConvert.SerializeObject(Instance, Formatting.Indented);
			File.WriteAllText(SETTINGS_FILE, contents);
		}
	}
}
