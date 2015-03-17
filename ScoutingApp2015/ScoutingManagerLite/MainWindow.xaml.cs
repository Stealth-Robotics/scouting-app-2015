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

using Ookii.Dialogs.Wpf;
using ScoutingManagerLite.Dialog;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace ScoutingManagerLite
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		bool hasLoaded = false;
		bool hasCrunched = false;

		DispatcherTimer timer;

		public List<TeamStatsLite> StatsList
		{ get; private set; }

		public FrcEvent Event
		{ get; private set; }

		public TeamsList Teams
		{ get; private set; }

		public string RecordsRoot
		{ get; private set; }

		// This is the first time I've ended up using a nested list
		public List<List<RecordLite>> AllRecords
		{ get; private set; }

		public Dictionary<int, RatingSet> CrunchedNumbers
		{ get; private set; }

		public List<string> DrivePaths
		{ get; private set; }

		public List<LiteSummary> DataGridSummaries
		{
			get
			{
				List<LiteSummary> res = new List<LiteSummary>();
				foreach (KeyValuePair<int, RatingSet> kvp in CrunchedNumbers)
				{
					res.Add(new LiteSummary(kvp.Key, kvp.Value));
				}
				return res;
			}
		}

		public bool AutoRefresh
		{ get; private set; }

		public bool AutoImport
		{ get; private set;}

		public MainWindow()
		{
			AppSettings.Initialize();

			StatsList = new List<TeamStatsLite>();
			AllRecords = new List<List<RecordLite>>();
			CrunchedNumbers = new Dictionary<int, RatingSet>();
			DrivePaths = new List<string>();
			RecordsRoot = ScoutingJson.LocalPath;

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += timer_Tick;
			timer.Start();

			InitializeComponent();
		}

		public void UpdateEventPathBox()
		{
			if (File.Exists(EventPathBox.Text))
			{
				EventPathBox.Foreground = new SolidColorBrush(Colors.Black);
				EventPathBox.ToolTip = EventPathBox.Text;
			}
			else
			{
				EventPathBox.Foreground = new SolidColorBrush(Colors.Red);
				EventPathBox.ToolTip = EventPathBox.Text + "\nFile does not exist.";
			}
		}
		public void UpdateTeamsPathBox()
		{
			if (File.Exists(TeamsPathBox.Text))
			{
				TeamsPathBox.Foreground = new SolidColorBrush(Colors.Black);
				TeamsPathBox.ToolTip = TeamsPathBox.Text;
			}
			else
			{
				TeamsPathBox.Foreground = new SolidColorBrush(Colors.Red);
				TeamsPathBox.ToolTip = TeamsPathBox.Text + "\nFile does not exist.";
			}
		}

		public void LoadAllRecords()
		{
			if (!Directory.Exists(RecordsRoot))
			{
				return;
			}

			DirectoryInfo root = new DirectoryInfo(RecordsRoot);
			List<DirectoryInfo> teamInfos = root.EnumerateDirectories().ToList();
			teamInfos.RemoveAll((di) => !di.Name.IsInteger());

			AllRecords.Clear();

			foreach (DirectoryInfo di in teamInfos)
			{
				List<RecordLite> recordSet = new List<RecordLite>();
				List<FileInfo> recordFiles = di.EnumerateFiles().ToList();
				foreach (FileInfo fi in recordFiles)
				{
					RecordLite rec = ScoutingJson.ParseLiteRecord(fi.FullName);
					
					if (Event != null && Teams != null)
					{
						rec.PostJsonLoading(Event);
					}

					recordSet.Add(rec);
				}

				AllRecords.Add(recordSet);
			}

			if (!hasCrunched)
			{
				DoCrunching();
			}
		}

		public void DoCrunching()
		{
			// Doesn't work. I need to throw a whole bunch more Dispatcher.Invoke()
			// calls for everything.

			//CrunchingWindow win = new CrunchingWindow();
			//win.StuffToDo = CrunchNumbers;
			//
			//bool? result = win.ShowDialog();
			//if (result != true)
			//{
			//	Close();
			//}

			// This works though (no crunching window)
			CrunchNumbers();
		}

		public void RefreshDrives()
		{
			DrivePaths.Clear();
			DriveList.Items.Clear();

			List<string> allDrives = Util.GetAllUsbDrivePaths().ToList();
			foreach (string path in allDrives)
			{
				string recordsPath = path + "\\Records";

				List<string> folders = Directory.EnumerateDirectories(recordsPath).ToList();
				List<DirectoryInfo> infos = folders.ConvertAll<DirectoryInfo>(
					(s) => new DirectoryInfo(s));
				if (infos.Exists((di) => di.Name.IsInteger()))
				{
					DrivePaths.Add(recordsPath);
					DriveList.Items.Add(recordsPath);
				}
			}

			if (DriveList.Items.Count > 0)
			{
				DriveList.SelectedIndex = 0;
			}
		}

		public void ImportSelected()
		{
			if (!Directory.Exists(RecordsRoot))
			{
				return;
			}

			string fromPath = DriveList.SelectedItem as string;
			string toPath = RecordsRoot + (RecordsRoot.EndsWith("\\") ? "" : "\\");
			if (fromPath == null)
			{
				return;
			}

			DirectoryInfo recordsInfo = new DirectoryInfo(RecordsRoot);

			var infos = from s in Directory.EnumerateDirectories(fromPath)
						where Util.GetFolderName(s).IsInteger()
						select new DirectoryInfo(s);

			foreach (DirectoryInfo di in infos)
			{
				string teamNumStr = di.Name;
				var files = from f in di.EnumerateFiles()
							where f.Extension.ToLower() == ScoutingJson.LiteRecordExtension
							select f;

				if (!Directory.Exists(toPath + teamNumStr))
				{
					Directory.CreateDirectory(toPath + teamNumStr);
				}

				string teamPath = toPath + teamNumStr + "\\";

				foreach (FileInfo fi in files)
				{
					if (!File.Exists(teamPath + fi.Name))
					{
						File.Move(fi.FullName, teamPath + fi.Name);
					}
				}

				// clean up
				Directory.Delete(toPath + teamNumStr, true);
			}

			RefreshDrives();
			LoadAllRecords();
		}

		private void CrunchNumbers()
		{
			if (AllRecords.Count <= 0)
			{
				return;
			}

			CrunchedNumbers.Clear();
			StatsList.Clear();

			foreach (List<RecordLite> recordSet in AllRecords)
			{
				if (recordSet.Count <= 0)
				{
					continue;
				}

				int teamNum = recordSet.First().TeamID;
				TeamStatsLite stats = TeamStatsLite.MakeStats(recordSet);
				StatsList.Add(stats);

				CrunchedNumbers.Add(teamNum, stats.GetMeanRatings());
			}

			InfoTeamsList.Items.Clear();
			foreach (TeamStatsLite stat in StatsList)
			{
				InfoTeamsList.Items.Add(stat);
			}

			if (InfoTeamsList.Items.Count > 0)
			{
				InfoTeamsList.SelectedIndex = 0;
			}

			hasCrunched = true;

			SortDataGrid.ItemsSource = DataGridSummaries;
		}

		public void LoadSettings()
		{
			if (!hasLoaded)
			{
				return;
			}

			EventPathBox.Text = AppSettings.EventPath;
			TeamsPathBox.Text = AppSettings.TeamsPath;

			RecordsRoot = AppSettings.RecordsPath;
			RecordsPathBox.Text = AppSettings.RecordsPath;
			LoadAllRecords();
		}

		#region event handlers

		private void timer_Tick(object sender, EventArgs e)
		{
			if (!hasLoaded)
			{
				return;
			}

			if (!AutoRefresh)
			{
				return;
			}
			RefreshDrives();

			if (!AutoImport)
			{
				return;
			}
			ImportSelected();
		}

		private void EventPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.Title = "Open FRC Event";
			ofd.Multiselect = false;
			ofd.Filter = "FRC Event Files (*.frc)|*.frc";

			bool? result = ofd.ShowDialog();

			if (result == true)
			{
				EventPathBox.Text = ofd.FileName;
			}
		}

		private void TeamsPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.Title = "Open Teams List";
			ofd.Multiselect = false;
			ofd.Filter = "Teams List File (*.teams)|*.teams";

			bool? result = ofd.ShowDialog();

			if (result == true)
			{
				TeamsPathBox.Text = ofd.FileName;
			}
		}

		private void RecordsPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var fbd = new VistaFolderBrowserDialog();
			fbd.UseDescriptionForTitle = true;
			fbd.Description = "Select root path...";

			bool? result = fbd.ShowDialog();

			if (result == true)
			{
				RecordsPathBox.Text = fbd.SelectedPath;
				RecordsRoot = fbd.SelectedPath;
				AppSettings.RecordsPath = RecordsRoot;

				LoadAllRecords();
			}
		}

		private void CrunchNumbersBtn_Click(object sender, RoutedEventArgs e)
		{
			DoCrunching();
		}

		private void EventPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!hasLoaded || !File.Exists(EventPathBox.Text))
			{
				return;
			}

			AppSettings.EventPath = EventPathBox.Text;

			Event = ScoutingJson.ParseFrcEvent(EventPathBox.Text);
			if (Teams != null)
			{
				Event.PostJsonLoading(Teams);
			}

			UpdateEventPathBox();
		}

		private void TeamsPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!hasLoaded || !File.Exists(TeamsPathBox.Text))
			{
				return;
			}

			AppSettings.TeamsPath = TeamsPathBox.Text;

			Teams = ScoutingJson.ParseTeamsList(TeamsPathBox.Text);
			if (Event != null)
			{
				Event.PostJsonLoading(Teams);
			}

			UpdateTeamsPathBox();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			hasLoaded = true;

			LoadSettings();

			if (Directory.Exists(EventPathBox.Text))
			{
				Event = ScoutingJson.ParseFrcEvent(EventPathBox.Text);
			}
			if (Directory.Exists(TeamsPathBox.Text))
			{
				Teams = ScoutingJson.ParseTeamsList(TeamsPathBox.Text);

				if (Event != null)
				{
					Event.PostJsonLoading(Teams);
				}
			}

			SortDataGrid.ItemsSource = DataGridSummaries;

			UpdateEventPathBox();
			UpdateTeamsPathBox();

			RefreshDrives();
		}

		private void InfoTeamsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TeamStatsLite stat = InfoTeamsList.SelectedItem as TeamStatsLite;
			if (stat == null)
			{
				return;
			}

			TeamNameTxt.Text = stat.Team.ToString();
			TeamLocationTxt.Text = "From " + stat.Team.School;
			TeamDescriptionTxt.Text = stat.Team.Description;

			RatingSet vals = stat.GetMeanRatings();

			OverallBar.Value = vals.OverallRating;
			OverallTxt.Text = vals.OverallRating.ToString("f3");

			AutonomousBar.Value = vals.Autonomous;
			AutonomousTxt.Text = vals.Autonomous.ToString("f3");
			StackingBar.Value = vals.Stacking;
			StackingTxt.Text = vals.Stacking.ToString("f3");
			CoopertitionBar.Value = vals.Coopertition;
			CoopertitionTxt.Text = vals.Coopertition.ToString("f3");
			ContainersBar.Value = vals.Containers;
			ContainersTxt.Text = vals.Containers.ToString("f3");
			MobilityBar.Value = vals.Mobility;
			MobilityTxt.Text = vals.Mobility.ToString("f3");
			EfficiencyBar.Value = vals.Efficiency;
			EfficiencyTxt.Text = vals.Efficiency.ToString("f3");
			StabilityBar.Value = vals.Stability;
			StabilityTxt.Text = vals.Stability.ToString("f3");
			GripBar.Value = vals.Grip;
			GripTxt.Text = vals.Grip.ToString("f3");
			HumanSkillBar.Value = vals.HumanPlayerSkill;
			HumanSkillTxt.Text = vals.HumanPlayerSkill.ToString("f3");
		}

		private void RefreshBtn_Click(object sender, RoutedEventArgs e)
		{
			RefreshDrives();
		}

		private void ImportBtn_Click(object sender, RoutedEventArgs e)
		{
			ImportSelected();
		}

		private void AutoRefreshCheck_Click(object sender, RoutedEventArgs e)
		{
			AutoRefresh = AutoRefreshCheck.IsChecked ?? false;
		}

		private void AutoImportCheck_Click(object sender, RoutedEventArgs e)
		{
			AutoImport = AutoImportCheck.IsChecked ?? false;
		}
	}
		#endregion

	public class LiteSummary
	{
		RatingSet set;

		public int Team
		{ get; private set; }

		public double Overall
		{
			get
			{
				return set.OverallRating;
			}
		}
		public double Auto
		{
			get
			{
				return set.Autonomous;
			}
		}
		public double Stacking
		{
			get
			{
				return set.Stacking;
			}
		}
		public double Coop
		{
			get
			{
				return set.Coopertition;
			}
		}
		public double Containers
		{
			get
			{
				return set.Containers;
			}
		}
		public double Mobility
		{
			get
			{
				return set.Mobility;
			}
		}
		public double Efficiency
		{
			get
			{
				return set.Efficiency;
			}
		}
		public double Stability
		{
			get
			{
				return set.Stability;
			}
		}
		public double Grip
		{
			get
			{
				return set.Grip;
			}
		}
		public double Players
		{
			get
			{
				return set.HumanPlayerSkill;
			}
		}

		public LiteSummary(int num, RatingSet _set)
		{
			set = _set;
			Team = num;
		}
	}
}
