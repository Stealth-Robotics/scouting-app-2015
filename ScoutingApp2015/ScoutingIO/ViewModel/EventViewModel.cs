using Framework.UI.Controls;
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
using System.Windows.Input;
using System.Windows.Media;

namespace ScoutingIO.ViewModel
{
	public class EventViewModel : INotifyPropertyChanged
	{
		public event EventHandler<EventViewModel> SendData;

		public event PropertyChangedEventHandler PropertyChanged;

		public FrcEvent Event
		{
			get
			{
				return _frc;
			}
			set
			{
				_frc = value;
				OnPropertyChanged("Event");
			}
		}
		FrcEvent _frc;

		public string Event_EventName
		{
			get
			{
				if (Event != null)
				{
					return Event.EventName;
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				Event.EventName = value;
				OnPropertyChanged("Event_EventName");
			}
		}

		public string EventPath
		{
			get
			{
				return _eventPath;
			}
			set
			{
				_eventPath = value;
				OnPropertyChanged("EventPath");
			}
		}
		public string _eventPath;

		public SolidColorBrush EventPath_Color
		{
			get
			{
				return _eventPath_Color;
			}
			set
			{
				_eventPath_Color = value;
				OnPropertyChanged("EventPath_Color");
			}
		}
		public SolidColorBrush _eventPath_Color;

		public string EventPath_Tooltip
		{
			get
			{
				return _eventPath_Tooltip;
			}
			set
			{
				_eventPath_Tooltip = value;
				OnPropertyChanged("EventPath_Tooltip");
			}
		}
		public string _eventPath_Tooltip;

		public ICommand SelectPathCmd
		{ get; private set; }

		public ICommand NewEventCmd
		{ get; private set; }

		// DEBUG
		public ICommand BreakCmd
		{ get; private set; }

		public EventViewModel()
		{
			SelectPathCmd = new DoStuffCommand(UILoadEvent, obj => true);
			NewEventCmd = new DoStuffCommand(NewEvent, obj => true);

			BreakCmd = new DoStuffCommand(Break, (o) => true);

			LoadDefault();
		}

		public void LoadDefault()
		{
			EventPath = ScoutingJson.LocalPath + "DefaultEvent.frc";
		}

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}

			if (propertyName == "Event")
			{
				if (SendData != null)
				{
					SendData(this, new EventArgs<EventViewModel>(this));
				}

				OnPropertyChanged("Event_EventName");
			}

			if (propertyName == "EventPath")
			{
				LoadEventFile(EventPath);
			}
		}

		public bool IsPathValid()
		{
			if (!File.Exists(EventPath))
			{
				return false;
			}

			FrcEvent frc = ScoutingJson.ParseFrcEvent(EventPath);

			if (frc == null)
			{
				return false;
			}

			return frc.EventName != null && frc.EventName != "";
		}

		public void UILoadEvent()
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.CheckPathExists = true;
			ofd.DefaultExt = ScoutingJson.EventExtension;
			ofd.Title = "Open Event File";
			ofd.InitialDirectory = ScoutingJson.LocalPath;
			ofd.Filter = "FRC Event Files (*.frc)|*.frc";

			bool? dialogResult = ofd.ShowDialog();

			if (dialogResult == true) // Nullable<bool>
			{
				EventPath = ofd.FileName;
				LoadEventFile(ofd.FileName);
			}
		}

		public void Break()
		{
			System.Diagnostics.Debugger.Break();
		}

		public void LoadEventFile(string file)
		{
			if (!File.Exists(file))
			{
				Event = null;
			}
			else
			{
				Event = ScoutingJson.ParseFrcEvent(file);
			}

			EventPath_Color = GetPathBrush();
			EventPath_Tooltip = GetPathTooltip();
			
			if (SendData != null)
			{
				SendData(this, new EventArgs<EventViewModel>(this));
			}
		}

		public void NewEvent()
		{
			NewEventDialog ned = new NewEventDialog(EventPath);
			bool? res = ned.ShowDialog();

			if (res == true) // Nullable<bool>
			{
				MessageBoxResult mbr = MessageBox.Show(
					"Do you want to save this event before creating a new one?",
					"Save Event", MessageBoxButton.YesNoCancel);

				if (mbr == MessageBoxResult.Yes)
				{
					SaveAll();
				}
				else if (mbr == MessageBoxResult.Cancel)
				{
					return;
				}

				FrcEvent frc = new FrcEvent(ned.EventName);
				Event = frc;
				ScoutingJson.SaveEvent(Event, ned.Path);
				EventPath = ned.Path;
			}
		}

		public SolidColorBrush GetPathBrush()
		{
			if (IsPathValid())
			{
				if (Util.IsPathOnRemovableDevice(EventPath))
				{
					return new SolidColorBrush(Colors.Orange);
				}

				// Ideal: save root event locally (or cloud folder)
				return new SolidColorBrush(Colors.Black);
			}
			else
			{
				return new SolidColorBrush(Colors.Red);
			}
		}
		public string GetPathTooltip()
		{
			if (EventPath_Color.Color == Colors.Red)
			{
				return EventPath + "\n\nFile does not exist.";
			}
			else if (EventPath_Color.Color == Colors.Orange)
			{
				return EventPath + "\n\nFile is on removable device. Ideally " +
					"this would be on a local folder or in a cloud storage folder.";
			}
			else
			{
				return EventPath;
			}
		}

		public void SaveAll()
		{
			ScoutingJson.SaveEvent(Event, EventPath);
		}

		public void SendTeamsData(object sender, EventArgs<TeamsViewModel> e)
		{
			TeamsList list = e.Arg.Teams;

			if (list != null)
			{
				Event.PostJsonLoading(list);
			}
		}
	}
}
