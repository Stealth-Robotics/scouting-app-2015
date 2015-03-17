using ScoutingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace ScoutingApp
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AppSettings
	{
		static readonly string SETTINGS_PATH = Util.APPDATA + @"\ScoutingApp2015\";
		static readonly string SETTINGS_FILENAME = "mainapp.json";

		public static AppSettings Instance
		{ get; private set; }

		public static bool PauseOnTeleop
		{
			get
			{
				return Instance.pauseOnTeleop;
			}
			set
			{
				Instance.pauseOnTeleop = value;
				Save();
			}
		}
		[JsonProperty]
		bool pauseOnTeleop;

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

		public static void Initialize()
		{
			if (!Directory.Exists(SETTINGS_PATH))
			{
				Directory.CreateDirectory(SETTINGS_PATH);
			}

			if (File.Exists(SETTINGS_PATH + SETTINGS_FILENAME))
			{
				string json = File.ReadAllText(SETTINGS_PATH + SETTINGS_FILENAME);
				Instance = JsonConvert.DeserializeObject<AppSettings>(json);
			}
			else
			{
				Instance = new AppSettings();
				Save();
			}
		}

		// Defaults
		public AppSettings()
		{
			pauseOnTeleop = true;
		}

		public static void Save()
		{
			string json = JsonConvert.SerializeObject(Instance, Formatting.Indented);
			File.WriteAllText(SETTINGS_PATH + SETTINGS_FILENAME, json);
		}
	}
}
