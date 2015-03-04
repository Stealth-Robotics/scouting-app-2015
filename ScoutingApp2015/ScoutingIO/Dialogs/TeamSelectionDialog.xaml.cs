using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ScoutingIO.Dialogs
{
	/// <summary>
	/// Interaction logic for TeamSelectionDialog.xaml
	/// </summary>
	public partial class TeamSelectionDialog : Elysium.Controls.Window
	{
		public TeamsList AllTeams
		{ get; private set; }

		public Team SelectedTeam
		{
			get
			{
				return TeamsListBox.SelectedItem as Team;
			}
		}

		public TeamSelectionDialog(TeamsList list)
		{
			AllTeams = list;
			InitializeComponent();
		}

		private void TeamsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OKBtn.IsEnabled = SelectedTeam != null;
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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			OKBtn.IsEnabled = SelectedTeam != null;

			foreach (Team t in AllTeams)
			{
				TeamsListBox.Items.Add(t);
			}
		}

		private void BreakBtn_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debugger.Break();
		}
	}
}
