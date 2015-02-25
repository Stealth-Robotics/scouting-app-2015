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

using ScoutingData.Data;
using ScoutingData.Sync;
using ScoutingData;

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Elysium.Controls.Window
	{
		public ScoutingAppSettings Settings
		{ get; private set; }

		public Match Pregame
		{ get; private set; }

		public int MatchID
		{
			get
			{
				return Settings.MatchID;
			}
			set
			{
				Settings.MatchID = value;
				Pregame = Settings.Frc.LoadMatch(value);

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

		public SettingsWindow(FrcEvent frc, int lastMatchID) : base()
		{
			InitializeComponent();

			Settings = new ScoutingAppSettings();
			Settings.Frc = frc;
			Settings.MatchID = Math.Min(lastMatchID + 1, frc.Matches.Count - 1);

			Pregame = frc.LoadMatch(Settings.MatchID);
		}

		private void MatchIDDownBtn_Click(object sender, RoutedEventArgs e)
		{
			MatchID = Math.Max(MatchID - 1, 0);
		}

		private void MatchIDUpBtn_Click(object sender, RoutedEventArgs e)
		{
			MatchID = MatchID + 1;
		}

		private void MatchIDBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int val = -1;
			bool canParse = int.TryParse(MatchIDBox.Text, out val);

			if (!canParse)
			{
				MatchIDBox.Text = MatchID.ToString();
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
			ofd.DefaultExt = ScoutingJson.Extension;
			ofd.Title = "Open Event File";
			ofd.Filter = "JSON Text Files (*.json)|*.json|Text Files (*.txt)|*.txt";

			bool? result = ofd.ShowDialog(this);
			if (result == true)
			{
				EventPathBox.Text = ofd.FileName;
			}
		}

		private void TeamsPathBtn_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
			ofd.DefaultExt = ScoutingJson.Extension;
			ofd.Title = "Open Teams List";
			ofd.Filter = "JSON Text Files (*.json)|*.json|Text Files (*.txt)|*.txt";

			bool? result = ofd.ShowDialog(this);
			if (result == true)
			{
				TeamsPathBox.Text = ofd.FileName;
			}
		}
	}

	public class ScoutingAppSettings
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
