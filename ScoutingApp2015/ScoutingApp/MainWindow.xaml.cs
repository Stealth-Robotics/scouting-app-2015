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
		public const string DEFENSE_PREFIX = "Defense: ";
		public const string LEVEL_PREFIX = "Level: ";

		public static readonly TimeSpan UPDATE_INTERVAL =
			TimeSpan.FromMilliseconds(50.0);

		public static readonly Color RED_ALLIANCE = Util.MakeColor("FFFF2222");
		public static readonly Color BLUE_ALLIANCE = Util.MakeColor("FF2E71FF");

		public static readonly Color WORKING_GREEN = Util.MakeColor("FF40FF40");
		public static readonly Color MALFUNCTIONING_RED = Util.MakeColor("FF880000");

		public DispatcherTimer timer;
		public Stopwatch watch;

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

			timer = new DispatcherTimer();
			watch = new Stopwatch();

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

			if (ScoutingAppSettings.PauseOnTeleop)
			{
				timer.Stop();
				watch.Stop();
			}
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

		private void RunSetup(bool cantTakeNoForAnAnswer)
		{
			SettingsWindow window = new SettingsWindow(Frc, Record != null ? Record.MatchNumber : 0,
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
				PregameMatch = settings.GetMatch();
				Color = AllianceColor.Red;

				TeamLineup = TeamDataContext.CreateDataList(PregameMatch);
				TeamsDropDown.Items.Clear();
				foreach (TeamDataContext tdc in TeamLineup)
				{
					TeamsDropDown.Items.Add(tdc);
				}
				TeamsDropDown.SelectedIndex = 0;
			}
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

		////////////////////
		// EVENT HANDLERS //
		////////////////////

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

		private void MatchSetupBtn_Click(object sender, RoutedEventArgs e)
		{
			RunSetup(false);
		}

		private void ToteSetToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			ToteSetStackedToggle.IsChecked = false;
		}

		private void ScoutingMainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Interval = UPDATE_INTERVAL;
			timer.Tick += timer_Tick;

			ScoutingJson.Initialize(false);

			RunSetup(true);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			Time = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);

			TimeBtn.Content = string.Format(@"{0:m\:ss\.f}", Time);
			TimeBar.Value = Time.TotalSeconds;

			if (Time < Util.TELEOP && (string)TimeBar.Tag != "AUTO")
			{
				TimeBar.Foreground = new SolidColorBrush(Colors.Yellow);
				TimeBar.Tag = "AUTO";
			}
			else if (Time >= Util.TELEOP && Time < Util.TRASH_TOSS && 
				(string)TimeBar.Tag != "TELEOP")
			{
				TimeBar.Foreground = new SolidColorBrush(Colors.SteelBlue);
				TimeBar.Tag = "TELEOP";

				DoTeleopStuff();
			}
			else if (Time >= Util.TRASH_TOSS && (string)TimeBar.Tag != "CHAOS")
			{
				TimeBar.Foreground = new SolidColorBrush(Colors.Red);
				TimeBar.Tag = "CHAOS";
			}
			else if (Time >= Util.MATCH_LENGTH)
			{
				TimeBar.Foreground = new SolidColorBrush(Colors.Gray);
				GameInProgress = false;
			}
		}

		private void TimeBtn_Click(object sender, RoutedEventArgs e)
		{
			if (timer.IsEnabled)
			{
				timer.Stop();
				watch.Stop();
			}
			else
			{
				if (Time.CountedSeconds() == Util.TELEOP.CountedSeconds()) // on post-auto resume
				{
					SetEnabledAuto(false);
					SetEnabledTeleop(true);

					AutoTeleopTabs.SelectedIndex = 1;
				}

				if (!GameInProgress) // start
				{
					GameInProgress = true;
					SetEnabledAuto(true);
				}

				timer.Start();
				watch.Start();
			}
		}

		private void TeamsDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
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
		}

		private void GrayToteBtn_Click(object sender, RoutedEventArgs e)
		{
			AddGoal(Goal.MakeGrayTote(SelectedTeam, Time.CountedSeconds()));
		}

		private void RecycledLitterBtn_Click(object sender, RoutedEventArgs e)
		{
			AddGoal(Goal.MakeRecycledLitter(SelectedTeam, Time.CountedSeconds()));
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
			AddGoal(Goal.MakeRecycledLitter(SelectedTeam, Time.CountedSeconds()));
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
				return;
			}

			Record.ScoredGoals.Remove(g);
		}

		private void GoalsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RemoveGoalBtn.IsEnabled = IsGoalSelected;
		}
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
