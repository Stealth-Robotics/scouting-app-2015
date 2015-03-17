using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ScoutingData;
using System.IO;

namespace ScoutingManagerLite
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AppSettings
	{
		static readonly string SETTINGS_PATH = Util.APPDATA + "\\ScoutingApp2015";
		static readonly string SETTINGS_FILE = SETTINGS_PATH + "\\managerlite.json";

		public static AppSettings Instance
		{ get; private set; }

		public static string EventPath
		{
			get
			{
				return Instance.eventPath;
			}
			set
			{
				Instance.eventPath = value;
				Save();
			}
		}
		[JsonProperty]
		string eventPath;

		public static string TeamsPath
		{
			get
			{
				return Instance.teamsPath;
			}
			set
			{
				Instance.teamsPath = value;
				Save();
			}
		}
		[JsonProperty]
		string teamsPath;

		public static string RecordsPath
		{
			get
			{
				return Instance.recordsPath;
			}
			set
			{
				Instance.recordsPath = value;
				Save();
			}
		}
		[JsonProperty]
		string recordsPath;

		public AppSettings()
		{
			eventPath = "";
			teamsPath = "";
			recordsPath = "";
		}

		public static void Initialize()
		{
			if (!Directory.Exists(SETTINGS_PATH) || !File.Exists(SETTINGS_FILE))
			{
				Instance = new AppSettings();

				if (!Directory.Exists(SETTINGS_PATH))
				{
					Directory.CreateDirectory(SETTINGS_PATH);
				}

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
