using ScoutingData;
using ScoutingData.Analysis;
using ScoutingData.Data;
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

namespace ScoutingStats
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		public static readonly Color LIGHT_RED = Util.MakeColor("FFFF4444");
		public static readonly Color LIGHT_BLUE = Util.MakeColor("FF4444FF");
		public static readonly Color TITLE_GRAY = Util.MakeColor("FF3E3E3E");

		public FrcEvent Event
		{ get; private set; }

		public TeamsList Teams
		{ get; private set; }

		public EventAnalysis Analysis
		{ get; private set; }

		public Match SelectedMatch
		{
			get
			{
				return MatchSelectionList.SelectedItem as Match;
			}
		}
		public Team SelectedTeam
		{
			get
			{
				return TeamSelectionList.SelectedItem as Team;
			}
		}

		public string EventPath
		{ get; private set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		public void LoadCreateAnalysis()
		{
			string analysisPath = EventPath + "\\" + Event.EventName +
				ScoutingJson.AnalysisExtension;

			if (File.Exists(analysisPath))
			{
				Analysis = ScoutingJson.ParseAnalysis(analysisPath);
			}
			else
			{
				Analysis = new EventAnalysis(Event);
			}
			Analysis.PostJsonLoading(Event);
		}

		public void SaveAnalysis()
		{
			ScoutingJson.SaveAnalysis(Analysis, EventPath + "\\" +
				Event.EventName + ScoutingJson.AnalysisExtension);
		}

		public void Calculate()
		{
			Analysis.Calculate();
			SaveAnalysis();
		}

		public void UpdateMatchesTab()
		{

		}

		public void UpdateTeamsTab()
		{

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var spsd = new StartupPathSelectionDialog();
			bool? res = spsd.ShowDialog();

			if (res != true)
			{
				Application.Current.Shutdown();
				return;
			}

			Event = ScoutingJson.ParseFrcEvent(spsd.EventPath);
			Teams = ScoutingJson.ParseTeamsList(spsd.TeamsPath);
			EventPath = Util.GetFolderPath(spsd.EventPath);

			if (Event != null && Teams != null)
			{
				Event.PostJsonLoading(Teams);
			}

			LoadCreateAnalysis();
			MatchSelectionList.ItemsSource = Event.Matches;
		}

		private void MatchSelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
