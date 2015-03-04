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
		public StartupPathSelectionDialog()
		{
			InitializeComponent();
		}

		public void EventPathBox_TextChanged(object sender, TextChangedEventArgs e)
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

			}
		}
	}
}
