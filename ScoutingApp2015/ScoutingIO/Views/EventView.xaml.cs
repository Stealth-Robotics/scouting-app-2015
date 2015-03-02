using ScoutingData;
using ScoutingData.Data;
using ScoutingIO.ViewModel;
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

namespace ScoutingIO.Views
{
	/// <summary>
	/// Interaction logic for EventView.xaml
	/// </summary>
	public partial class EventView : UserControl
	{
		public event RoutedEventHandler GoToMatches;
		public event RoutedEventHandler GoToTeams;
		public event EventHandler<EventViewModel> SendMatchesData;

		public EventViewModel ViewModel
		{
			get
			{
				return Resources["ViewModel"] as EventViewModel;
			}
		}

		public EventView()
		{
			InitializeComponent();
		}

		private void GoToMatchesBtn_Click(object sender, RoutedEventArgs e)
		{
			if (GoToMatches != null)
			{
				GoToMatches(this, e);
			}
		}

		private void GoToTeamsBtn_Click(object sender, RoutedEventArgs e)
		{
			if (GoToTeams != null)
			{
				GoToTeams(this, e);
			}
		}

		private void OnSendMatchesData(object sender, EventArgs<EventViewModel> e)
		{
			if (SendMatchesData != null)
			{
				SendMatchesData(sender, e);
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			OnSendMatchesData(this, new EventArgs<EventViewModel>(ViewModel));
		}

		public void SendTeamsData(object sender, EventArgs<TeamsViewModel> e)
		{
			if (ViewModel != null)
			{
				ViewModel.SendTeamsData(sender, e);
			}
		}

		public void SendInitData()
		{
			ViewModel.DoSendData();
		}
	}
}
