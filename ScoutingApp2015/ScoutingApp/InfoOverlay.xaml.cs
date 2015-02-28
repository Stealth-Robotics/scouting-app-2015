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

using ScoutingData.Sync;

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for InfoOverlay.xaml
	/// </summary>
	public partial class InfoOverlay : Elysium.Controls.Window
	{
		public InfoOverlay(RecordedMatch record)
		{
			InitializeComponent();

			DescriptionBox.Text = record.TeamDescription;
			ExpectationsBox.Text = record.TeamExpectations;
		}

		private void OKBtn_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
