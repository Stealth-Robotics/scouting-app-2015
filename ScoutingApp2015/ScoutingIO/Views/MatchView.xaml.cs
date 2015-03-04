using ScoutingData.Data;
using ScoutingIO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScoutingIO.Views
{
	/// <summary>
	/// Interaction logic for MatchView.xaml
	/// </summary>
	public partial class MatchView : UserControl
	{
		public MatchViewModel ViewModel
		{
			get
			{
				return Resources["ViewModel"] as MatchViewModel;
			}
		}

		public MatchView()
		{
			InitializeComponent();
		}

		public void SendData(object sender, EventArgs<EventViewModel> e)
		{
			ViewModel.SendData(sender, e);
		}

		public void SendInitData()
		{
			ViewModel.DoInit();
		}

		private void MatchesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				if (ViewModel.EventVM.Event == null)
				{
					return;
				}

				DataGridCellInfo cell = MatchesDataGrid.SelectedCells.LastOrDefault();
				if (cell.Column.DisplayIndex == MatchesDataGrid.Columns.Count - 1 &&
					MatchesDataGrid.Items.IndexOf(cell.Item) == MatchesDataGrid.Items.Count - 1)
				{
					e.Handled = true;

					Match m = new Match();
					ViewModel.EventVM.Event.Matches.Add(m);
					ViewModel.RefreshDatagrid();
					//MatchesDataGrid.SelectedItem = m;
					MatchesDataGrid.SelectedIndex = ViewModel.EventVM.Event.Matches.Count - 1;

					MatchesDataGrid.UpdateLayout();
				}
			}
		}
	}
}
