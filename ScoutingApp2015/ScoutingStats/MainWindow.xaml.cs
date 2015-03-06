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
		public static readonly Color WORKING_GREEN = Colors.Green;
		public static readonly Color MALFUNCTIONING_RED = Util.MakeColor("FFB20000");

		private bool hasLoaded;

		public FrcEvent Event
		{ get; private set; }

		public TeamsList Teams
		{ get; private set; }

		public EventAnalysis FrcAnalysis
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
				FrcAnalysis = ScoutingJson.ParseAnalysis(analysisPath);
			}
			else
			{
				FrcAnalysis = new EventAnalysis(Event);
			}
			FrcAnalysis.PostJsonLoading(Event);
		}

		public void SaveAnalysis()
		{
			ScoutingJson.SaveAnalysis(FrcAnalysis, EventPath + "\\" +
				Event.EventName + ScoutingJson.AnalysisExtension);
		}

		public void Calculate()
		{
			FrcEvent before = Event;
			try
			{
				Event = ScoutingJson.ParseFrcEvent(EventPath + "\\" +
						Event.EventName + ScoutingJson.EventExtension);
				FrcAnalysis.Event = Event;
			}
			catch (Exception e)
			{
				Util.DebugLog(LogLevel.Error, "ANALYSIS",
					 "Could not load Event:\n\t" +e.Message);
				Event = before;
			}

			FrcAnalysis.Calculate();
			SaveAnalysis();
		}

		public void UpdateMatchesTab()
		{
			Match selected = MatchSelectionList.SelectedItem as Match;
			MatchAnalysis selectedAnalysis = FrcAnalysis.LoadMatch(selected.Number);

			if (selected == null)
			{
				Util.DebugLog(LogLevel.Error, "ANALYSIS", 
					"Selected match was null or not a match at all.");
			}

			UpdateMatchSummary(selected);
			UpdateMatchPregame(selected, selectedAnalysis);
			UpdateMatchPostgame(selected, selectedAnalysis);
		}

		public void UpdateTeamsTab()
		{
			Team selected = TeamSelectionList.SelectedItem as Team;
			TeamAnalysis selectedAnalysis = FrcAnalysis.LoadTeam(selected.Number);

			if (selected == null)
			{
				Util.DebugLog(LogLevel.Error, "ANALYSIS",
					"Selected team was null or not a team at all.");
			}

			UpdateTeamSummary(selected);
			UpdateTeamAnalysis(selected, selectedAnalysis);
		}

		#region sub-update functions

		private void UpdateMatchSummary(Match match)
		{
			if (match == null)
			{
				MatchNumberTxt.Text = "Match NULL";
				MatchRedTeamsTxt.Text = "####-####-####";
				MatchBlueTeamsTxt.Text = "####-####-####";

				MatchFinalScoreRedTxt.Text = "NULL";
				MatchFinalScoreBlueTxt.Text = "NULL";

				MatchFinalScoreLabelTxt.Visibility = System.Windows.Visibility.Collapsed;
				MatchFinalScoreRedTxt.Visibility = System.Windows.Visibility.Collapsed;
				MatchFinalScoreToTxt.Visibility = System.Windows.Visibility.Collapsed;
				MatchGoalCountBlueTxt.Visibility = System.Windows.Visibility.Collapsed;
				return;
			}

			MatchNumberTxt.Text = "Match " + match.Number.ToString();
			MatchRedTeamsTxt.Text = match.RedAlliance.ToString();
			MatchBlueTeamsTxt.Text = match.BlueAlliance.ToString();

			if (match.Pregame)
			{
				MatchFinalScoreRedTxt.Text = "???";
				MatchFinalScoreBlueTxt.Text = "???";

				MatchFinalScoreLabelTxt.Visibility = System.Windows.Visibility.Collapsed;
				MatchFinalScoreRedTxt.Visibility = System.Windows.Visibility.Collapsed;
				MatchFinalScoreToTxt.Visibility = System.Windows.Visibility.Collapsed;
				MatchFinalScoreBlueTxt.Visibility = System.Windows.Visibility.Collapsed;
			}
			else
			{
				MatchFinalScoreRedTxt.Text = match.RedFinalScore.ToString();
				MatchFinalScoreBlueTxt.Text = match.BlueFinalScore.ToString();

				MatchFinalScoreLabelTxt.Visibility = System.Windows.Visibility.Visible;
				MatchFinalScoreRedTxt.Visibility = System.Windows.Visibility.Visible;
				MatchFinalScoreToTxt.Visibility = System.Windows.Visibility.Visible;
				MatchFinalScoreBlueTxt.Visibility = System.Windows.Visibility.Visible;
			}
		}
		private void UpdateMatchPregame(Match match, MatchAnalysis analysis)
		{
			if (match == null || analysis == null)
			{
				MatchWinrateRedTxt.Text = "NULL";
				MatchWinrateBlueTxt.Text = "NULL";

				MatchExpectedWinnerTxt.Text = "NULL";
				MatchExpectedWinnerTxt.Foreground = new SolidColorBrush(TITLE_GRAY);
				MatchAdvantageTxt.Text = "NULL";

				MatchExpectedFinalScoreRedTxt.Text = "NULL";
				MatchExpectedFinalScoreBlueTxt.Text = "NULL";

				MatchProfileTxt.Text = "NULL";
				return;
			}

			MatchWinrateRedTxt.Text = analysis.RedWinRateMean.ToStringPct();
			MatchWinrateBlueTxt.Text = analysis.BlueWinRateMean.ToStringPct();

			MatchExpectedWinnerTxt.Text = analysis.ExpectedWinner.ToString();
			MatchExpectedWinnerTxt.Foreground = new SolidColorBrush(
				analysis.ExpectedWinner == AllianceColor.Red ? 
				LIGHT_RED : LIGHT_BLUE);
			MatchAdvantageTxt.Text = analysis.Advantage.ToString();

			MatchExpectedFinalScoreRedTxt.Text = analysis.RedExpectedFinalScore.ToString();
			MatchExpectedFinalScoreBlueTxt.Text = analysis.BlueExpectedFinalScore.ToString();

			MatchProfileTxt.Text = analysis.GameProfileValue.ToString();
		}
		private void UpdateMatchPostgame(Match match, MatchAnalysis analysis)
		{
			if (match == null || analysis == null)
			{
				MatchDefenseRedTxt.Text = "NULL";
				MatchDefenseBlueTxt.Text = "NULL";

				MatchGoalCountRedTxt.Text = "NULL";
				MatchGoalCountBlueTxt.Text = "NULL";
				return;
			}

			if (analysis.Pregame)
			{
				MatchDefenseRedTxt.Text = "PREGAME";
				MatchDefenseBlueTxt.Text = "PREGAME";

				MatchGoalCountRedTxt.Text = "PREGAME";
				MatchGoalCountBlueTxt.Text = "PREGAME";
				return;
			}

			MatchDefenseRedTxt.Text = analysis.RedDefenseMean.Value.ToString();
			MatchDefenseBlueTxt.Text = analysis.BlueDefenseMean.Value.ToString();

			MatchGoalCountRedTxt.Text = analysis.RedGoalCount.Value.ToString();
			MatchGoalCountBlueTxt.Text = analysis.BlueGoalCount.Value.ToString();
		}

		private void UpdateTeamSummary(Team team)
		{
			if (team == null)
			{
				TeamHeaderTxt.Text = "NULL";
				TeamLocationTxt.Text = "NULL";
				TeamDescriptionTxt.Text = "NULL";
				return;
			}

			TeamHeaderTxt.Text = team.ToString();
			TeamLocationTxt.Text = "From " + team.School;
			TeamDescriptionTxt.Text = team.Description;
		}
		private void UpdateTeamAnalysis(Team team, TeamAnalysis analysis)
		{
			if (team == null || analysis == null)
			{
				TeamWorkingTxt.Text = "NULL";
				TeamWorkingTxt.Foreground = new SolidColorBrush(TITLE_GRAY);

				WinrateValText.Text = "NULL";
				WinRateZText.Text = "NULL";

				ScoredPointsValText.Text = "NULL";
				ScoredPointsZText.Text = "NULL";

				FinalScoreValText.Text = "NULL";
				FinalScoreZText.Text = "NULL";

				PenaltiesValText.Text = "NULL";
				PenaltiesZText.Text = "NULL";

				DefenseValText.Text = "NULL";
				DefenseZText.Text = "NULL";

				ResponsivenessValText.Text = "NULL";
				ResponsivenessZText.Text = "NULL";
				return;
			}

			TeamWorkingTxt.Text = analysis.WorkingCurrently ? "WORKING" : "MALFUNCTIONING";
			TeamWorkingTxt.Foreground = new SolidColorBrush(
				analysis.WorkingCurrently ? WORKING_GREEN : MALFUNCTIONING_RED);

			WinrateValText.Text = analysis.WinRate.ToStringPct();
			WinRateZText.Text = "[z = " + analysis.WinRateZ.ToString() + "]";

			ScoredPointsValText.Text = analysis.ScoredPoints.ToString();
			ScoredPointsZText.Text = "[z = " + analysis.ScoredPoints.CenterZScore.ToString() + "]";

			FinalScoreValText.Text = analysis.FinalScore.ToString();
			FinalScoreZText.Text = "[z = " + analysis.FinalScore.CenterZScore.ToString() + "]";

			PenaltiesValText.Text = analysis.Penalties.ToString();
			PenaltiesZText.Text = "[z = " + analysis.Penalties.CenterZScore.ToString() + "]";

			DefenseValText.Text = analysis.Defense.ToString();
			DefenseZText.Text = "[z = " + analysis.Defense.CenterZScore.ToString() + "]";

			ResponsivenessValText.Text = analysis.ResponsivenessRate.ToStringPct();
			ResponsivenessZText.Text = "[z = " + analysis.ResponsivenessRateZ.ToString() + "]";
		}

		#endregion

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
			hasLoaded = true;

			LoadCreateAnalysis();
			MatchSelectionList.ItemsSource = Event.Matches;
		}

		private void MatchSelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!hasLoaded)
			{
				return;
			}

			UpdateMatchesTab();
		}

		private void TeamSelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!hasLoaded)
			{
				return;
			}

			UpdateTeamsTab();
		}

		private void RecalculateBtn_Click(object sender, RoutedEventArgs e)
		{
			RecalculateBtn.Visibility = System.Windows.Visibility.Collapsed;
			RecalculateProgress.Visibility = System.Windows.Visibility.Visible;

			Calculate();

			UpdateMatchesTab();
			UpdateTeamsTab();

			RecalculateBtn.Visibility = System.Windows.Visibility.Visible;
			RecalculateProgress.Visibility = System.Windows.Visibility.Collapsed;
		}
	}
}
