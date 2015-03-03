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
	/// Interaction logic for TeamsView.xaml
	/// </summary>
	public partial class TeamsView : UserControl
	{
		bool isSending = false;

		public TeamsViewModel ViewModel
		{
			get
			{
				return Resources["ViewModel"] as TeamsViewModel;
			}
		}

		public event EventHandler<TeamsViewModel> SendData;

		public TeamsView()
		{
			InitializeComponent();
		}

		private void TeamsViewModel_SendData(object sender, EventArgs<TeamsViewModel> e)
		{
			if (SendData != null)
			{
				SendData(sender, e);
			}
		}

		public void SendInitData()
		{
			if (!isSending)
			{
				isSending = true;
				ViewModel.DoSendData();
				isSending = false;
			}
		}
	}
}
