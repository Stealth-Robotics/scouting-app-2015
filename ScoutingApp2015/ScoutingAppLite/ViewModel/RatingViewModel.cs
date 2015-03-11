using ScoutingData.Data;
using ScoutingData.Lite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingAppLite.ViewModel
{
	public class RatingViewModel : INotifyPropertyChanged
	{
		public RatingSet Ratings
		{
			get
			{
				return _ratings;
			}
			set
			{
				_ratings = value;
				OnPropertyChanged("Ratings");

				OnPropertyChanged("Ratings_Autonomous");
				OnPropertyChanged("Ratings_Stacking");
				OnPropertyChanged("Ratings_Coopertition");
				OnPropertyChanged("Ratings_Containers");
				OnPropertyChanged("Ratings_Mobility");
				OnPropertyChanged("Ratings_Efficiency");
				OnPropertyChanged("Ratings_Stability");
				OnPropertyChanged("Ratings_Grip");
				OnPropertyChanged("Ratings_HumanPlayerSkill");
			}
		}
		RatingSet _ratings;

		#region ratings
		public double Ratings_Autonomous
		{ 
			get
			{
				return Ratings.Autonomous;
			}
			set
			{
				Ratings.Autonomous = value;
				OnPropertyChanged("Ratings_Autonomous");
			}
		}
		public double Ratings_Stacking
		{
			get
			{
				return Ratings.Stacking;
			}
			set
			{
				Ratings.Stacking = value;
				OnPropertyChanged("Ratings_Stacking");
			}
		}
		public double Ratings_Coopertition
		{
			get
			{
				return Ratings.Coopertition;
			}
			set
			{
				Ratings.Coopertition = value;
				OnPropertyChanged("Ratings_Coopertition");
			}
		}
		public double Ratings_Containers
		{
			get
			{
				return Ratings.Containers;
			}
			set
			{
				Ratings.Containers = value;
				OnPropertyChanged("Ratings_Containers");
			}
		}
		public double Ratings_Mobility
		{
			get
			{
				return Ratings.Mobility;
			}
			set
			{
				Ratings.Mobility = value;
				OnPropertyChanged("Ratings_Mobility");
			}
		}
		public double Ratings_Efficiency
		{
			get
			{
				return Ratings.Efficiency;
			}
			set
			{
				Ratings.Efficiency = value;
				OnPropertyChanged("Ratings_Efficiency");
			}
		}
		public double Ratings_Stability
		{
			get
			{
				return Ratings.Stability;
			}
			set
			{
				Ratings.Stability = value;
				OnPropertyChanged("Ratings_Stability");
			}
		}
		public double Ratings_Grip
		{
			get
			{
				return Ratings.Grip;
			}
			set
			{
				Ratings.Grip = value;
				OnPropertyChanged("Ratings_Grip");
			}
		}
		public double Ratings_HumanPlayerSkill
		{
			get
			{
				return Ratings.HumanPlayerSkill;
			}
			set
			{
				Ratings.HumanPlayerSkill = value;
				OnPropertyChanged("Ratings_HumanPlayerSkill");
			}
		}
		#endregion

		public Team RatedTeam
		{
			get
			{
				return _ratedTeam;
			}
			set
			{
				_ratedTeam = value;
				OnPropertyChanged("RatedTeam");
				OnPropertyChanged("RatedTeamName");
			}
		}
		Team _ratedTeam;

		public string RatedTeamName
		{
			get
			{
				return RatedTeam.ToString();
			}
		}

		public bool IsTracked
		{
			get
			{
				return _isTracked;
			}
			set
			{
				_isTracked = value;
				OnPropertyChanged("IsTracked");
			}
		}
		bool _isTracked;

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string name)
		{
			if (PropertyChanged == null)
			{
				return;
			}

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
