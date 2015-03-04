using ScoutingData.Analysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;

namespace ScoutingData
{
	public enum LogLevel
	{
		Info,
		Warning,
		Error,
		Critical
	}

	public delegate void Printigate(object sender, PrintEventArgs e);

	public class PrintEventArgs : EventArgs
	{
		public string Text
		{ get; set; }

		public LogLevel Level
		{ get; set; }

		public int RankLevel
		{
			get
			{
				return (int)Level;
			}
		}
	}

	/// <summary>
	/// Utility class containing various functions with no other purpose.
	/// </summary>
	public static class Util
	{
		public static readonly string USERPROFILE =
			Environment.GetEnvironmentVariable("USERPROFILE");

		internal static readonly IFormatProvider DEF_FORMAT = 
			CultureInfo.CurrentCulture.NumberFormat;

		/// <summary>
		/// Length of time for an FRC 2015 match, in the form of a TimeSpan object
		/// </summary>
		public static readonly TimeSpan MATCH_LENGTH = new TimeSpan(0, 2, 30);
		/// <summary>
		/// Time when teleop starts
		/// </summary>
		public static readonly TimeSpan TELEOP = TimeSpan.FromSeconds(15);
		/// <summary>
		/// Time when players can no longer throw litter noodles into the arena
		/// </summary>
		public static readonly TimeSpan TRASH_TOSS_STOP = new TimeSpan(0, 2, 10);

		/// <summary>
		/// Subscribe to add an additional output location for Util.DebugLog().
		/// </summary>
		public static event Printigate OnPrint;

		/// <summary>
		/// Clamps a value between a min and max (inclusively), and returns if clamping was necessary.
		/// </summary>
		/// <typeparam name="T">Type being compared when clamping. Must extend IComparable</typeparam>
		/// <param name="val">Value to clamp</param>
		/// <param name="min">Minimum</param>
		/// <param name="max">Maximum</param>
		/// <returns></returns>
		public static bool Clamp<T>(T val, T min, T max) where T : IComparable<T>
		{
			bool needed = false;

			if (val.CompareTo(min) < 0)
			{
				needed = true;
				val = min;
			}
			if (val.CompareTo(max) > 0)
			{
				needed = true;
				val = max;
			}

			return needed;
		}

		/// <summary>
		/// Alternative to ToString() for enums, just apply a DescriptionAttribute to
		/// the enum value.
		/// </summary>
		/// <typeparam name="T">Enum type</typeparam>
		/// <param name="enumerationValue">Value converted to string</param>
		/// <returns>
		/// A string provided by the DescriptionAttribute, or 
		/// enumerationValue.ToString() if there is no DescriptionAttribute applied.
		/// </returns>
		public static string GetDescription<T>(this T enumerationValue)
			where T : struct
		{
			Type type = enumerationValue.GetType();
			if (!type.IsEnum)
			{
				throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
			}

			//Tries to find a DescriptionAttribute for a potential friendly name
			//for the enum
			MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
			if (memberInfo != null && memberInfo.Length > 0)
			{
				object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attrs != null && attrs.Length > 0)
				{
					//Pull out the description value
					return ((DescriptionAttribute)attrs[0]).Description;
				}
			}
			//If we have no description attribute, just return the ToString of the enum
			return enumerationValue.ToString();

		}

		public static Color MakeColor(int a, int r, int g, int b)
		{
			return new Color() { A = (byte)a, R = (byte)r, G = (byte)g, B = (byte)b };
		}
		public static Color MakeColor(string src)
		{
			string hexString = src.TrimStart('#');

			if (hexString.Length != 6 && hexString.Length != 8)
			{
				return Colors.Transparent;
			}

			hexString = hexString.ToUpper();

			if (hexString.Length == 6)
			{
				hexString = "FF" + hexString;
			}

			string aStr = hexString.Substring(0, 2);
			string rStr = hexString.Substring(2, 2);
			string gStr = hexString.Substring(4, 2);
			string bStr = hexString.Substring(6, 2);

			byte a = byte.Parse(aStr, NumberStyles.HexNumber);
			byte r = byte.Parse(rStr, NumberStyles.HexNumber);
			byte g = byte.Parse(gStr, NumberStyles.HexNumber);
			byte b = byte.Parse(bStr, NumberStyles.HexNumber);

			return MakeColor(a, r, g, b);
		}

		/// <summary>
		/// Shortcut for Debug Logging
		/// </summary>
		/// <param name="level">Seriousness of message</param>
		/// <param name="message">Message logged</param>
		public static void DebugLog(LogLevel level, string message)
		{
			string output = ">> [" + level.ToString().ToUpper() + "] " + message + "\n";
			System.Diagnostics.Debugger.Log((int)level, "SCOUTING", output);

			if (OnPrint != null)
			{
				OnPrint(null, new PrintEventArgs() { Level = level, Text = output });
			}
		}
		/// <summary>
		/// Shortcut for Debug Logging, with category
		/// </summary>
		/// <param name="level">Seriousness of message</param>
		/// <param name="category">Section of app with log</param>
		/// <param name="message">Message logged</param>
		public static void DebugLog(LogLevel level, string category, string message)
		{
			DebugLog(level, "[" + category + "] " + message);
		}

