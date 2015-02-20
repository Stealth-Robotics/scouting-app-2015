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
		/// File extension used when saving files
		/// </summary>
		public static string Extension
		{ get; set; }

		/// <summary>
		/// Root path to use when saving. Possibly obsolete.
		/// </summary>
		public static string RootPath
		{ get; set; }
		/// <summary>
		/// Data path to use when saving. Possibly obsolete.
		/// </summary>
		public static string DataPath
		{
			get
			{
				return RootPath + @"Data\";
			}
		}

		internal static DirectoryInfo RootDir
		{ get; private set; }
		internal static DirectoryInfo DataDir
		{ get; private set; }

		public static bool IsInitialized
		{ get; set; }

		public static List<string> EventFilenames
		{ get; private set; }

		public static Dictionary<string, string> LoadedFiles
		{ get; private set; }
		
		public static List<FrcEvent> LoadedEvents
		{ get; private set; }

		public static void Initialize()
		{
			if (IsInitialized)
			{
				return;
			}

			RootPath = Util.USERPROFILE + @"\ScoutingApp2015\";
			Extension = ".json";
			EventFilenames = new List<string>();
			LoadedFiles = new Dictionary<string, string>();
			LoadedEvents = new List<FrcEvent>();

			InitFiles();

			// END INIT
			IsInitialized = true;
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

			EventFilenames.Clear();
			IEnumerable<FileInfo> files = DataDir.EnumerateFiles();
			foreach (FileInfo fi in files)
			{
				EventFilenames.Add(fi.ToString());
			}
		}

		public static void LoadFile(string filename)
		{
			if (!EventFilenames.Contains(filename))
			{
				EventFilenames.Add(filename);
			}

			string contents = File.ReadAllText(filename);
			LoadedFiles.AddSet(filename, contents);
		}

		public static void ParseEvent(string filename)
		{
			if (!LoadedFiles.ContainsKey(filename))
			{
				return;
			}

			string contents = LoadedFiles[filename];
			FrcEvent frc = null;
			try
			{
				frc = JsonConvert.DeserializeObject<FrcEvent>(contents);
			}
			catch (JsonException)
			{
				Util.DebugLog(LogLevel.Critical, "Could not deserialize file " + filename);
			}
			frc.PostJsonLoading();
		}

		public static void SaveEvent(FrcEvent frc)
		{
			string contents = JsonConvert.SerializeObject(frc, Formatting.Indented);

			string filename = DataPath + frc.EventName + Extension;
			File.WriteAllText(filename, contents);
		}
	}
}
