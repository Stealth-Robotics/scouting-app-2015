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
	/// <summary>
	/// Overarching static class with a whole bunch of random stuff
	/// </summary>
	public static class ScoutingJson
	{
		/// <summary>
		/// File extension used when saving event files
		/// </summary>
		public static string EventExtension
		{ get; set; }

		/// <summary>
		/// File extension used when saving teams list files
		/// </summary>
		public static string TeamsListExtension
		{ get; set; }

		/// <summary>
		/// Root path to use when saving. Possibly obsolete.
		/// </summary>
		public static string LocalPath
		{
			get
			{
				return Util.USERPROFILE + "\\" + FolderName + "\\";
			}
		}
		public static string FolderName
		{ get; set; }
		public static string UsbPath
		{
			get
			{
				return DriveLetter + ":\\" + FolderName + "\\";
			}
		}
		public static char DriveLetter
		{ get; set; }

		public static bool IsInitialized
		{ get; set; }

		public static void Initialize(bool reInit)
		{
			if (IsInitialized && !reInit)
			{
				return;
			}

			FolderName = "ScoutingApp2015";
			DriveLetter = 'G';
			EventExtension = ".frc";
			TeamsListExtension = ".teams";

			InitFiles(false);

			// END INIT
			IsInitialized = true;
		}

		public static void InitFiles(bool usbToo)
		{
			if (!Directory.Exists(LocalPath))
			{
				Directory.CreateDirectory(LocalPath);
			}

			if (usbToo && Directory.Exists(DriveLetter + ":\\"))
			{
				if (!Directory.Exists(UsbPath))
				{
					Directory.CreateDirectory(UsbPath);
				}
			}
		}
		public static FrcEvent ParseFrcEvent(string fullPathName)
		{
			Initialize(false);

			string contents = File.ReadAllText(fullPathName);
			FrcEvent frc = null;
			try
			{
				frc = JsonConvert.DeserializeObject<FrcEvent>(contents);
			}
			catch (JsonException)
			{
				Util.DebugLog(LogLevel.Critical, "Could not deserialize file.");
			}

			return frc;
		}
		public static FrcEvent ParseFrcEvent(string filename, bool usb)
		{
			return ParseFrcEvent(usb ? UsbPath : LocalPath);
		}
		public static TeamsList ParseTeamsList(string fullPathName)
		{
			Initialize(false);

			string contents = File.ReadAllText(fullPathName);
			TeamsList list = null;
			try
			{
				list = JsonConvert.DeserializeObject<TeamsList>(contents);
			}
			catch (JsonException)
			{
				Util.DebugLog(LogLevel.Critical, "Could not deserialize file.");
			}

			return list;
		}
		public static TeamsList ParseTeamsList(string filename, bool usb)
		{
			return ParseTeamsList(usb ? UsbPath : LocalPath);
		}

		public static void SaveEvent(FrcEvent frc, bool usb)
		{
			Initialize(false);

			string contents = JsonConvert.SerializeObject(frc, Formatting.Indented);

			string filename = (usb ? UsbPath : LocalPath) + frc.EventName + EventExtension;
			File.WriteAllText(filename, contents);
		}
		public static void SaveTeamsList(TeamsList list, bool usb)
		{
			Initialize(false);

			string contents = JsonConvert.SerializeObject(list, Formatting.Indented);

			string filename = (usb ? UsbPath : LocalPath) + "Teams" + TeamsListExtension;
			File.WriteAllText(filename, contents);
		}
	}
}
