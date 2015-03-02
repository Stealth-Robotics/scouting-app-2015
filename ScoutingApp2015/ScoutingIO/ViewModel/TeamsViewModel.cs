using ScoutingData;
using ScoutingData.Data;
using ScoutingIO.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ScoutingIO.ViewModel
{
	public class TeamsViewModel : INotifyPropertyChanged
	{
		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				SaveFile();
				_filePath = value;
				OnPropertyChanged("FilePath");
			}
		}
		string _filePath;

		public SolidColorBrush PathBrush
		{
			get
			{
				return _pathBrush;
			}
			set
			{
				_pathBrush = value;
				OnPropertyChanged("PathBrush");
			}
		}
		SolidColorBrush _pathBrush;

		public string PathTooltip
		{
			get
			{
				return _pathTooltip;
			}
			set
			{
				_pathTooltip = value;
				OnPropertyChanged("PathTooltip");
			}
		}
		string _pathTooltip;

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
				DoSendData();
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
					Team sel = SelectedTeam;
					OnPropertyChanged("Teams_Adjusted");
					SelectedTeam = sel;
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
					Team sel = SelectedTeam;
					OnPropertyChanged("Teams_Adjusted");
					SelectedTeam = sel;
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
					return "";
				}
			}
			set
			{
				if (SelectedTeam != null)
				{
					SelectedTeam.School = value;
					OnPropertyChanged("TeamLocation");
					Team sel = SelectedTeam;
					OnPropertyChanged("Teams_Adjusted");
					SelectedTeam = sel;
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
					return "";
				}
			}
			set
			{
				if (SelectedTeam != null)
				{
					SelectedTeam.Description = value;
					OnPropertyChanged("TeamDescription");
					Team sel = SelectedTeam;
					OnPropertyChanged("Teams_Adjusted");
					SelectedTeam = sel;
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
					return "";
				}
			}
			set
			{
				if (SelectedTeam != null)
				{
					SelectedTeam.Expectations = value;
					OnPropertyChanged("TeamExpectations");
					Team sel = SelectedTeam;
					OnPropertyChanged("Teams_Adjusted");
					SelectedTeam = sel;
				}
			}
		}

		public ICollectionView TeamsUI
		{
			get
			{
				return _teamsUI;
			}
			private set
			{
				_teamsUI = value;
				OnPropertyChanged("TeamsUI");
			}
		}
		ICollectionView _teamsUI;

		public ICommand BreakCmd
		{ get; private set; }

		public ICommand SelectionChangedCmd
		{ get; private set; }

		public ICommand CellEditedCmd
		{ get; private set;}

		public ICommand OpenTeamsFileCmd
		{ get; private set; }

		public ICommand NewTeamCmd
		{ get; private set; }

		public ICommand DeleteTeamCmd
		{ get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<TeamsViewModel> SendData;

		public TeamsViewModel()
		{
			ScoutingJson.Initialize(false);

			BreakCmd = new DoStuffCommand(Break, obj => true);
			OpenTeamsFileCmd = new DoStuffCommand(OpenFile, obj => true);
			CellEditedCmd = new DoStuffCommand(OnDataGridCellChanged, obj => true);
			NewTeamCmd = new DoStuffCommand(OnNewTeamBtn_Clicked, obj => true);
			DeleteTeamCmd = new DoStuffCommand(OnDeleteTeamBtn_Clicked, obj => true);

			PropertyChanged += SelectedTeam_Changed;
			PropertyChanged += TeamsList_Changed;
			PropertyChanged += TeamsPath_Changed;
			PropertyChanged += TeamsList_Adjusted;

			// Load Default
			FilePath = ScoutingJson.LocalPath + "Teams" + 
				ScoutingJson.TeamsListExtension;
		}

		public void Break()
		{
			System.Diagnostics.Debugger.Break();
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
			SaveFile();
		}

		public void TeamsList_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Teams")
			{
				return; // none of my business
			}

			OnPropertyChanged("SelectedTeam");
			if (Teams != null)
			{
				Teams.PropertyChanged += TeamsList_Adjusted;
			}

			OnPropertyChanged("Teams_Adjusted");
		}

		public void TeamsList_Adjusted(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Teams_Adjusted")
			{
				return;
			}

			if (Teams != null)
			{
				TeamsUI = CollectionViewSource.GetDefaultView(Teams.ToList());
				SaveFile();
			}
		}

		public void TeamsPath_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "FilePath")
			{
				return; // none of my business
			}

			if (File.Exists(FilePath))
			{
				if (Util.IsPathOnRemovableDevice(FilePath))
				{
					PathBrush = new SolidColorBrush(Colors.Orange);
					PathTooltip = "File is on removable device. Local " +
						"storage or a cloud folder is ideal.";
				}
				else
				{
					PathBrush = new SolidColorBrush(Colors.Black);
					PathTooltip = FilePath;
				}

				Teams = ScoutingJson.ParseTeamsList(FilePath);
				SelectedTeam = Teams.FirstOrDefault();
			}
			else
			{
				PathBrush = new SolidColorBrush(Colors.Red);
				PathTooltip = "File does not exist.";
			}
		}

		public void SaveFile()
		{
			if (Teams == null)
			{
				return;
			}

			ScoutingJson.SaveTeamsList(Teams, FilePath);
			DoSendData();
		}

		public void DoSendData()
		{
			if (SendData != null)
			{
				SendData(this, new EventArgs<TeamsViewModel>(this));
			}
		}

		public void OpenFile()
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.CheckPathExists = true;
			ofd.DefaultExt = ScoutingJson.TeamsListExtension;
			ofd.Title = "Open Teams File";
			ofd.InitialDirectory = ScoutingJson.LocalPath;
			ofd.Filter = "FRC Teams Files (*.teams)|*.teams";
			bool? res = ofd.ShowDialog();

			if (res == true) // Nullable<bool>
			{
				if (File.Exists(ofd.FileName))
				{
					FilePath = ofd.FileName;
				}
			}
		}

		public void OnDataGridCellChanged()
		{
			OnPropertyChanged("SelectedTeam");
		}

		public void OnNewTeamBtn_Clicked()
		{
			NewTeamDialog ntd = new NewTeamDialog(Teams);
			bool? res = ntd.ShowDialog();

			if (res == true) // Nullable<bool>
			{
				Team t = ntd.MakeTeam();
				Teams.Add(t);
				SelectedTeam = t;
				OnPropertyChanged("Teams_Adjusted");
			}
		}

		public void OnDeleteTeamBtn_Clicked()
		{
			MessageBoxResult mbr = MessageBox.Show(
				"Are you sure you want to delete Team " +
				TeamNumber_String + "?", "Confirm Delete",
				MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (mbr == MessageBoxResult.Yes)
			{
				Team del = SelectedTeam;
				Teams.Remove(del);
				SelectedTeam = Teams.FirstOrDefault();
				OnPropertyChanged("Teams_Adjusted");
				SaveFile();
			}
		}
	}
}
