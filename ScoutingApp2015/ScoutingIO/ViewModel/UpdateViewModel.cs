using ScoutingData;
using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace ScoutingIO.ViewModel
{
	public class UpdateViewModel : INotifyPropertyChanged
	{
		public FrcEvent Event
		{ get; private set; }

		public Timer SuperTimer
		{ get; private set; }

		public bool CheckForNewFolders
		{
			get
			{
				return SuperTimer.Enabled;
			}
			set
			{
				SuperTimer.Enabled = value;
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

		public event PropertyChangedEventHandler PropertyChanged;

		public UpdateViewModel()
		{
			_checkedPaths = new ObservableCollection<UpdateModel>();

			SuperTimer = new Timer();
			SuperTimer.Interval = 2000;
			SuperTimer.Elapsed += SuperTimer_Tick;

			_processAllNewFolders = true;
		}

		// Check for files if Process All is on
		public void SuperTimer_Tick(object sender, EventArgs e)
		{
			CheckForRecordData();

			if (!ProcessAllNewFolders)
			{
				return;
			}

			foreach (UpdateModel m in CheckedPaths)
			{
				// process them
			}
		}

		public void CheckForRecordData()
		{
			foreach (string drive in Util.GetAllUsbDrivePaths())
			{
				if (ScoutingJson.HasRecordInFolder(drive))
				{
					UpdateModel added = new UpdateModel() { Path = drive, IsSelected = true };
					if (!CheckedPaths.Exists((um) => um.Path == drive))
					{
						CheckedPaths.Add(added);
					}
				}
				else
				{
					if (CheckedPaths.Exists((um) => um.Path == drive))
					{
						CheckedPaths.Remove((um) => um.Path == drive);
					}
				}
			}
			OnPropertyChanged("CheckedPaths");
		}

		public void OnPropertyChanged(string prop)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
	}

	public class UpdateModel
	{
		public string Path
		{ get; set; }

		public bool IsSelected
		{ get; set; }

		public void Process()
		{

		}
	}
}
