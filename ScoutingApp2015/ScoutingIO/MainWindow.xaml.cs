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

using ScoutingIO.ViewModel;
using ScoutingIO.Views;

namespace ScoutingIO
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		public MatchView MatchV
		{
			get
			{
				return MatchTab.Content as MatchView;
			}
		}

		public MainWindow()
		{
			ScoutingData.ScoutingJson.Initialize(false);

			InitializeComponent();
		}

		private void EventView_GoToMatches(object sender, RoutedEventArgs e)
		{
			MainTabControl.SelectedIndex = 1;
		}

		private void EventView_GoToTeams(object sender, RoutedEventArgs e)
		{
			MainTabControl.SelectedIndex = 2;
		}

		// Sort of a cheaty pipeline here
		private void EventView_SendMatchesData(object sender, EventArgs<EventViewModel> e)
		{
			MatchV.SendData(sender, e);
		}
	}
}
