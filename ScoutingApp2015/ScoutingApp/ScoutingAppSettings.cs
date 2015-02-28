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
	public class ScoutingAppSettings
	{
		static readonly string SETTINGS_PATH = Util.USERPROFILE + 
			"\\AppData\\Local\\ScoutingApp2015\\";
		static readonly string SETTINGS_FILENAME = "Settings.json";

		public static ScoutingAppSettings Instance
		{ get; private set; }

		public static bool PauseOnTeleop
		{
			get
			{
				Initialize();
				return Instance.Inst_pauseOnTeleop;
			}
			set
			{
				Initialize();
				Instance.Inst_pauseOnTeleop = value;
			}
		}
		[JsonProperty]
		public bool Inst_pauseOnTeleop
		{ get; private set; }

		public static bool hasInitialized = false;

		public static void Initialize()
		{
			if (hasInitialized)
			{
				return;
			}

			if (!Directory.Exists(SETTINGS_PATH))
			{
				Directory.CreateDirectory(SETTINGS_PATH);
			}

			if (File.Exists(SETTINGS_PATH + SETTINGS_FILENAME))
			{
				string json = File.ReadAllText(SETTINGS_PATH + SETTINGS_FILENAME);
				Instance = JsonConvert.DeserializeObject<ScoutingAppSettings>(json);
			}
			else
			{
				Instance = new ScoutingAppSettings();
				Save();
			}

			hasInitialized = true;
		}

		// Defaults
		public ScoutingAppSettings()
		{
			Inst_pauseOnTeleop = true;
		}

		public static void Save()
		{
			string json = JsonConvert.SerializeObject(Instance, Formatting.Indented);
			File.WriteAllText(SETTINGS_PATH + SETTINGS_FILENAME, json);
		}
	}
}
