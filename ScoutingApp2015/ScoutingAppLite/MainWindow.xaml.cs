using ScoutingAppLite.View;
using ScoutingData;
using ScoutingData.Data;
using ScoutingData.Lite;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScoutingAppLite
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		static readonly Color LIGHT_RED = Util.MakeColor(0xFFFF8888);

		bool hasLoaded = false;

		public FrcEvent Event
		{ get; private set; }

		public TeamsList Teams
		{ get; private set; }

		public Match SelectedMatch
		{
			get
			{
				return _selectedMatch;
			}
			set
			{
				_selectedMatch = value;
				UpdateRatingViewsMatch(value);
			}
		}
		Match _selectedMatch;

		public MainWindow()
		{
			AppSettings.Initialize();

			InitializeComponent();
		}

		public void LoadEvent(string filepath)
		{
			if (!File.Exists(filepath))
			{
				return;
			}

			FrcEvent temp = ScoutingJson.ParseFrcEvent(filepath);
			if (temp != null)
			{
				Event = temp;
				AppSettings.EventFile = filepath;

				if (Teams != null)
				{
					Event.PostJsonLoading(Teams);
				}

				SelectedMatch = Event.Matches.FirstOrDefault();
			}

			UpdateTextBoxInfo();
		}

		public void LoadTeams(string filepath)
		{
			if (!File.Exists(filepath))
			{
				return;
			}

			TeamsList temp = ScoutingJson.ParseTeamsList(filepath);
			if (temp != null)
			{
				AppSettings.TeamsFile = filepath;

				if (Event != null)
				{
					Event.PostJsonLoading(temp);
					UpdateRatingViewsMatch(SelectedMatch);
				}
				
				Teams = temp;
			}

			UpdateTextBoxInfo();
		}

		public void UpdateTextBoxInfo()
		{
			if (!File.Exists(EventPathBox.Text))
			{
				EventPathBox.Foreground = new SolidColorBrush(LIGHT_RED);
				EventPathBox.ToolTip = EventPathBox.Text + "\nFile does not exist.";
			}
			else
			{
				EventPathBox.Foreground = new SolidColorBrush(Colors.White);
				EventPathBox.ToolTip = EventPathBox.Text;
			}

			if (!File.Exists(TeamsPathBox.Text))
			{
				TeamsPathBox.Foreground = new SolidColorBrush(LIGHT_RED);
				TeamsPathBox.ToolTip = TeamsPathBox.Text + "\nFile does not exist.";
			}
			else
			{
				TeamsPathBox.Foreground = new SolidColorBrush(Colors.White);
				TeamsPathBox.ToolTip = TeamsPathBox.Text;
			}

			if (Event == null)
			{
				MatchIDBox.IsEnabled = false;
				MatchID_UpBtn.IsEnabled = false;
				MatchID_DownBtn.IsEnabled = false;
			}
			else
			{
				MatchIDBox.IsEnabled = true;
				MatchID_UpBtn.IsEnabled = true;
				MatchID_DownBtn.IsEnabled = true;
			}
		}

		public void UpdateMatchIDBoxInfo(int id)
		{
			if (Event == null)
			{ 
				return; 
			}

			if (Event.LoadMatch(id) != null)
			{
				MatchIDBox.Foreground = new SolidColorBrush(Colors.White);
				MatchIDBox.ToolTip = null;
			}
			else
			{
				MatchIDBox.Foreground = new SolidColorBrush(LIGHT_RED);
				MatchIDBox.ToolTip = "Match does not exist.";
			}
		}

		public void UpdateRatingViewsMatch(Match m)
		{
			BlueAView.SetMatchContext(m);
			BlueBView.SetMatchContext(m);
			BlueCView.SetMatchContext(m);
			RedAView.SetMatchContext(m);
			RedBView.SetMatchContext(m);
			RedCView.SetMatchContext(m);
		}

		public List<RecordLite> MakeRecords()
		{
			List<RecordLite> res = new List<RecordLite>();

			if (BlueAView.IsRecording)
			{
				res.Add(BlueAView.MakeRecord());
			}
			if (BlueBView.IsRecording)
			{
				res.Add(BlueBView.MakeRecord());
			}
			if (BlueCView.IsRecording)
			{
				res.Add(BlueCView.MakeRecord());
			}
			if (RedAView.IsRecording)
			{
				res.Add(RedAView.MakeRecord());
			}
			if (RedBView.IsRecording)
			{
				res.Add(RedBView.MakeRecord());
			}
			if (RedCView.IsRecording)
			{
				res.Add(RedCView.MakeRecord());
			}

			return res;
		}

		public void SaveAll()
		{
			List<RecordLite> records = MakeRecords();

			ConfirmSaveDialog csd = new ConfirmSaveDialog();
			bool? result = csd.ShowDialog();
			if (result == true)
			{
				string rootFolder = csd.SelectedPath;
				if (!rootFolder.EndsWith("\\")) ;
				{
					rootFolder += "\\";
				}

				if (!Directory.Exists(rootFolder))
				{
					Directory.CreateDirectory(rootFolder);
				}

				foreach (RecordLite rec in records)
				{
					string teamFolder = rootFolder + rec.TeamID.ToString() + "\\";
					if (!Directory.Exists(teamFolder))
					{
						Directory.CreateDirectory(teamFolder);
					}

					string recordPath = teamFolder + "Match" + rec.MatchID.ToString("00") +
						ScoutingJson.LiteRecordExtension;
					ScoutingJson.SaveLiteRecord(rec, recordPath);
				}
			}
		}

		#region event handlers
		private void EventPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.DefaultExt = ScoutingJson.EventExtension;
			ofd.Filter = "FRC Event Files (*.frc)|*.frc";
			ofd.Title = "Open FRC Event...";

			bool? result = ofd.ShowDialog();
			if (result == true)
			{
				EventPathBox.Text = ofd.FileName;
			}
		}

		private void TeamsPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.DefaultExt = ScoutingJson.TeamsListExtension;
			ofd.Filter = "FRC Teams List (*.teams)|*.teams";
			ofd.Title = "Open Teams List...";

			bool? result = ofd.ShowDialog();
			if (result == true)
			{
				TeamsPathBox.Text = ofd.FileName;
			}
		}

		private void MatchID_DownBtn_Click(object sender, RoutedEventArgs e)
		{
			int id = -1;
			bool worked = int.TryParse(MatchIDBox.Text, out id);
			if (worked)
			{
				id = Math.Max(0, id - 1);

				MatchIDBox.Text = id.ToString();
			}
		}

		private void MatchID_UpBtn_Click(object sender, RoutedEventArgs e)
		{
			int id = -1;
			bool worked = int.TryParse(MatchIDBox.Text, out id);
			if (worked)
			{
				id++;

				MatchIDBox.Text = id.ToString();
			}
		}

		private void MatchIDBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int id = -1;
			bool worked = int.TryParse(MatchIDBox.Text, out id);
			if (worked)
			{
				UpdateMatchIDBoxInfo(id);

				if (Event != null)
				{
					UpdateRatingViewsMatch(Event.LoadMatch(id));
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			hasLoaded = true;

			EventPathBox.Text = AppSettings.EventFile;
			LoadEvent(AppSettings.EventFile);
			TeamsPathBox.Text = AppSettings.TeamsFile;
			LoadTeams(AppSettings.TeamsFile);

			UpdateTextBoxInfo();
		}

		private void EventPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (hasLoaded)
			{
				LoadEvent(EventPathBox.Text);
			}
		}

		private void TeamsPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (hasLoaded)
			{
				LoadTeams(TeamsPathBox.Text);
			}
		}

		private void BreakBtn_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debugger.Break();
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			SaveAll();
		}

		private void Any_IsRecordingChanged(object sender, bool e)
		{
			if (BlueAView.IsRecording || BlueBView.IsRecording || BlueCView.IsRecording ||
				RedAView.IsRecording || RedBView.IsRecording || RedCView.IsRecording)
			{
				SaveBtn.IsEnabled = true;
			}
			else
			{
				SaveBtn.IsEnabled = false;
			}
		}
	}
		#endregion
}
