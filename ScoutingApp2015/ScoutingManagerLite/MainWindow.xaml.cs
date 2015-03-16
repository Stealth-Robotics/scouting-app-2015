using ScoutingData;
using ScoutingData.Data;
using ScoutingData.Lite;
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

namespace ScoutingManagerLite
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		public List<TeamStatsLite> StatsList
		{ get; private set; }

		public FrcEvent Event
		{ get; private set; }

		public TeamsList Teams
		{ get; private set; }

		public string RecordsRoot
		{ get; private set; }

		public List<List<RecordLite>> AllRecords
		{ get; private set; }

		public Dictionary<int, RatingSet> CrunchedNumbers
		{ get; private set; }

		public MainWindow()
		{
			StatsList = new List<TeamStatsLite>();
			AllRecords = new List<List<RecordLite>>();
			CrunchedNumbers = new Dictionary<int, RatingSet>();
			RecordsRoot = ScoutingJson.LocalPath;
			InitializeComponent();
		}

		public void LoadAllRecords()
		{
			if (!Directory.Exists(RecordsRoot))
			{
				return;
			}

			DirectoryInfo root = new DirectoryInfo(RecordsRoot);
			List<DirectoryInfo> teamInfos = root.EnumerateDirectories().ToList();
			teamInfos.RemoveAll((di) => 
			{ 
				int buf = 0; 
				return !int.TryParse(di.Name, out buf); 
			});

			AllRecords.Clear();

			foreach (DirectoryInfo di in teamInfos)
			{
				List<RecordLite> recordSet = new List<RecordLite>();
				List<FileInfo> recordFiles = di.EnumerateFiles().ToList();
				foreach (FileInfo fi in recordFiles)
				{
					RecordLite rec = ScoutingJson.ParseLiteRecord(fi.FullName);
					recordSet.Add(rec);
				}

				AllRecords.Add(recordSet);
			}
		}

		public void CrunchNumbers()
		{
			if (AllRecords.Count <= 0)
			{
				return;
			}

			CrunchedNumbers.Clear();

			foreach (List<RecordLite> recordSet in AllRecords)
			{
				if (recordSet.Count <= 0)
				{
					continue;
				}

				int teamNum = recordSet.First().TeamID;
				TeamStatsLite stats = TeamStatsLite.MakeStats(recordSet);

				CrunchedNumbers.Add(teamNum, stats.GetMeanRatings());
			}
		}
	}
}
