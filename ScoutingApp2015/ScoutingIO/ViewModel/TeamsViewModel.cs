using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingIO.ViewModel
{
	public class TeamsViewModel : INotifyPropertyChanged
	{
		public TeamsList Teams
		{
			get
			{
				return _teams;
			}
			set
			{
				_teams = value;
				OnPropertyChanged("Teams");
			}
		}
		TeamsList _teams;

		public Team SelectedTeam
		{
			get
			{
				return _selectedTeam;
			}
			set
			{
				_selectedTeam = value;
				OnPropertyChanged("SelectedTeam");
			}
		}
		Team _selectedTeam;

		public string TeamNumber_String
		{
			get
			{
				if (SelectedTeam != null)
				{
					return SelectedTeam.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked && SelectedTeam != null)
				{
					SelectedTeam.Number = i;
					OnPropertyChanged("TeamNumber_String");
				}
			}
		}

		public string TeamName
		{
			get
			{
				if (SelectedTeam != null)
				{
					return SelectedTeam.Name;
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (SelectedTeam != null)
				{
					SelectedTeam.Name = value;
					OnPropertyChanged("TeamName");
				}
			}
		}

		public string TeamLocation
		{
			get
			{
				if (SelectedTeam != null)
				{
					return SelectedTeam.School;
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (SelectedTeam != null)
				{
					SelectedTeam.School = value;
					OnPropertyChanged("TeamLocation");
				}
			}
		}

		public string TeamDescription
		{
			get
			{
				if (SelectedTeam != null)
				{
					return SelectedTeam.Description;
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (SelectedTeam != null)
				{
					SelectedTeam.Description = value;
					OnPropertyChanged("TeamDescription");
				}
			}
		}

		public string TeamExpectations
		{
			get
			{
				if (SelectedTeam != null)
				{
					return SelectedTeam.Expectations;
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (SelectedTeam != null)
				{
					SelectedTeam.Expectations = value;
					OnPropertyChanged("TeamExpectations");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public TeamsViewModel()
		{
			PropertyChanged += SelectedTeam_Changed;
			PropertyChanged += TeamsList_Changed;
		}

		public void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		public void SelectedTeam_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "SelectedTeam")
			{
				return; // none of my business
			}

			// Let the UI know everything's changing
			OnPropertyChanged("TeamNumber_String");
			OnPropertyChanged("TeamName");
			OnPropertyChanged("TeamLocation");
			OnPropertyChanged("TeamDescription");
			OnPropertyChanged("TeamExpectations");
		}

		public void TeamsList_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Teams")
			{
				return; // none of my business
			}

			OnPropertyChanged("SelectedTeam");
		}
	}
}