		/// <summary>
		/// Gets whether a path is on a removable device or not
		/// </summary>
		/// <param name="path">Path to check</param>
		/// <returns>True if the path is a removable device, false otherwise.</returns>
		public static bool IsPathOnRemovableDevice(string path)
		{
			DriveInfo[] infos = DriveInfo.GetDrives();

			foreach (DriveInfo inf in infos)
			{
				if (inf.DriveType == DriveType.Removable)
				{
					if (path.StartsWith(inf.Name))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the paths of all the removable drives on the machine.
		/// </summary>
		/// <returns>IEnumerable of paths of the devices</returns>
		public static IEnumerable<string> GetAllUsbDrivePaths()
		{
			DriveInfo[] infos = DriveInfo.GetDrives();

			foreach (DriveInfo di in infos)
			{
				if (di.DriveType == DriveType.Removable)
				{
					yield return di.Name;
				}
			}
		}

		/// <summary>
		/// Gets path of the first (letter) removable drive on the machine.
		/// </summary>
		/// <returns>Path of removable device, or null if none were found.</returns>
		public static string GetFirstUsbDrivePath()
		{
			DriveInfo[] infos = DriveInfo.GetDrives();

			foreach (DriveInfo di in infos)
			{
				if (di.DriveType == DriveType.Removable)
				{
					return di.Name;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the path of the folder containing the file
		/// </summary>
		/// <param name="filePath">Path to the file in question</param>
		/// <returns>Path of the folder containing the file</returns>
		public static string GetFolderPath(string filePath)
		{
			int lastBS = filePath.LastIndexOf('\\');
			if (lastBS == -1)
			{
				return "0";
			}

			return filePath.Substring(0, lastBS + 1);
		}

		/// <summary>
		/// Gets the name of the file, excluding the containing folder path
		/// </summary>
		/// <param name="filePath">Path to the file in question</param>
		/// <param name="extension">Whether to include the extension in the result</param>
		/// <returns>Name of the file, without the folder path</returns>
		public static string GetFileName(string filePath, bool extension)
		{
			string name = filePath.Substring(GetFolderPath(filePath).Length);

			if (!extension)
			{
				int firstDot = name.IndexOf('.');
				if (firstDot != -1)
				{
					name = name.Substring(0, firstDot);
				}
			}

			return name;
		}

		///////////////////////
		// Extension Methods //
		///////////////////////

		/// <summary>
		/// Extension method, converting timespans into seconds left, for convenience
		/// </summary>
		/// <param name="ts">Object to convert</param>
		/// <returns>Total seconds left in the timespan</returns>
		public static int CountedSeconds(this TimeSpan ts)
		{
			return (3600 * ts.Hours) + (60 * ts.Minutes) + ts.Seconds;
		}

		/// <summary>
		/// Adds a value to the dictionary, overwriting if the key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type in the Dictionary</typeparam>
		/// <typeparam name="TVal">Value type in the Dictionary</typeparam>
		/// <param name="dict">IDictionary to add stuff to</param>
		/// <param name="key">Key to set or add</param>
		/// <param name="val">Value the key is set to</param>
		public static void AddSet<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key, TVal val)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = val;
			}
			else
			{
				dict.Add(key, val);
			}
		}

		/// <summary>
		/// Adds a value to the dictionary, incrementing by an amount if the key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type of dictionary</typeparam>
		/// <typeparam name="TVal">Value type of dictionary. Must be numeric.</typeparam>
		/// <param name="dict">Dictionary to do this with</param>
		/// <param name="key">Key for whose value to increment</param>
		/// <param name="incVal">Amount to increment by</param>
		/// <param name="startVal">Value to start at when adding new pairs</param>
		public static void AddIncrement<TKey, TVal>(this IDictionary<TKey, TVal> dict, 
			TKey key, double incVal, double startVal) where TVal : IConvertible, new()
		{
			if (dict.ContainsKey(key))
			{
				// this is how you increment generic types
				IConvertible conv = dict[key].ToDouble(DEF_FORMAT) + 1.0;
				dict[key] = (TVal)conv.ToType(typeof(TVal), DEF_FORMAT);
			}
			else
			{
				// this is how you instantiate generic types (that implement IConvertible)
				IConvertible start = (IConvertible)startVal;
				dict.Add(key, (TVal)start.ToType(typeof(TVal), DEF_FORMAT));
			}
		}

		/// <summary>
		/// Adds a value to the dictionary with a key of zero, incrementing by one if the 
		/// key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type of dictionary</typeparam>
		/// <typeparam name="TVal">Value type of dictionary. Must be numeric.</typeparam>
		/// <param name="dict">Dictionary to do this with</param>
		/// <param name="key">Key for whose value to increment</param>
		public static void AddIncrement<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key)
			where TVal : IConvertible, new()
		{
			dict.AddIncrement(key, 1, 0);
		}
		
		/// <summary>
		/// Quick function for converting numbers from 0 to 1 into percentages
		/// </summary>
		/// <param name="n">number to convert</param>
		/// <returns>percentage string</returns>
		public static string ToStringPct(this double n)
		{
			return (n * 100).ToString() + "%";
		}

		public static T Find<T>(this IEnumerable<T> list, Predicate<T> predicate)
		{
			foreach (T t in list)
			{
				if (predicate(t))
				{
					return t;
				}
			}

			return default(T);
		}

		public static bool Exists<T>(this IEnumerable<T> list, Predicate<T> predicate)
		{
			foreach (T t in list)
			{
				if (predicate(t))
				{
					return true;
				}
			}

			return false;
		}

		public static bool Remove<T>(this ObservableCollection<T> obs, Predicate<T> predicate)
		{
			T rem = default(T);
			bool found = false;
			foreach (T t in obs)
			{
				if (predicate(t))
				{
					rem = t;
					found = true;
					break;
				}
			}

			if (!found)
			{
				return false;
			}

			return obs.Remove(rem);
		}
	}
}
