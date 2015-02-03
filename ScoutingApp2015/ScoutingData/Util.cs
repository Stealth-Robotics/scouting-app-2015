using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData
{
	public enum LogLevel
	{
		Info,
		Warning,
		Error,
		Critical
	}

	public static class Util
	{
		/// <summary>
		/// Length of time for an FRC 2015 match, in the form of a TimeSpan object
		/// </summary>
		public static readonly TimeSpan MATCH_LENGTH = new TimeSpan(0, 1, 30);
		/// <summary>
		/// Time left when teleop starts
		/// </summary>
		public static readonly TimeSpan TELEOP = new TimeSpan(0, 1, 15);

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
		/// Shortcut for Debug Logging
		/// </summary>
		/// <param name="level">Seriousness of message</param>
		/// <param name="message">Message logged</param>
		public static void DebugLog(LogLevel level, string message)
		{
			System.Diagnostics.Debugger.Log((int)level, "SCOUTING", 
				"\n[" + level.ToString().ToUpper() + "] " + message);
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
		/// Extension method, converting timespans into seconds left, for convenience
		/// </summary>
		/// <param name="ts">Object to convert</param>
		/// <returns>Total seconds left in the timespan</returns>
		public static int CountedSeconds(this TimeSpan ts)
		{
			return (3600 * ts.Hours) + (60 * ts.Minutes) + ts.Seconds;
		}
	}
}
