using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using ScoutingData.Data;
using System.IO;
using ScoutingData.Sync;
using ScoutingData.Analysis;
using ScoutingData.Lite;

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
		/// File extension used when saving match records
		/// </summary>
		public static string MatchRecordExtension
		{ get; set; }

		/// <summary>
		/// File extension used when saving analysis caches
		/// </summary>
		public static string AnalysisExtension
		{ get; set; }

		/// <summary>
		/// File extension used when saving lite records
		/// </summary>
		public static string LiteRecordExtension
		{ get; set; }

		/// <summary>
		/// Default local path when saving and loading
		/// </summary>
		public static string LocalPath
		{
			get
			{
				return Util.USERPROFILE + "\\" + FolderName + "\\";
			}
		}

		/// <summary>
		/// Name of folder appended to path when saving and loading
		/// </summary>
		public static string FolderName
		{ get; set; }

		/// <summary>
		/// Default removable device path when saving and loading (possibly obsolete)
		/// </summary>
		public static string UsbPath
		{
			get
			{
				return DriveLetter + ":\\" + FolderName + "\\";
			}
		}

		/// <summary>
		/// Drive letter of loaded USB drive
		/// </summary>
		public static char DriveLetter
		{ get; set; }

		/// <summary>
		/// If the I/O has initialized or not
		/// </summary>
		public static bool IsInitialized
		{ get; set; }

		/// <summary>
		/// Initializes the static properties. Call at the beginning point of your app.
		/// </summary>
		/// <param name="reInit">Set to true to re-initialize to default values</param>
		public static void Initialize(bool reInit)
		{
			if (IsInitialized && !reInit)
			{
				return;
			}

			FolderName = "ScoutingApp2015";
			string rem = Util.GetFirstUsbDrivePath();
			DriveLetter = (rem != null ? rem[0] : 'G');
			EventExtension = ".frc";
			TeamsListExtension = ".teams";
			MatchRecordExtension = ".match";
			AnalysisExtension = ".stats";
			LiteRecordExtension = ".lrec";

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
				return null;
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
		public static RecordedMatch ParseMatchRecord(string fullPathName)
		{
			Initialize(false);

			string contents = File.ReadAllText(fullPathName);
			RecordedMatch rec = null;
			try
			{
				rec = JsonConvert.DeserializeObject<RecordedMatch>(contents);
			}
			catch (JsonException)
			{
				Util.DebugLog(LogLevel.Critical, "Could not deserialize file.");
			}

			return rec;
		}
		public static EventAnalysis ParseAnalysis(string fullPathName)
		{
			Initialize(false);

			string contents = File.ReadAllText(fullPathName);
			EventAnalysis stats = null;
			try
			{
				stats = JsonConvert.DeserializeObject<EventAnalysis>(contents);
			}
			catch (JsonException)
			{
				Util.DebugLog(LogLevel.Critical, "Could not deserialize file.");
			}

			return stats;
		}

		public static RecordLite ParseLiteRecord(string path)
		{
			Initialize(false);

			string contents = File.ReadAllText(path);
			RecordLite lite = null;
			try
			{
				lite = JsonConvert.DeserializeObject<RecordLite>(contents);
			}
			catch (JsonException)
			{
				Util.DebugLog(LogLevel.Critical, "Could not deserialize file.");
			}

			return lite;
		}

		public static void SaveEvent(FrcEvent frc, string path)
		{
			Initialize(false);

			string contents = JsonConvert.SerializeObject(frc, Formatting.Indented);
			File.WriteAllText(path, contents);
		}
		public static void SaveTeamsList(TeamsList list, string path)
		{
			Initialize(false);

			string contents = JsonConvert.SerializeObject(list, Formatting.Indented);

			File.WriteAllText(path, contents);
		}
		public static void SaveMatchRecord(RecordedMatch rec, string path)
		{
			Initialize(false);

			string contents = JsonConvert.SerializeObject(rec, Formatting.Indented);
			File.WriteAllText(path, contents);
		}
		public static void SaveAnalysis(EventAnalysis stats, string path)
		{
			Initialize(false);

			string contents = JsonConvert.SerializeObject(stats, Formatting.Indented);
			File.WriteAllText(path, contents);
		}
		public static void SaveLiteRecord(RecordLite rec, string path)
		{
			Initialize(false);

			string contents = JsonConvert.SerializeObject(rec, Formatting.Indented);
			File.WriteAllText(path, contents);
		}

		public static bool HasRecordInFolder(string folderPath)
		{
			IEnumerable<string> allFiles = new List<string>();

			try
			{
				allFiles = Directory.EnumerateFiles(folderPath);
			}
			catch (IOException e)
			{
				Util.DebugLog(LogLevel.Warning, "Error enumerating files: " + e.Message);
			}

			foreach (string filename in allFiles)
			{
				if (filename.EndsWith(MatchRecordExtension))
				{
					return true;
				}
			}

			return false;
		}

		public static RecordedMatch GetFirstRecordInFolder(string folderPath)
		{
			if (!HasRecordInFolder(folderPath))
			{
				return null;
			}

			string path = (from file in Directory.EnumerateFiles(folderPath)
						   where file.EndsWith(MatchRecordExtension)
						   select file).FirstOrDefault();
			return ParseMatchRecord(path);
		}

		public static bool HasEventInFolder(string folderPath)
		{
			foreach (string filename in Directory.EnumerateFiles(folderPath))
			{
				if (filename.EndsWith(EventExtension))
				{
					return true;
				}
			}

			return false;
		}
		public static FrcEvent GetFirstEventInFolder(string folderPath)
		{
			if (!HasEventInFolder(folderPath))
			{
				return null;
			}

			string path = (from file in Directory.EnumerateFiles(folderPath)
						   where file.EndsWith(EventExtension)
						   select file).FirstOrDefault();
			return ParseFrcEvent(path);
		}
	}
}
