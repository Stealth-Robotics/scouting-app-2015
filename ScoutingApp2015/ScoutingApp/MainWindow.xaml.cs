﻿using ScoutingData;
using ScoutingData.Data;
using ScoutingData.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace ScoutingApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		public const string DEFENSE_PREFIX = "Defense: ";
		public const string LEVEL_PREFIX = "Level: ";

		public DispatcherTimer timer;

		public RecordedMatch Record
		{ get; private set; }

		public FrcEvent Frc
		{ get; private set; }

		public MainWindow()
		{
			InitializeComponent();
			timer = new DispatcherTimer();
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

			int val = (int)e.NewValue;
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

			int val = (int)e.NewValue;
			RecyclingLevelLbl.Content = DEFENSE_PREFIX + val.ToString();
		}

		private void MatchSetupBtn_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow window = new SettingsWindow(Frc, Record.MatchNumber);
			bool? result = window.ShowDialog();

			if (result == true) // nullable bool
			{
				ScoutingAppSettings settings = window.Settings;

				Record = settings.MakeRecord();
			}
		}

		private void ToteSetToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			ToteSetStackedToggle.IsChecked = false;
		}

		private void ScoutingMainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += timer_Tick;

			InitLoadFiles();
		}

		public void InitLoadFiles()
		{
			ScoutingJson.Initialize(false);
			Frc = ScoutingJson.ParseFrcEvent("Default.json", false);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			Util.DebugLog(LogLevel.Info, "TICK");
		}
	}
}
