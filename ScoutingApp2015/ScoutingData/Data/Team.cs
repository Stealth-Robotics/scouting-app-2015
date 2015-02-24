using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

using Newtonsoft.Json;
using System.Windows.Documents;

namespace ScoutingData.Data
{
	/// <summary>
	/// An FRC Team, with all pertaining data
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Team : IDescribable
	{
		/// <summary>
		/// Team number [PRIMARY KEY]
		/// </summary>
		[JsonProperty]
		public int Number
		{ get; set; }

		/// <summary>
		/// Team name
		/// </summary>
		[JsonProperty]
		public string Name
		{ get; set; }

		/// <summary>
		/// School of origin
		/// </summary>
		[JsonProperty]
		public string School
		{ get; set; }

		/// <summary>
		/// Description of physical characteristics, for easy identification
		/// </summary>
		[JsonProperty]
		public string Description
		{ get; set; }

		/// <summary>
		/// What to expect in a match from this bot
		/// </summary>
		[JsonProperty]
		public string Expectations
		{ get; set; }

		/// <summary>
		/// Overriding equality to only compare team numbers.
		/// </summary>
		/// <param name="obj">Object to compare to</param>
		/// <returns>
		/// True if teams have the same number, false if not or second 
		/// object is not a Team.
		/// </returns>
		public override bool Equals(object obj)
		{
			Team other = obj as Team;
			if (other == null)
			{
				return false;
			}

			return other.Number == this.Number;
		}
		/// <summary>
		/// Gets the hash code based on the team number,
		/// </summary>
		/// <returns>Team number.</returns>
		public override int GetHashCode()
		{
			return Number.GetHashCode();
		}

		/// <summary>
		/// Converts the team to a string in the format "Team 1234: TeamName".
		/// </summary>
		/// <returns>Number and name of team</returns>
		public override string ToString()
		{
			return "Team " + Number.ToString() + ": " + Name;
		}

		/// <summary>
		/// Overridden equality operator comparing teams
		/// </summary>
		/// <param name="a">Left-hand team</param>
		/// <param name="b">Right-hand team</param>
		/// <returns>True if the objects are equal, false if not.</returns>
		public static bool operator==(Team a, Team b)
		{
			object objA = a as object;
			object objB = b as object;

			if (objA == null && objB == null)
			{
				return true;
			}

			return a.Equals(b);
		}

		/// <summary>
		/// Overridden inequality operator comparing teams
		/// </summary>
		/// <param name="a">Left-hand team</param>
		/// <param name="b">Right-hand team</param>
		/// <returns>False if the objects are equal, true if not.</returns>
		public static bool operator!=(Team a, Team b)
		{
			return !(a == b);
		}

		public string GetDescription()
		{
			string res = ToString() + "\n";
			res += "From: " + School + "\n\n";
			res += "Description:\n";
			res += Description + "\n\n";
			res += "Expectations:\n";
			res += Expectations;

			return res;
		}

		public StackPanel GetDescriptionWPF()
		{
			StackPanel res = new StackPanel();
			TextBlock txt = new TextBlock();
			txt.Text = ToString();
			txt.FontWeight = FontWeights.Bold;
			res.Children.Add(txt);

			txt = new TextBlock();
			txt.Inlines.Add(new Bold(new Run("From: ")));
			txt.Inlines.Add(new Run(School));
			txt.Margin = new Thickness(0, 5, 0, 5);
			res.Children.Add(txt);

			txt = new TextBlock();
			txt.Text = "Description:";
			txt.FontWeight = FontWeights.Bold;
			res.Children.Add(txt);

			txt = new TextBlock();
			txt.Text = Description;
			txt.Margin = new Thickness(15, 0, 0, 5);
			res.Children.Add(txt);

			txt = new TextBlock();
			txt.Text = "Expectations:";
			txt.FontWeight = FontWeights.Bold;
			res.Children.Add(txt);

			txt = new TextBlock();
			txt.Text = Expectations;
			txt.Margin = new Thickness(15, 0, 0, 0);
			res.Children.Add(txt);

			return res;
		}
	}
}
