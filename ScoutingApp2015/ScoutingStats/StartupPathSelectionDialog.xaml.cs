using ScoutingData;
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
using System.Windows.Shapes;

namespace ScoutingStats
{
	/// <summary>
	/// Interaction logic for StartupPathSelectionDialog.xaml
	/// </summary>
	public partial class StartupPathSelectionDialog : Elysium.Controls.Window
	{
		public string EventPath
		{
			get
			{
				return EventPathBox.Text;
			}
		}

		public string TeamsPath
		{
			get
			{
				return TeamsPathBox.Text;
			}
		}

		public StartupPathSelectionDialog()
		{
			InitializeComponent();
		}

		public void AnyPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValidatePaths();
		}

		public void ValidatePaths()
		{
			bool ok = true;
			if (File.Exists(EventPathBox.Text))
			{
				EventPathBox.Foreground = new SolidColorBrush(Colors.Black);
				EventPathBox.ToolTip = EventPathBox.Text;
			}
			else
			{
				EventPathBox.Foreground = new SolidColorBrush(Colors.Red);
				EventPathBox.ToolTip = "File does not exist.";
				ok = false;
			}

			if (File.Exists(TeamsPathBox.Text))
			{
				TeamsPathBox.Foreground = new SolidColorBrush(Colors.Black);
				TeamsPathBox.ToolTip = TeamsPathBox.Text;
			}
			else
			{
				TeamsPathBox.Foreground = new SolidColorBrush(Colors.Red);
				TeamsPathBox.ToolTip = "File does not exist.";
				ok = false;
			}

			OKBtn.IsEnabled = ok;
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

		private void TeamsPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new Microsoft.Win32.OpenFileDialog();

			ofd.DefaultExt = ScoutingJson.AnalysisExtension;
			ofd.InitialDirectory = ScoutingJson.LocalPath;
			ofd.Title = "Select a Teams File";
			ofd.Multiselect = false;
			ofd.Filter = "Teams Files (*.teams)|*.teams";

			bool? result = ofd.ShowDialog();

			if (result == true) // Nullable<bool>
			{
				TeamsPathBox.Text = ofd.FileName;
			}
		}

		private void EventPathBtn_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new Microsoft.Win32.OpenFileDialog();

			ofd.DefaultExt = ScoutingJson.AnalysisExtension;
			ofd.InitialDirectory = ScoutingJson.LocalPath;
			ofd.Title = "Select an Event File";
			ofd.Multiselect = false;
			ofd.Filter = "FRC Event Files (*.frc)|*.frc";

			bool? result = ofd.ShowDialog();

			if (result == true) // Nullable<bool>
			{
				EventPathBox.Text = ofd.FileName;
			}
		}
	}
}
