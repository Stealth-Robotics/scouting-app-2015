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

namespace ScoutingManagerLite
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		bool hasLoaded = false;
		bool hasCrunched = false;

		public List<TeamStatsLite> StatsList
		{ get; private set; }

		public FrcEvent Event
		{ get; private set; }

		public TeamsList Teams
		{ get; private set; }

		public string RecordsRoot
		{ get; private set; }

		public List<List<RecordLite>> AllRecords
		{ get; private set; }

		public Dictionary<int, RatingSet> CrunchedNumbers
		{ get; private set; }

		public MainWindow()
		{
			StatsList = new List<TeamStatsLite>();
			AllRecords = new List<List<RecordLite>>();
			CrunchedNumbers = new Dictionary<int, RatingSet>();
			RecordsRoot = ScoutingJson.LocalPath;

			InitializeComponent();
		}

		public void LoadAllRecords()
		{
			if (!Directory.Exists(RecordsRoot))
			{
				return;
			}

			DirectoryInfo root = new DirectoryInfo(RecordsRoot);
			List<DirectoryInfo> teamInfos = root.EnumerateDirectories().ToList();
			teamInfos.RemoveAll((di) => 
			{ 
				int buf = 0; 
				return !int.TryParse(di.Name, out buf); 
			});

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
				CrunchNumbers();
			}
		}

		public void CrunchNumbers()
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

			hasCrunched = true;
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

				LoadAllRecords();
			}
		}

		private void CrunchNumbersBtn_Click(object sender, RoutedEventArgs e)
		{
			CrunchNumbers();
		}

		private void EventPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!hasLoaded || !File.Exists(EventPathBox.Text))
			{
				return;
			}

			Event = ScoutingJson.ParseFrcEvent(EventPathBox.Text);
			if (Teams != null)
			{
				Event.PostJsonLoading(Teams);
			}
		}

		private void TeamsPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!hasLoaded || !File.Exists(TeamsPathBox.Text))
			{
				return;
			}

			Teams = ScoutingJson.ParseTeamsList(TeamsPathBox.Text);
			if (Event != null)
			{
				Event.PostJsonLoading(Teams);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			hasLoaded = true;
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

			AutonomousBar.Value = vals.Autonomous;
			AutonomousTxt.Text = vals.Autonomous.ToString();
			StackingBar.Value = vals.Stacking;
			StackingTxt.Text = vals.Stacking.ToString();
			CoopertitionBar.Value = vals.Coopertition;
			CoopertitionTxt.Text = vals.Coopertition.ToString();
			ContainersBar.Value = vals.Containers;
			ContainersTxt.Text = vals.Containers.ToString();
			MobilityBar.Value = vals.Mobility;
			MobilityTxt.Text = vals.Mobility.ToString();
			EfficiencyBar.Value = vals.Efficiency;
			EfficiencyTxt.Text = vals.Efficiency.ToString();
			StabilityBar.Value = vals.Stability;
			StabilityTxt.Text = vals.Stability.ToString();
			GripBar.Value = vals.Grip;
			GripTxt.Text = vals.Grip.ToString();
			HumanSkillBar.Value = vals.HumanPlayerSkill;
			HumanSkillTxt.Text = vals.HumanPlayerSkill.ToString();
		}
	}
}
