using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScoutingManagerLite.Dialog
{
	/// <summary>
	/// Interaction logic for CrunchingWindow.xaml
	/// </summary>
	[Obsolete]
	public partial class CrunchingWindow : Elysium.Controls.Window
	{
		public Action StuffToDo
		{ get; set; }

		Thread thread;

		public CrunchingWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			thread = new Thread(OnThreadStart);

			thread.Start();
		}

		private void OnThreadStart()
		{
			if (StuffToDo != null)
			{
				StuffToDo();
			}

			Dispatcher.Invoke(() =>
			{
				DialogResult = true;
				Close();
			});
		}
	}
}
