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

using ScoutingData;

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for SettingsOverlay.xaml
	/// </summary>
	public partial class ConfigOverlay : Elysium.Controls.Window
	{
		public ConfigOverlay()
		{
			InitializeComponent();

			AppSettings.Initialize();
			LoadFromSettings();
		}

		public void LoadFromSettings()
		{
			PauseOnTeleopCheck.IsChecked = AppSettings.PauseOnTeleop;
		}

		private void PauseOnTeleopCheck_Click(object sender, RoutedEventArgs e)
		{
			AppSettings.PauseOnTeleop = PauseOnTeleopCheck.IsChecked ?? false;
		}

		private void OKBtn_Click(object sender, RoutedEventArgs e)
		{
			AppSettings.Save();
			Close();
		}

		private void DefaultEventPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			AppSettings.EventPath = DefaultEventPathBox.Text;
		}

		private void DefaultTeamsPathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			AppSettings.TeamsPath = DefaultTeamsPathBox.Text;
		}
	}
}
