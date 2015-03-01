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
	/// Interaction logic for MatchView.xaml
	/// </summary>
	public partial class MatchView : UserControl
	{
		public MatchView()
		{
			InitializeComponent();
		}

		public void SendData(object sender, EventArgs<EventViewModel> e)
		{
			MatchViewModel vm = Resources["ViewModel"] as MatchViewModel;

			vm.SendData(sender, e);
		}
	}
}
