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

namespace ScoutingIO.Dialogs
{
	/// <summary>
	/// Interaction logic for NewEventDlg.xaml
	/// </summary>
	public partial class NewEventDlg : Elysium.Controls.Window
	{
		string loadedPath;
		bool stillLoading = true;

		public string Path
		{
			get
			{
				return PathBox.Text;
			}
			set
			{
				PathBox.Text = value;
				PathBox.ToolTip = value;
			}
		}

		public string EventName
		{
			get
			{
				return NameBox.Text;
			}
			set
			{
				NameBox.Text = value;
			}
		}

		public NewEventDlg(string path)
		{
			InitializeComponent();
			loadedPath = path;
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

		private void PathBtn_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
			sfd.DefaultExt = ScoutingJson.EventExtension;
			sfd.InitialDirectory = ScoutingJson.LocalPath;
			sfd.OverwritePrompt = true;
			sfd.Title = "Choose Save Location";
			sfd.Filter = "FRC Event Files (*.frc)|*.frc";
			sfd.AddExtension = true;

			bool? res = sfd.ShowDialog();
			if (res == true) // Nullable<bool>
			{
				Path = sfd.FileName;
			}
		}

		private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (stillLoading)
			{
				return;
			}

			EventName = Util.GetFileName(Path, false);
		}

		private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (stillLoading)
			{
				return;
			}

			Path = Util.GetFolderPath(Path) + EventName + ScoutingJson.EventExtension;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			stillLoading = false;
			Path = loadedPath;
		}
	}
}
