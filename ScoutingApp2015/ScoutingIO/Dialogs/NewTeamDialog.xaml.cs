using ScoutingData;
using ScoutingData.Data;
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
using System.Windows.Shapes;

namespace ScoutingIO.Dialogs
{
	/// <summary>
	/// Interaction logic for NewTeamDialog.xaml
	/// </summary>
	public partial class NewTeamDialog : Elysium.Controls.Window
	{
		public static readonly Color GRAY_BORDER = Util.MakeColor("FF777777");

		public TeamsList ExistingTeams
		{ get; private set; }

		public int TeamNumber
		{
			get
			{
				return _teamNumber;
			}
			set
			{
				_teamNumber = value;
				NumberBox.Text = value.ToString();
			}
		}
		int _teamNumber = 0;

		public string TeamName
		{
			get
			{
				return NameBox.Text;
			}
			set
			{
				NameBox.Text = value;
			}
		}

		public string Location
		{
			get
			{
				return LocationBox.Text;
			}
			set
			{
				LocationBox.Text = value;
			}
		}

		public string Description
		{
			get
			{
				return DescriptionBox.Text;
			}
			set
			{
				DescriptionBox.Text = value;
			}
		}

		public string Expectations
		{
			get
			{
				return ExpectationsBox.Text;
			}
			set
			{
				ExpectationsBox.Text = value;
			}
		}

		public NewTeamDialog(TeamsList existing)
		{
			ExistingTeams = existing;
			InitializeComponent();
		}

		public Team MakeTeam()
		{
			Team res = new Team();
			res.Number = TeamNumber;
			res.Name = TeamName;
			res.School = Location;
			res.Description = Description;
			res.Expectations = Expectations;

			return res;
		}

		private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int n = -1;
			bool worked = int.TryParse(NumberBox.Text, out n);
			if (!worked)
			{
				NumberBox.Text = TeamNumber.ToString();
			}
			else if (ExistingTeams.Contains(n))
			{
				NumberBox.BorderBrush = new SolidColorBrush(Colors.Red);
				NumberBox.ToolTip = "A team with that number already exists.";
				OKBtn.IsEnabled = false;
			}
			else
			{
				TeamNumber = n;

				NumberBox.BorderBrush = new SolidColorBrush(GRAY_BORDER);
				NumberBox.ToolTip = null;

				if (TeamName.Trim() != "")
				{
					OKBtn.IsEnabled = true;
					NameBox.BorderBrush = new SolidColorBrush(GRAY_BORDER);
					NameBox.ToolTip = null;
				}
				else
				{
					OKBtn.IsEnabled = false;
					NameBox.BorderBrush = new SolidColorBrush(Colors.Red);
					NameBox.ToolTip = "You must specify a name.";
				}
			}
		}

		private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!ExistingTeams.Contains(TeamNumber))
			{
				if (TeamName.Trim() != "")
				{
					OKBtn.IsEnabled = true;
					NameBox.BorderBrush = new SolidColorBrush(GRAY_BORDER);
					NameBox.ToolTip = null;
				}
				else
				{
					OKBtn.IsEnabled = false;
					NameBox.BorderBrush = new SolidColorBrush(Colors.Red);
					NameBox.ToolTip = "You must specify a name.";
				}
			}
		}

		private void OKBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NumberBox_TextChanged(sender, null);
		}

		private void NumberBox_GotFocus(object sender, RoutedEventArgs e)
		{
			NumberBox.SelectAll();
		}
	}
}
