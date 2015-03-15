using Ookii.Dialogs.Wpf;
using ScoutingData.Lite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoutingAppLite.View
{
	/// <summary>
	/// Interaction logic for ConfirmSaveDialog.xaml
	/// </summary>
	public partial class ConfirmSaveDialog : Elysium.Controls.Window
	{
		public string SelectedPath
		{
			get
			{
				return LocationPathBox.Text;
			}
		}

		public ConfirmSaveDialog()
		{
			InitializeComponent();
		}

		private void LocationPathBtn_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
			fbd.UseDescriptionForTitle = true;
			if (Directory.Exists(LocationPathBox.Text))
			{
				fbd.SelectedPath = LocationPathBox.Text;
			}
			fbd.Description = "Choose Save Path...";

			bool? result = fbd.ShowDialog();
			if (result == true)
			{
				LocationPathBox.Text = fbd.SelectedPath;
			}
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
