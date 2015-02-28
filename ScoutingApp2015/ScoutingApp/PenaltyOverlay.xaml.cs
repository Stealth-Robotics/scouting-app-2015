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

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for PenaltyOverlay.xaml
	/// </summary>
	public partial class PenaltyOverlay : Elysium.Controls.Window
	{
		public string Reasoning
		{
			get
			{
				return ReasonBox.Text;
			}
		}

		public PenaltyOverlay()
		{
			InitializeComponent();
		}

		private void ReasonBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CancelBtn.IsEnabled = (ReasonBox.Text.Trim() != "");
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
	}
}
