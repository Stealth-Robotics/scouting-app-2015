using ScoutingData;
using ScoutingData.Analysis;
using ScoutingData.Data;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ScoutingStats
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		public FrcEvent Event
		{ get; private set; }

		public TeamsList Teams
		{ get; private set; }

		public EventAnalysis Analysis
		{ get; private set; }

		public string EventPath
		{ get; private set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		public void LoadCreateAnalysis()
		{
			string analysisPath = EventPath + "\\" + Event.EventName +
				ScoutingJson.AnalysisExtension;

			if (File.Exists(analysisPath))
			{
				Analysis = ScoutingJson.ParseAnalysis(analysisPath);
			}
			else
			{
				Analysis = new EventAnalysis(Event);
			}
			Analysis.PostJsonLoading(Event);
		}

		public void SaveAnalysis()
		{
			ScoutingJson.SaveAnalysis(Analysis, EventPath + "\\" +
				Event.EventName + ScoutingJson.AnalysisExtension);
		}

		public void Calculate()
		{
			Analysis.Calculate();
			SaveAnalysis();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var spsd = new StartupPathSelectionDialog();
			bool? res = spsd.ShowDialog();

			if (res != true)
			{
				Application.Current.Shutdown();
				return;
			}

			Event = ScoutingJson.ParseFrcEvent(spsd.EventPath);
			Teams = ScoutingJson.ParseTeamsList(spsd.TeamsPath);
			EventPath = Util.GetFolderPath(spsd.EventPath);

			if (Event != null && Teams != null)
			{
				Event.PostJsonLoading(Teams);
			}

			LoadCreateAnalysis();
		}
	}
}
