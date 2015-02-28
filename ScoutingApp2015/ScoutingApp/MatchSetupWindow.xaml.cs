using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

using ScoutingData.Data;
using ScoutingData.Sync;
using ScoutingData;
using System.Windows.Interop;

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for MatchSetupWindow.xaml
	/// </summary>
	public partial class MatchSetupWindow : Elysium.Controls.Window
	{
		public LoadMatchSettings Settings
		{ get; private set; }

		public Match Pregame
		{ get; private set; }

		public bool CantTakeNo
		{ get; private set; }

		bool hasLoaded = false;

		int maxMatchID = -1;

		public int MatchID
		{
			get
			{
				return Settings.MatchID;
			}
			set
			{
				Settings.MatchID = value;

				if (!hasLoaded)
				{
					return;
				}

				Pregame = Settings.Frc.LoadMatch(value);
				
				int max = 0;
				foreach (Match m in Settings.Frc.Matches)
				{
					if (m.Number > max)
					{
						max = m.Number;
					}
				}
				maxMatchID = max;

				MatchIDBox.Text = value.ToString();
				if (Pregame.Pregame)
				{
					MatchIDBox.Foreground = new SolidColorBrush(Colors.Black);
					MatchIDBox.ToolTip = null;
				}
				else
				{
					MatchIDBox.Foreground = new SolidColorBrush(Colors.Red);
					MatchIDBox.ToolTip = "Match is already complete.";
				}
			}
		}

		#region preview bindings

		Team RedA
		{
			get
			{
				return Pregame.RedAlliance.A;
			}
		}
		StackPanel InfoRedA
		{
			get
			{
				return RedA.GetDescriptionWPF();
			}
		}

		Team RedB
		{
			get
			{
				return Pregame.RedAlliance.B;
			}
		}
		StackPanel InfoRedB
		{
			get
			{
				return RedB.GetDescriptionWPF();
			}
		}

		Team RedC
		{
			get
			{
				return Pregame.RedAlliance.C;
			}
		}
		StackPanel InfoRedC
		{
			get
			{
				return RedC.GetDescriptionWPF();
			}
		}

		Team BlueA
		{
			get
			{
				return Pregame.BlueAlliance.A;
			}
		}
		StackPanel InfoBlueA
		{
			get
			{ 
				return BlueA.GetDescriptionWPF();
			}
		}

		Team BlueB
		{
			get
			{
				return Pregame.BlueAlliance.B;
			}
		}
		StackPanel InfoBlueB
		{
			get
			{
				return BlueB.GetDescriptionWPF();
			}
		}

		Team BlueC
		{
			get
			{
				return Pregame.BlueAlliance.C;
			}
		}
		StackPanel InfoBlueC
		{
			get
			{
				return BlueC.GetDescriptionWPF();
			}
		}

		#endregion

		public MatchSetupWindow(FrcEvent frc, int lastMatchID, 
			bool cantTakeNoForAnAnswer) : base()
		{
			Settings = new LoadMatchSettings();
			Settings.Frc = frc;
			if (frc == null)
			{
				Settings.MatchID = 1;
			}
			else
			{
				Settings.MatchID = Math.Min(lastMatchID + 1, frc.Matches.Count - 1);
				Pregame = frc.LoadMatch(Settings.MatchID);
			}

			CantTakeNo = cantTakeNoForAnAnswer;

			InitializeComponent();
		}

		public void UpdateTeamPreviews()
		{
			if (Settings.Frc == null)
			{
				return;
			}

			RedATxt.Text = RedA.ToString();
			RedBTxt.Text = RedB.ToString();
			RedCTxt.Text = RedC.ToString();

			BlueATxt.Text = BlueA.ToString();
			BlueBTxt.Text = BlueB.ToString();
			BlueCTxt.Text = BlueC.ToString();

			RedATxt.ToolTip = InfoRedA;
			RedBTxt.ToolTip = InfoRedB;
			RedCTxt.ToolTip = InfoRedC;

			BlueATxt.ToolTip = InfoBlueA;
			BlueBTxt.ToolTip = InfoBlueB;
			BlueCTxt.ToolTip = InfoBlueC;
		}

		public void DoContextualLoading()
		{
			if (CantTakeNo)
			{
				OKBtn.IsEnabled = File.Exists(TeamsPathBox.Text) &&
					File.Exists(EventPathBox.Text);
			}

			bool matchIDReady = true;
			if (File.Exists(EventPathBox.Text))
			{
				FrcEvent frc = ScoutingJson.ParseFrcEvent(EventPathBox.Text);

				if (frc.IsCorrectlyLoaded())
				{
					Settings.Frc = frc;

					EventPathBox.Foreground = new SolidColorBrush(Colors.Black);
					EventPathBox.ToolTip = null;
				}
				else
				{
					EventPathBox.Foreground = new SolidColorBrush(Colors.Red);
					EventPathBox.ToolTip = "File is invalid or corrupted.";
					matchIDReady = false;
				}
			}
			else
			{
				EventPathBox.Foreground = new SolidColorBrush(Colors.Red);
				EventPathBox.ToolTip = "File does not exist.";
				matchIDReady = false;
			}

			if (Settings.Frc != null && File.Exists(TeamsPathBox.Text))
			{
				TeamsList teams = ScoutingJson.ParseTeamsList(TeamsPathBox.Text);

				if (teams != null && teams.IsCorrectlyLoaded())
				{
					Settings.Frc.PostJsonLoading(teams);

					MatchID = 1;

					TeamsPathBox.Foreground = new SolidColorBrush(Colors.Black);
					TeamsPathBox.ToolTip = null;
				}
				else
				{
					TeamsPathBox.Foreground = new SolidColorBrush(Colors.Red);
					TeamsPathBox.ToolTip = "File is invalid or corrupted.";
					matchIDReady = false;
				}
			}
			else
			{
				TeamsPathBox.Foreground = new SolidColorBrush(Colors.Red);
				TeamsPathBox.ToolTip = "File does not exist.";
				matchIDReady = false;
			}

			MatchIDDownBtn.IsEnabled = matchIDReady;
			MatchIDUpBtn.IsEnabled = matchIDReady;

			UpdateTeamPreviews();
		}

		private void MatchIDDownBtn_Click(object sender, RoutedEventArgs e)
		{
			MatchID = Math.Max(MatchID - 1, 1);
		}

		private void MatchIDUpBtn_Click(object sender, RoutedEventArgs e)
		{
			MatchID = Math.Min(MatchID + 1, Settings.Frc.Matches.Count);
		}

		private void MatchIDBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int val = -1;
			bool canParse = int.TryParse(MatchIDBox.Text, out val);

			if (!canParse || val <= 0 || val > maxMatchID)
			{
				MatchIDBox.Text = MatchID.ToString();
			}
			else
			{
				MatchID = val;
			}
		}

		private void OKBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void EventPathBtn_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.DefaultExt = ScoutingJson.EventExtension;
			ofd.Title = "Open Event File";
			ofd.InitialDirectory = ScoutingJson.LocalPath;
			ofd.Filter = "FRC Event Files (*.frc)|*.frc";

			bool? result = ofd.ShowDialog(this);
			if (result == true)
			{
				EventPathBox.Text = ofd.FileName;
			}
		}

		private void TeamsPathBtn_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.DefaultExt = ScoutingJson.EventExtension;
			ofd.Title = "Open Teams List";
			ofd.InitialDirectory = ScoutingJson.LocalPath;
			ofd.Filter = "Team List Files (*.teams)|*.teams";

			bool? result = ofd.ShowDialog(this);
			if (result == true)
			{
				TeamsPathBox.Text = ofd.FileName;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (CantTakeNo)
			{
				CancelBtn.IsEnabled = false;
				OKBtn.IsEnabled = false;
			}

			hasLoaded = true;

			DoContextualLoading();
			UpdateTeamPreviews();
		}

		private void PathBoxesChanged(object sender, TextChangedEventArgs e)
		{
			if (!hasLoaded)
			{
				return;
			}

			DoContextualLoading();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (CantTakeNo && DialogResult != true)
			{
				Application.Current.Shutdown();
				DialogResult = false;
			}
		}
	}

	public class LoadMatchSettings
	{
		public FrcEvent Frc
		{ get; set; }

		public int MatchID
		{ get; set; }

		public RecordedMatch MakeRecord()
		{
			return new RecordedMatch(MatchID, GetMatch().RedAlliance.A, AllianceColor.Red);
		}

		public AllianceColor GetColorForTeamID(int teamID)
		{
			return GetMatch().GetTeamColor(Frc.LoadTeam(teamID));
		}

		public Match GetMatch()
		{
			return Frc.LoadMatch(MatchID);
		}
	}
}
