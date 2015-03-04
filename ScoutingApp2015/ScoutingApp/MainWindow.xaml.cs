using Framework.UI.Controls;
using ScoutingData;
using ScoutingData.Data;
using ScoutingData.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public sealed partial class MainWindow : Elysium.Controls.Window
	{
		#region Constants
		public const string DEFENSE_PREFIX = "Defense: ";
		public const string LEVEL_PREFIX = "Level: ";

		public static readonly TimeSpan UPDATE_INTERVAL =
			TimeSpan.FromMilliseconds(50.0);

		public static readonly Color RED_ALLIANCE = Util.MakeColor("FFFF2222");
		public static readonly Color BLUE_ALLIANCE = Util.MakeColor("FF2E71FF");

		public static readonly Color WORKING_GREEN = Util.MakeColor("FF40FF40");
		public static readonly Color MALFUNCTIONING_RED = Util.MakeColor("FF880000");

		public static readonly Color AUTO_COLOR = Colors.Yellow;
		public static readonly Color TELEOP_COLOR = Colors.Lime;
		public static readonly Color CHAOS_COLOR = Util.MakeColor("FFAA0000");
		#endregion

		public DispatcherTimer DispTimer
		{ get; private set; }

		public Stopwatch Watch
		{ get; private set; }

		public bool IsSettingUp
		{ get; private set; }

		public RecordedMatch Record
		{ get; private set; }

		public AllianceColor Color
		{ get; private set; }

		public Team SelectedTeam
		{ get; private set; }

		public FrcEvent Frc
		{ get; private set; }

		public Match PregameMatch
		{ get; private set; }

		public TimeSpan Time
		{ get; private set; }

		public bool GameInProgress
		{
			get
			{
				return _gameInProgress;
			}
			set
			{
				_gameInProgress = value;

				if (!value)
				{
					SetEnabledAuto(false);
					SetEnabledTeleop(false);

					TeamsDropDown.IsEnabled = true;
					MatchSetupBtn.IsEnabled = true;
				}
				else
				{
					TeamsDropDown.IsEnabled = false;
					MatchSetupBtn.IsEnabled = false;
				}
			}
		}
		bool _gameInProgress = false;

		public List<TeamDataContext> TeamLineup
		{ get; set; }

		public bool IsGoalSelected
		{
			get
			{
				return GoalsList.SelectedIndex != -1 && GoalsList.Items.Count > 0;
			}
		}

		public MainWindow()
		{
			ScoutingAppSettings.Initialize();

			DispTimer = new DispatcherTimer();
			Watch = new Stopwatch();

			InitializeComponent();
		}

		public void DoTeleopStuff()
		{
			if (RobotSetToggle.IsChecked == true) // Nullable<bool>
			{
				AddGoal(Goal.MakeRobotSet(Color));
			}
			if (ContainerSetToggle.IsChecked == true)
			{
				AddGoal(Goal.MakeContainerSet(Color));
			}
			if (ToteSetToggle.IsChecked == true)
			{
				AddGoal(Goal.MakeYellowToteSet(
					ToteSetStackedToggle.IsChecked == true, Color));
			}
		}

		public void EndGame()
		{
			AddAllUnprocessedLitter();

			GameInProgress = false;
			PenaltyBtn.IsEnabled = false;

			SaveBtn.IsEnabled = true;

			TimeBar.Tag = "AUTO";
			Time = TimeSpan.Zero;
			TimeBtn.Content = "Start";
			TimeBtn.IsEnabled = false;
			TimeBar.Value = 0;
			StopBtn.IsEnabled = false;
		}

		public void SetEnabledAuto(bool en)
		{
			RobotSetToggle.IsEnabled = en;
			ContainerSetToggle.IsEnabled = en;
			ToteSetToggle.IsEnabled = en;
			ToteSetStackedToggle.IsEnabled = en;
		}

		public void SetEnabledTeleop(bool en)
		{
			GrayToteBtn.IsEnabled = en;
			RecycledLitterBtn.IsEnabled = en;
			CoopertitionBtn.IsEnabled = en;
			CoopertitionStackedBtn.IsEnabled = en;
			LandfillLitterBtn.IsEnabled = en;
			RecyclingBtn.IsEnabled = en;
			UnprocessedLitterDownBtn.IsEnabled = en;
			UnprocessedLitterUpBtn.IsEnabled = en;
			UnprocessedLitterCount.IsEnabled = en;
		}

		private void UpdateInfoTooltip()
		{
			InfoDescriptionTxt.Text = Record.TeamDescription;
			InfoExpectationsTxt.Text = Record.TeamExpectations;

			if (Record.TeamDescription.Trim() != "")
			{
				InfoDescriptionTxt.Visibility = System.Windows.Visibility.Visible;
				InfoDescriptionTxtHeader.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				InfoDescriptionTxt.Visibility = System.Windows.Visibility.Collapsed;
				InfoDescriptionTxtHeader.Visibility = System.Windows.Visibility.Collapsed;
			}

			if (Record.TeamExpectations.Trim() != "")
			{
				InfoExpectationsTxt.Visibility = System.Windows.Visibility.Visible;
				InfoExpectationsTxtHeader.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				InfoExpectationsTxt.Visibility = System.Windows.Visibility.Collapsed;
				InfoExpectationsTxtHeader.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void RunSetup(bool cantTakeNoForAnAnswer)
		{
			IsSettingUp = true;

			MatchSetupWindow window = new MatchSetupWindow(Frc, Record != null ? Record.MatchNumber : 0,
				cantTakeNoForAnAnswer);
			bool? result = window.ShowDialog();

			if (cantTakeNoForAnAnswer && result == false)
			{
				Application.Current.Shutdown();
			}
			else if (result == true) // nullable bool
			{
				LoadMatchSettings settings = window.Settings;

				Record = settings.MakeRecord();
				GoalsList.Items.Clear();
				PregameMatch = settings.GetMatch();
				Color = AllianceColor.Red;

				TeamLineup = TeamDataContext.CreateDataList(PregameMatch);
				TeamsDropDown.Items.Clear();
				foreach (TeamDataContext tdc in TeamLineup)
				{
					TeamsDropDown.Items.Add(tdc);
				}
				TeamsDropDown.SelectedIndex = 0;

				TimeBtn.IsEnabled = true;
			}

			IsSettingUp = false;
		}

		public void AddGoal(Goal g)
		{
			Record.AddGoal(g);

			ListBoxItem item = new ListBoxItem();
			item.ToolTip = g.UITooltip;
			item.Content = g.UIForm;
			item.Tag = g; // reference back to goal

			GoalsList.Items.Add(item);
		}

		public void AddAllUnprocessedLitter()
		{
			int count = 0;
			bool worked = int.TryParse(UnprocessedLitterCount.Text, out count);

			if (!worked)
			{
				Util.DebugLog(LogLevel.Error, "Unprocessed litter count cannot be parsed.");
				return;
			}

			for (int i = 0; i < count; i++)
			{
				AddGoal(Goal.MakeUnprocessedLitter(Color));
			}
		}

		public void PauseTimer()
		{
			DispTimer.Stop();
			Watch.Stop();

			StopBtn.IsEnabled = true;
		}
		public void StartTimer()
		{
			DispTimer.Start();
			Watch.Start();

			StopBtn.IsEnabled = false;
		}

		public void PregameReset()
		{
			GoalsList.Items.Clear();
			TimeBtn.IsEnabled = true;
			RobotSetToggle.IsChecked = false;
			ToteSetToggle.IsChecked = false;
			ContainerSetToggle.IsChecked = false;
			Time = TimeSpan.Zero;
			Watch.Reset();
			TimeBar.Value = 0;
			UnprocessedLitterCount.Text = "0";
			SaveBtn.IsEnabled = false;
			TimeBar.Foreground = new SolidColorBrush(AUTO_COLOR);
		}

		#region Event Handlers

		private void timer_Tick(object sender, EventArgs e)
		{
			Time = TimeSpan.FromMilliseconds(Watch.ElapsedMilliseconds);

			TimeBtn.Content = string.Format(@"{0:m\:ss\.f}", Time);
			TimeBar.Value = Time.TotalSeconds;

			if (Time < Util.TELEOP && (string)TimeBar.Tag != "AUTO")
			{
				TimeBar.Foreground = new SolidColorBrush(AUTO_COLOR);
				TimeBar.Tag = "AUTO";
			}
			else if (Time >= Util.TELEOP && Time < Util.TRASH_TOSS_STOP &&
				(string)TimeBar.Tag != "TELEOP")
			{
				TimeBar.Foreground = new SolidColorBrush(TELEOP_COLOR);
				TimeBar.Tag = "TELEOP";

				if (ScoutingAppSettings.PauseOnTeleop)
				{
					PauseTimer();
				}
				else
				{
					DoTeleopStuff();
				}
			}
			else if (Time >= Util.TRASH_TOSS_STOP && (string)TimeBar.Tag != "CHAOS")
			{
				TimeBar.Foreground = new SolidColorBrush(CHAOS_COLOR);
				TimeBar.Tag = "CHAOS";
			}
			else if (Time >= Util.MATCH_LENGTH)
			{
				TimeBar.Foreground = new SolidColorBrush(Colors.Gray);
				PauseTimer();
				EndGame();
			}
		}

		#region Goals and Stuff
		private void UnprocessedLitterDownBtn_Click(object sender, RoutedEventArgs e)
		{
			int count = int.Parse(UnprocessedLitterCount.Text);
			count = Math.Max(0, count - 1);
			UnprocessedLitterCount.Text = count.ToString();
		}

		private void UnprocessedLitterUpBtn_Click(object sender, RoutedEventArgs e)
		{
			int count = int.Parse(UnprocessedLitterCount.Text);
			count++;
			UnprocessedLitterCount.Text = count.ToString();
		}

		private void DefenseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!IsLoaded)
			{
				return;
			}

			int val = (int)e.NewValue;
			DefenseLbl.Content = DEFENSE_PREFIX + val.ToString();
			Record.Defense = val;
		}

		private void ToteSetStackedToggle_Checked(object sender, RoutedEventArgs e)
		{
			ToteSetToggle.IsChecked = true;
		}

		private void RecyclingLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!IsLoaded)
			{
				return;
			}

			int val = (int)e.NewValue;
			RecyclingLevelLbl.Content = LEVEL_PREFIX + val.ToString();
		}

		private void ToteSetToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			ToteSetStackedToggle.IsChecked = false;
		}

		private void GrayToteBtn_Click(object sender, RoutedEventArgs e)
		{
			AddGoal(Goal.MakeGrayTote(SelectedTeam, Time.CountedSeconds(), Color));
		}

		private void RecycledLitterBtn_Click(object sender, RoutedEventArgs e)
		{
			AddGoal(Goal.MakeRecycledLitter(SelectedTeam, Time.CountedSeconds(), Color));
		}

		private void CoopertitionStackedBtn_Click(object sender, RoutedEventArgs e)
		{
			AddGoal(Goal.MakeCoopertition(true, Time.CountedSeconds()));

			CoopertitionBtn.IsEnabled = false;
			CoopertitionStackedBtn.IsEnabled = false;
		}

		private void CoopertitionBtn_Click(object sender, RoutedEventArgs e)
		{
			if (Record.ScoredGoals.Exists((g) => g.Type == GoalType.Coopertition))
			{
				return;
			}

			AddGoal(Goal.MakeCoopertition(false, Time.CountedSeconds()));

			CoopertitionBtn.IsEnabled = false;
			CoopertitionStackedBtn.IsEnabled = false;
		}

		private void LandfillLitterBtn_Click(object sender, RoutedEventArgs e)
		{
			AddGoal(Goal.MakeLandfillLitter(Color));
		}

		private void RecyclingBtn_Click(object sender, RoutedEventArgs e)
		{
			AddGoal(Goal.MakeContainerTeleop((int)RecyclingLevelSlider.Value, SelectedTeam,
				Time.CountedSeconds(), Color));
		}

		#endregion

		private void ScoutingMainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			DispTimer.Interval = UPDATE_INTERVAL;
			DispTimer.Tick += timer_Tick;

			ScoutingJson.Initialize(false);

			RunSetup(true);
		}

		private void MatchSetupBtn_Click(object sender, RoutedEventArgs e)
		{
			RunSetup(false);
		}
		private void TimeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (DispTimer.IsEnabled)
			{
				PauseTimer();
			}
			else
			{
				if (Time.CountedSeconds() == Util.TELEOP.CountedSeconds()) // on post-auto resume
				{
					SetEnabledAuto(false);
					SetEnabledTeleop(true);

					AutoTeleopTabs.SelectedIndex = 1;

					DoTeleopStuff();
				}

				if (!GameInProgress) // start
				{
					GameInProgress = true;
					SetEnabledAuto(true);
					PenaltyBtn.IsEnabled = true;
				}

				StartTimer();
			}
		}

		private void TeamsDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsSettingUp)
			{
				return;
			}

			int index = TeamsDropDown.SelectedIndex;

			SelectedTeam = PregameMatch.GetTeamByInclusiveIndex(index);
			Color = PregameMatch.GetTeamColor(SelectedTeam);

			Record = new RecordedMatch(Record.MatchNumber, SelectedTeam, Color);

			if (PregameMatch.GetTeamColor(SelectedTeam) == AllianceColor.Red)
			{
				TeamInfoBtn.Background = new SolidColorBrush(RED_ALLIANCE);
			}
			else // blue (or indeterminate, but that'll never happen)
			{
				TeamInfoBtn.Background = new SolidColorBrush(BLUE_ALLIANCE);
			}

			PregameReset();

			UpdateInfoTooltip();
		}

		private void IsWorkingToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			if (Record == null)
			{
				return;
			}

			IsWorkingToggle.Content = "Malfunctioning";
			IsWorkingToggle.ToolTip = "Robot is malfunctioning as of this match";
			IsWorkingToggle.Background = new SolidColorBrush(MALFUNCTIONING_RED);
			Record.Working = false;
		}

		private void IsWorkingToggle_Checked(object sender, RoutedEventArgs e)
		{
			if (Record == null)
			{
				return;
			}

			IsWorkingToggle.Content = "Working";
			IsWorkingToggle.ToolTip = "Robot is working as expected";
			IsWorkingToggle.Background = new SolidColorBrush(WORKING_GREEN);
			Record.Working = true;
		}

		private void RemoveGoalBtn_Click(object sender, RoutedEventArgs e)
		{
			ListBoxItem lbi = GoalsList.SelectedItem as ListBoxItem;
			if (lbi == null)
			{
				return;
			}

			Goal g = lbi.Tag as Goal;
			if (g == null)
			{
				Penalty p = lbi.Tag as Penalty;
				if (p != null)
				{
					Record.AlliancePenalties.Remove(p);
					GoalsList.Items.Remove(lbi);
				}

				return;
			}

			if (g.Type == GoalType.Coopertition)
			{
				CoopertitionBtn.IsEnabled = true;
				CoopertitionStackedBtn.IsEnabled = true;
			}

			Record.ScoredGoals.Remove(g);
			GoalsList.Items.Remove(lbi);
		}

		private void GoalsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RemoveGoalBtn.IsEnabled = IsGoalSelected;
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			SaveOverlay saveOverlay = new SaveOverlay(Record, TeamsDropDown.SelectedIndex);
			saveOverlay.Owner = System.Windows.Window.GetWindow(this);
			bool? result = saveOverlay.ShowDialog();

			if (result == true) // Nullable<bool>
			{
				GoalsList.Items.Clear();
				PregameReset();
			}
		}

		private void StopBtn_Click(object sender, RoutedEventArgs e)
		{
			EndGame();
		}

		private void PenaltyBtn_Click(object sender, RoutedEventArgs e)
		{
			int clickedTime = Time.CountedSeconds();

			PenaltyOverlay penOverlay = new PenaltyOverlay();
			penOverlay.Owner = System.Windows.Window.GetWindow(this);
			bool? result = penOverlay.ShowDialog();

			if (result == true) // Nullable<bool>
			{
				Penalty p = new Penalty(clickedTime, penOverlay.Reasoning, 
					Color, SelectedTeam);

				Record.AlliancePenalties.Add(p);

				ListBoxItem lbi = new ListBoxItem();
				lbi.Content = p.UIForm;
				lbi.Foreground = new SolidColorBrush(MALFUNCTIONING_RED);
				lbi.FontWeight = FontWeights.SemiBold;
				lbi.ToolTip = "Penalty: " + p.Reasoning;
				lbi.Tag = p;

				GoalsList.Items.Add(lbi);
			}
		}

		private void TeamInfoBtn_Click(object sender, RoutedEventArgs e)
		{
			InfoOverlay info = new InfoOverlay(Record);
			info.Owner = System.Windows.Window.GetWindow(this);
			info.OKBtn.Click += InfoOverlay_OKBtn_Click;

			info.Show();
		}

		private void InfoOverlay_OKBtn_Click(object sender, RoutedEventArgs e)
		{
			Button okBtn = sender as Button;
			InfoOverlay info = System.Windows.Window.GetWindow(okBtn) as InfoOverlay;

			Record.TeamDescription = info.DescriptionBox.Text;
			Record.TeamExpectations = info.ExpectationsBox.Text;

			UpdateInfoTooltip();
		}

		private void ConfigBtn_Click(object sender, RoutedEventArgs e)
		{
			ConfigOverlay cfg = new ConfigOverlay();
			cfg.ShowDialog();
		}

		private void TabItem_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up)
			{
				RecyclingLevelSlider.Value = Math.Min(RecyclingLevelSlider.Value + 1, 
					RecyclingLevelSlider.Maximum);
				e.Handled = true;
			}
			if (e.Key == Key.Down)
			{
				RecyclingLevelSlider.Value = Math.Max(RecyclingLevelSlider.Value - 1, 
					RecyclingLevelSlider.Minimum);
			}
		}

		#endregion
	}

	public class TeamDataContext
	{
		public Team Team
		{ get; set; }

		public Brush UIBrush
		{
			get
			{
				switch (Color)
				{
				case AllianceColor.Indeterminate:
					return new SolidColorBrush(Colors.White);
				case AllianceColor.Red:
					return new SolidColorBrush(Colors.Red);
				case AllianceColor.Blue:
					return new SolidColorBrush(Colors.Blue);
				default:
					return null;
				}
			}
		}

		public AllianceColor Color
		{ get; set; }

		public TeamDataContext(Team t, AllianceColor col)
		{
			Team = t;
			Color = col;
		}

		public object ToUIElement()
		{
			ListItem res = new ListItem();

			TextBlock txt = new TextBlock();
			txt.Text = Team.ToString();
			res.FontWeight = FontWeights.Bold;
			res.Foreground = UIBrush;

			return res;
		}

		public override string ToString()
		{
			return "[" + Color.ToString() + "] " + Team.ToString();
		}

		public static List<TeamDataContext> CreateDataList(Match m)
		{
			List<TeamDataContext> res = new List<TeamDataContext>();

			res.Add(new TeamDataContext(m.RedAlliance.A, AllianceColor.Red));
			res.Add(new TeamDataContext(m.RedAlliance.B, AllianceColor.Red));
			res.Add(new TeamDataContext(m.RedAlliance.C, AllianceColor.Red));

			res.Add(new TeamDataContext(m.BlueAlliance.A, AllianceColor.Blue));
			res.Add(new TeamDataContext(m.BlueAlliance.B, AllianceColor.Blue));
			res.Add(new TeamDataContext(m.BlueAlliance.C, AllianceColor.Blue));

			return res;
		}
	}
}
