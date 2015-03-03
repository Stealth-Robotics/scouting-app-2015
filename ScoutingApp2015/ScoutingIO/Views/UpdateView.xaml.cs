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
	/// Interaction logic for UpdateView.xaml
	/// </summary>
	public partial class UpdateView : UserControl
	{
		public UpdateViewModel ViewModel
		{
			get
			{
				return Resources["ViewModel"] as UpdateViewModel;
			}
		}

		public UpdateView()
		{
			InitializeComponent();
		}

		public void SendData(object sender, EventArgs<EventViewModel> e)
		{
			ViewModel.SendData(sender, e);
		}
	}
}
