using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Lite
{
	[JsonObject(MemberSerialization.OptIn)]
	public class RatingSet
	{
		/// <summary>
		/// Rating of robot's capabilities in autonomous
		/// </summary>
		[JsonProperty]
		public double Autonomous
		{ get; set; }

		/// <summary>
		/// Rating of robot's stacking abilities
		/// </summary>
		[JsonProperty]
		public double Stacking
		{ get; set; }

		/// <summary>
		/// Rating of robot's coopertition capabilities
		/// </summary>
		[JsonProperty]
		// Still not a word
		public double Coopertition
		{ get; set; }

		/// <summary>
		/// Rating of robot's ability to lift containers high or fill them
		/// </summary>
		[JsonProperty]
		public double Containers
		{ get; set; }

		/// <summary>
		/// Rating of robot's ability to move about the field
		/// </summary>
		[JsonProperty]
		public double Mobility
		{ get; set; }

		/// <summary>
		/// Rating of robot's ability to perform tasks quickly
		/// </summary>
		[JsonProperty]
		public double Efficiency
		{ get; set; }

		/// <summary>
		/// Rating of robot's ability to stay upright
		/// </summary>
		[JsonProperty]
		public double Stability
		{ get; set; }

		/// <summary>
		/// Rating of robot's ability to keep a hold of totes/containers
		/// </summary>
		[JsonProperty]
		public double Grip
		{ get; set; }

		/// <summary>
		/// Rating of the human players' skill in operating the robot
		/// </summary>
		[JsonProperty]
		public double HumanPlayerSkill
		{ get; set; }

		/// <summary>
		/// Overall rating from other qualities
		/// </summary>
		public double OverallRating
		{
			get
			{
				double sigma = Autonomous + Stacking +
					Coopertition + Containers + Mobility +
					Efficiency + Stability + Grip +
					HumanPlayerSkill;

				return sigma / 9.0;
			}
		}

		public RatingSet()
		{
			Autonomous = 0;
			Stacking = 0;
			Coopertition = 0;
			Containers = 0;
			Mobility = 0;
			Efficiency = 0;
			Stability = 0;
			Grip = 0;
			HumanPlayerSkill = 0;
		}
	}
}
