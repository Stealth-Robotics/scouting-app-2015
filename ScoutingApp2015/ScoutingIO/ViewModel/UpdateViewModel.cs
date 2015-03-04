using ScoutingData;
using ScoutingData.Data;
using ScoutingData.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ScoutingIO.ViewModel
{
	public class UpdateViewModel : INotifyPropertyChanged
	{
		public string SavePath
		{ get; private set; }

		public FrcEvent Event
		{ get; private set; }

		public string SelectedMatchID_String
		{
			get
			{
				return _selectedMatchID.ToString();
			}
			set
			{
				int i = _selectedMatchID;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					bool changed = (i != _selectedMatchID); 

					_selectedMatchID = i;
					OnPropertyChanged("SelectedMatchID_String");
					if (Event != null)
					{
						UpdateIndicators(Event.LoadMatch(i));
					}
				}
			}
		}
		int _selectedMatchID;

		public bool CanMerge
		{
			get
			{
				return Records.Count((rec) => rec != null) >= Merging.MATCH_NOT_ENOUGH_DATA_THRESHOLD;
			}
		}

		public bool IsLoading
		{
			get
			{
				return _isLoading;
			}
			private set
			{
				_isLoading = value;
				OnPropertyChanged("IsLoading");
			}
		}
		bool _isLoading;

		public DispatcherTimer SuperTimer
		{ get; private set; }

		public RecordedMatch[] Records
		{ get; private set; }

		public TeamIndicator RedA
		{ get; private set; }

		public TeamIndicator RedB
		{ get; private set; }

		public TeamIndicator RedC
		{ get; private set; }

		public TeamIndicator BlueA
		{ get; private set; }

		public TeamIndicator BlueB
		{ get; private set; }

		public TeamIndicator BlueC
		{ get; private set; }

		public bool CheckForNewFolders
		{
			get
			{
				return SuperTimer.IsEnabled;
			}
			set
			{
				SuperTimer.IsEnabled = value;
				OnPropertyChanged("CheckForNewFolders");
			}
		}

		public bool ProcessAllNewFolders
		{
			get
			{
				return _processAllNewFolders;
			}
			set
			{
				_processAllNewFolders = value;
				OnPropertyChanged("ProcessAllNewFolders");
			}
		}
		bool _processAllNewFolders;

		public ObservableCollection<UpdateModel> CheckedPaths
		{
			get
			{
				return _checkedPaths;
			}
			set
			{
				_checkedPaths = value;
				OnPropertyChanged("CheckedPaths");
			}
		}
		ObservableCollection<UpdateModel> _checkedPaths;

		public UpdateModel SelectedPathModel
		{
			get
			{
				return _selectedPathModel;
			}
			set
			{
				_selectedPathModel = value;
				OnPropertyChanged("SelectedPathModel");
			}
		}
		UpdateModel _selectedPathModel;

		public ICommand ProcessSelectedCmd
		{ get; private set; }

		public ICommand MergeMatchCmd
		{ get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public UpdateViewModel()
		{
			_checkedPaths = new ObservableCollection<UpdateModel>();

			ProcessSelectedCmd = new DoStuffCommand(() => 
				ProcessPath(SelectedPathModel), obj => true);
			MergeMatchCmd = new DoStuffCommand(MergeMatchRecords, obj => true);

			SuperTimer = new DispatcherTimer();
			SuperTimer.Interval = TimeSpan.FromSeconds(2);
			SuperTimer.Tick += SuperTimer_Tick;
			SuperTimer.IsEnabled = true;

			Records = new RecordedMatch[6];

			bool prettifySetup = false;

			RedA = new TeamIndicator(false, -1);
			RedB = new TeamIndicator(false, -1);
			RedC = new TeamIndicator(false, -1);
			BlueA = new TeamIndicator(true, -1);
			BlueB = new TeamIndicator(true, -1);
			BlueC = new TeamIndicator(true, -1);
			
			if (prettifySetup)
			{
				RedA.IsReady = true;
				RedC.IsReady = true;
				BlueB.IsReady = true;
				BlueA.IsReady = true;
			}

			_processAllNewFolders = false;
		}

		// Check for files if Process All is on
		public void SuperTimer_Tick(object sender, EventArgs e)
		{
			IsLoading = true;
			CheckForRecordData();

			if (!ProcessAllNewFolders)
			{
				IsLoading = false;
				return;
			}

			foreach (UpdateModel m in CheckedPaths)
			{
				if (m.IsSelected)
				{
					ProcessPath(m);
				}
			}
			IsLoading = false;
		}

		public void CheckForRecordData()
		{
			CheckedPaths.Clear();
			foreach (string drive in Util.GetAllUsbDrivePaths())
			{
				UpdateModel added = new UpdateModel() { Path = drive };
				added.IsSelected = ScoutingJson.HasRecordInFolder(drive);

				CheckedPaths.Add(added);
			}
			OnPropertyChanged("CheckedPaths");

			SelectedPathModel = CheckedPaths.FirstOrDefault((um) => um.IsSelected);
		}

		public void OnPropertyChanged(string prop)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public void ProcessPath(UpdateModel model)
		{
			if (model == null)
			{
				return;
			}

			LoadAndTransferRecords(model.Path);
			UpdateTeamsAndEvent(model.Path);
		}

		public void LoadAndTransferRecords(string path)
		{
			List<string> files = Directory.EnumerateFiles(path, 
				"*" + ScoutingJson.MatchRecordExtension,
				SearchOption.TopDirectoryOnly).ToList();

			foreach (string f in files)
			{
				if (!f.EndsWith(ScoutingJson.MatchRecordExtension))
				{
					continue;
				}

				string fn = Util.GetFileName(f, false);
				int position = GetPositionFromFilename(fn);

				RecordedMatch rec = ScoutingJson.ParseMatchRecord(f);
				rec.PostJsonLoading(Event);
				Records[position] = rec;
				OnPropertyChanged("CanMerge");

				MarkReady(position);

				bool result = MoveFileToEventPath(f);
			}
		}

		public void MarkReady(int position)
		{
			switch (position)
			{
			case 0:
				RedA.IsReady = true;
				break;
			case 1:
				RedB.IsReady = true;
				break;
			case 2:
				RedC.IsReady = true;
				break;
			case 3:
				BlueA.IsReady = true;
				break;
			case 4:
				BlueB.IsReady = true;
				break;
			case 5:
				BlueC.IsReady = true;
				break;
			}
		}

		public bool MoveFileToEventPath(string filePath)
		{
			string fileName = Util.GetFileName(filePath, true);

			try
			{
				File.Move(filePath, SavePath + fileName);
			}
			catch (IOException e)
			{
				string msg = "File " + fileName + " could not be moved.";
				Util.DebugLog(LogLevel.Error, msg);

				MessageBox.Show(msg + "\n\n" + e.Message, "Error",
					MessageBoxButton.OK, MessageBoxImage.Stop);
				return false;
			}

			return true;
		}

		public void UpdateTeamsAndEvent(string path)
		{
			if (Event.AllTeams != null)
			{
				ScoutingJson.SaveTeamsList(Event.AllTeams, path + 
					"Teams" + ScoutingJson.TeamsListExtension);
			}
			ScoutingJson.SaveEvent(Event, path +
				Event.EventName + ScoutingJson.EventExtension);
		}

		public void UpdateIndicators(Match match)
		{
			if (match == null)
			{
				RedA.SetNumber(-1);
				RedB.SetNumber(-1);
				RedC.SetNumber(-1);
				BlueA.SetNumber(-1);
				BlueB.SetNumber(-1);
				BlueC.SetNumber(-1);

				return;
			}

			RedA.SetNumber(match.RedAlliance.TeamA_ID);
			RedB.SetNumber(match.RedAlliance.TeamB_ID);
			RedC.SetNumber(match.RedAlliance.TeamC_ID);
			BlueA.SetNumber(match.BlueAlliance.TeamA_ID);
			BlueB.SetNumber(match.BlueAlliance.TeamB_ID);
			BlueC.SetNumber(match.BlueAlliance.TeamC_ID);
		}

		public void MergeMatchRecords()
		{
			foreach (RecordedMatch record in Records)
			{
				Merging.AdjustTeamInfo(Event, record);
			}

			Match merged = Merging.Merge(Event,
				Records[0], Records[1], Records[2],
				Records[3], Records[4], Records[5]);

			int index = Event.Matches.FindIndex((m) => m.Number == merged.Number);
			if (index != -1)
			{
				Event.Matches[index] = merged;
			}
			else
			{
				Event.Matches.Add(merged);
			}

			ScoutingJson.SaveEvent(Event, SavePath + Event.EventName + 
				ScoutingJson.EventExtension);

			ArchiveRecords(merged.Number);
			Records = new RecordedMatch[6];
			OnPropertyChanged("CanMerge");
		}

		public void ArchiveRecords(int matchNum)
		{
			string subPath = SavePath + (SavePath.EndsWith("\\") ? "" : "\\") + 
				matchNum.ToString() + "\\";
			Directory.CreateDirectory(subPath);

			List<string> files = Directory.EnumerateFiles(SavePath,
				"*" + ScoutingJson.MatchRecordExtension,
				SearchOption.TopDirectoryOnly).ToList();
			
			foreach (string file in files)
			{
				try
				{
					File.Move(file, subPath + Util.GetFileName(file, true));
				}
				catch (Exception e)
				{
					string msg = "Could not move file " + Util.GetFileName(file, true);
					Util.DebugLog(LogLevel.Error, msg);
					MessageBox.Show(msg + "\n\n" + e.Message, "Error",
						MessageBoxButton.OK, MessageBoxImage.Error);
					ProcessAllNewFolders = false;
					return;
				}
			}
		}

		private static int GetPositionFromFilename(string filename)
		{
			switch (filename)
			{
			case "RedA":
				return 0;
			case "RedB":
				return 1;
			case "RedC":
				return 2;
			case "BlueA":
				return 3;
			case "BlueB":
				return 4;
			case "BlueC":
				return 5;
			default:
				return -1;
			}
		}

		public void SendData(object sender, EventArgs<EventViewModel> e)
		{
			Event = e.Arg.Event;
			SavePath = Util.GetFolderPath(e.Arg.EventPath);
			CheckForRecordData();
		}
	}

	public class UpdateModel : INotifyPropertyChanged
	{
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
				OnPropertyChanged("Path");
			}
		}
		string _path;

		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				_isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
		bool _isSelected;

		public bool HasRecord
		{
			get
			{
				return _hasRecord;
			}
			set
			{
				_hasRecord = value;
				OnPropertyChanged("HasRecord");
				OnPropertyChanged("Tooltip");
			}
		}
		bool _hasRecord;

		public string Tooltip
		{
			get
			{
				return HasRecord ? "Record found." : "Record not found.";
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}
	
		public void Process()
		{

		}
	}

	public class TeamIndicator : INotifyPropertyChanged
	{
		bool isBlue;

		public static readonly Color LIGHT_RED = Util.MakeColor("FFFF4444");

		public bool IsReady
		{
			get
			{
				return _isReady;
			}
			set
			{
				_isReady = value;
				OnPropertyChanged("IsReady");
				OnPropertyChanged("Foreground");
				OnPropertyChanged("Bold");
			}
		}
		bool _isReady;

		public SolidColorBrush Foreground
		{
			get
			{
				if (!IsReady)
				{
					return new SolidColorBrush(Colors.Gray);
				}

				return new SolidColorBrush(isBlue ? Colors.CornflowerBlue : LIGHT_RED);
			}
		}

		public FontWeight Bold
		{
			get
			{
				return IsReady ? FontWeights.Bold : FontWeights.Normal;
			}
		}

		public string TeamNumber_String
		{
			get
			{
				if (_teamNumber_String == "-1")
				{
					return "----";
				}
				else
				{
					return _teamNumber_String;
				}
			}
			private set
			{
				_teamNumber_String = value;
				OnPropertyChanged("TeamNumber_String");
			}
		}
		string _teamNumber_String;

		public event PropertyChangedEventHandler PropertyChanged;

		public TeamIndicator(bool blue, int number)
		{
			isBlue = blue;
			_teamNumber_String = number.ToString();
		}

		public void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		public void SetNumber(int num)
		{
			TeamNumber_String = num.ToString();
		}
	}
}
