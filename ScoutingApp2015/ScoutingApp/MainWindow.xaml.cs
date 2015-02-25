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

		public bool IsGoalSelected
		{
			get
			{
				return GoalsList.SelectedIndex != -1;
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			timer = new DispatcherTimer();
			watch = new Stopwatch();
		}

		public void InitLoadFiles()
		{
			ScoutingJson.Initialize(false);
			Frc = ScoutingJson.ParseFrcEvent("Default.json", false);
		}

		public void DoTeleopStuff()
		{
			if (RobotSetToggle.IsChecked == true) // Nullable<bool>
			{
				Record.AddGoal(Goal.MakeRobotSet(Color));
			}
			if (ContainerSetToggle.IsChecked == true)
			{
				Record.AddGoal(Goal.MakeContainerSet(Color));
			}
			if (ToteSetToggle.IsChecked == true)
			{
				Record.AddGoal(Goal.MakeYellowToteSet(
					ToteSetStackedToggle.IsChecked == true, Color));
			}

			SetEnabledAuto(false);
			SetEnabledTeleop(true);
		}

		public void SetEnabledAuto(bool en)
		{
			RobotSetToggle.IsEnabled = en;
			ContainerSetToggle.IsEnabled = en;
			ToteSetToggle.IsEnabled = en;
			ToteSetStackedToggle.IsEnabled = en;

			timer.Stop();
			watch.Stop();

			AutoTeleopTabs.SelectedIndex = 1; // Teleop's tab
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
			SettingsWindow window = new SettingsWindow(Frc, Record != null ? Record.MatchNumber : 0);
			bool? result = window.ShowDialog();

			if (result == true) // nullable bool
			{
				ScoutingAppSettings settings = window.Settings;

				Record = settings.MakeRecord();
				PregameMatch = settings.GetMatch();
				Color = AllianceColor.Red;

				TeamsDropDown.SelectedIndex = 0;

				GoalsList.ItemsSource = Record.ScoredGoals;
			}
		}

		private void ToteSetToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			ToteSetStackedToggle.IsChecked = false;
		}

		private void ScoutingMainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Interval = UPDATE_INTERVAL;
			timer.Tick += timer_Tick;

			InitLoadFiles();
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
				timer.Start();
				watch.Start();
			}
		}

		private void TeamsDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object selectedObj = TeamsDropDown.SelectedItem;
			Team selectedTeam = selectedObj as Team;

			if (selectedTeam == null)
			{
				// something
				return;
			}

			SelectedTeam = selectedTeam;

			Color = PregameMatch.GetTeamColor(SelectedTeam);
		}
	}
}
