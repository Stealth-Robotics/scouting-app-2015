using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Ookii.Dialogs.Wpf;

using ScoutingData;
using ScoutingData.Data;
using ScoutingData.Sync;
using System.IO;
namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for SaveOverlay.xaml
	/// </summary>
	public partial class SaveOverlay : Elysium.Controls.Window
	{
		public RecordedMatch Record
		{ get; private set; }

		public int GlobalPosition
		{ get; private set; }

		public string Filename
		{
			get
			{
				switch (GlobalPosition)
				{
				case 0:
					return "RedA" + ScoutingJson.MatchRecordExtension;
				case 1:
					return "RedB" + ScoutingJson.MatchRecordExtension;
				case 2:
					return "RedC" + ScoutingJson.MatchRecordExtension;
				case 3:
					return "BlueA" + ScoutingJson.MatchRecordExtension;
				case 4:
					return "BlueB" + ScoutingJson.MatchRecordExtension;
				case 5:
					return "BlueC" + ScoutingJson.MatchRecordExtension;
				default:
					return null;
				}
			}
		}

		bool hasLoaded = false;

		public SaveOverlay(RecordedMatch record, int globalPos)
		{
			Record = record;
			GlobalPosition = globalPos;
			Record.Winner = AllianceColor.Red;

			InitializeComponent();
		}

		public void Save()
		{
			string path = SavePathBox.Text;
			ScoutingJson.SaveMatchRecord(Record, path);
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			Save();
			DialogResult = true;
			Close();
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void SavePathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string path = SavePathBox.Text;
			int lastBS = path.LastIndexOf('\\');
			string folder = path.Substring(0, lastBS + 1);

			if (!SavePathBox.Text.EndsWith(Filename))
			{
				path = folder + Filename;

				SavePathBox.Text = path;
			}

			if (!hasLoaded)
			{
				return;
			}

			if (!Directory.Exists(folder))
			{
				SavePathBox.Foreground = new SolidColorBrush(Colors.Red);
				SavePathBox.ToolTip = "Folder does not exist.";
				SaveBtn.IsEnabled = false;
				
			}
			else if (!Util.IsPathOnRemovableDevice(folder))
			{
				SavePathBox.Foreground = new SolidColorBrush(Colors.Blue);
				SavePathBox.ToolTip = "Folder is local. A removable device " +
					"folder is recommended.";
				SaveBtn.IsEnabled = true;
			}
			else
			{
				SavePathBox.Foreground = new SolidColorBrush(Colors.Black);
				SavePathBox.ToolTip = null;
				SaveBtn.IsEnabled = true;
			}
		}

		private void WinningTeamToggle_Click(object sender, RoutedEventArgs e)
		{
			if (WinningTeamToggle.IsChecked == true) // Nullable<bool>
			{
				Record.Winner = AllianceColor.Red;
				WinningTeamToggle.Background = new SolidColorBrush(MainWindow.RED_ALLIANCE);
				WinningTeamToggle.Content = "Red Wins";
			}
			else
			{
				Record.Winner = AllianceColor.Blue;
				WinningTeamToggle.Background = new SolidColorBrush(MainWindow.BLUE_ALLIANCE);
				WinningTeamToggle.Content = "Blue Wins";
			}
		}

		private void FinalScoreBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!hasLoaded)
			{
				return;
			}

			int val = -1;
			bool isValid = int.TryParse(FinalScoreBox.Text, out val);

			if (isValid)
			{
				Record.AllianceFinalScore = val;
			}
			else
			{
				FinalScoreBox.Text = Record.AllianceFinalScore.ToString();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			hasLoaded = true;

			SavePathBox.Text = (Util.GetFirstUsbDrivePath() ?? 
				ScoutingJson.LocalPath) + Filename;
		}

		private void SavePathBtn_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
			fbd.RootFolder = Environment.SpecialFolder.MyComputer;
			fbd.ShowNewFolderButton = true;
			fbd.Description = "Select a save location";
			fbd.UseDescriptionForTitle = true;

			bool? res = fbd.ShowDialog(this);

			if (res == true) // Nullable<bool>
			{
				string path = fbd.SelectedPath;
				if (!path.EndsWith("\\"))
				{
					path += "\\";
				}

				SavePathBox.Text = path + Filename;
			}
		}
	}
}
