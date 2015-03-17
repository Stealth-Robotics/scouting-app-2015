using ScoutingData.Data;
using ScoutingData.Lite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				if (Ratings == null)
				{
					return 0;
				}

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
				OnPropertyChanged("Color");
				OnPropertyChanged("RatedTeamInfo");
			}
		}
		Team _ratedTeam;

		public StackPanel RatedTeamInfo
		{
			get
			{
				if (RatedTeam == null)
				{
					return null;
				}

				return RatedTeam.GetDescriptionWPF();
			}
		}

		public Match IndicatedMatch
		{
			get
			{
				return _indicatedMatch;
			}
			set
			{
				_indicatedMatch = value;
				OnPropertyChanged("IsTracked");
				OnPropertyChanged("Color");
				OnPropertyChanged("Color_Brush");
			}
		}
		Match _indicatedMatch;

		public AllianceColor Color
		{ 
			get
			{
				if (IndicatedMatch == null)
				{
					return AllianceColor.Indeterminate;
				}

				return IndicatedMatch.GetTeamColor(RatedTeam);
			}
		}

		public SolidColorBrush Color_Brush
		{
			get
			{
				switch (Color)
				{
				case AllianceColor.Blue:
					return new SolidColorBrush(Colors.CornflowerBlue);
				case AllianceColor.Red:
					return new SolidColorBrush(Colors.Salmon);
				default:
					return new SolidColorBrush(Colors.Gray);
				}
			}
		}

		public string RatedTeamName
		{
			get
			{
				if (RatedTeam == null)
				{
					return "NULL";
				}

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

		public RatingViewModel()
		{
			Ratings = new RatingSet();
		}

		public void OnPropertyChanged(string name)
		{
			if (PropertyChanged == null)
			{
				return;
			}

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public RecordLite MakeRecord()
		{
			if (IndicatedMatch == null)
			{
				return null;
			}

			RecordLite rec = new RecordLite(RatedTeam, IndicatedMatch);
			rec.Ratings = Ratings;
			return rec;
		}
	}
}
