using ScoutingData;
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

namespace ScoutingIO.Dialogs
{
	/// <summary>
	/// Interaction logic for NewEventDialog.xaml
	/// </summary>
	public partial class NewEventDialog : Elysium.Controls.Window
	{
		string _path;

		public string Path
		{
			get
			{
				return PathBox.Text;
			}
			set
			{
				PathBox.Text = value;
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

		public NewEventDialog(string path)
		{
			InitializeComponent();
			_path = path;
		}

		private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string filename = Util.GetFileName(PathBox.Text, false);
			EventName = filename;
		}

		private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string path = Util.GetFolderPath(PathBox.Text);
			Path = path + NameBox.Text + ScoutingJson.EventExtension;
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
			sfd.Filter = "FRC Event Files (*.frc)|*.frc";
			sfd.Title = "Choose event save location...";
			sfd.InitialDirectory = ScoutingJson.LocalPath;
			bool? res = sfd.ShowDialog(this);

			if (res == true) // Nullable<bool>
			{
				PathBox.Text = sfd.FileName;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Path = _path;
		}
	}
}
