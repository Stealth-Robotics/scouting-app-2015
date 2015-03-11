using ScoutingAppLite.ViewModel;
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
		public RatingViewModel ViewModel
		{
			get
			{
				return Resources["ViewModel"] as RatingViewModel;
			}
		}

		public RatingView()
		{
			InitializeComponent();
		}
	}
}
