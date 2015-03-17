using ScoutingAppLite.ViewModel;
using ScoutingData.Data;
using ScoutingData.Lite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScoutingAppLite.View
{
	/// <summary>
	/// Interaction logic for RatingView.xaml
	/// </summary>
	public partial class RatingView : UserControl
	{
		public event EventHandler<bool> IsRecordingChanged;

		public RatingViewModel ViewModel
		{
			get
			{
				return Resources["ViewModel"] as RatingViewModel;
			}
		}

		public Brush ThemeBrush
		{ get; set; }

		public bool IsRecording
		{
			get
			{
				return TitleCheck.IsChecked == true;
			}
		}

		public int TeamIndex
		{ get; set; }

		public RatingView()
		{
			InitializeComponent();
		}

		public void SetMatchContext(Match match)
		{
			if (match != null)
			{
				ViewModel.RatedTeam = match.GetTeamByInclusiveIndex(TeamIndex);
			}
			ViewModel.IndicatedMatch = match;
		}

		public RecordLite MakeRecord()
		{
			RecordLite res = new RecordLite(ViewModel.RatedTeam, 
				ViewModel.IndicatedMatch);
			res.Ratings = ViewModel.Ratings;

			return res;
		}

		private void TitleCheck_Click(object sender, RoutedEventArgs e)
		{
			if (IsRecordingChanged != null)
			{
				IsRecordingChanged(this, TitleCheck.IsChecked ?? false);
			}
		}
	}
}
