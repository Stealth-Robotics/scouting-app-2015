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

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		public const string DEFENSE_PREFIX = "Defense: ";
		public const string LEVEL_PREFIX = "Level: ";

		public MainWindow()
		{
			InitializeComponent();
		}

		private void UnprocessedLitterDownBtn_Click(object sender, RoutedEventArgs e)
		{
			int count = int.Parse(UnprocessedLitterCount.Text);
			count = Math.Max(0, count - 1);
			UnprocessedLitterCount.Text = count.ToString();
		}

		private void UnprocessedLitterUpBtn_Click(object sender, RoutedEventArgs e)
		{
			int count = int.Parse(UnprocessedLitterCount.Text);
			count++;
			UnprocessedLitterCount.Text = count.ToString();
		}

		private void DefenseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!IsLoaded)
			{
				return;
			}

			int val = (int)DefenseSlider.Value;
			DefenseLbl.Content = DEFENSE_PREFIX + val.ToString();
		}

		private void ToteSetStackedToggle_Checked(object sender, RoutedEventArgs e)
		{
			ToteSetToggle.IsChecked = true;
		}

		private void RecyclingLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!IsLoaded)
			{
				return;
			}

			int val = (int)RecyclingLevelSlider.Value;
			RecyclingLevelLbl.Content = DEFENSE_PREFIX + val.ToString();
		}
	}
}
