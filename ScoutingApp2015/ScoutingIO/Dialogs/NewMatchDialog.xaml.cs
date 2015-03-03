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
	/// Interaction logic for NewMatchDialog.xaml
	/// </summary>
	public partial class NewMatchDialog : Elysium.Controls.Window
	{
		FrcEvent frc;

		public int MatchID
		{
			get
			{
				int id = -1;
				bool worked = int.TryParse(MatchNumberBox.Text, out id);
				return id;
			}
			set
			{
				MatchNumberBox.Text = value.ToString();
			}
		}

		#region teams
		public int RedA
		{
			get
			{
				int id = -1;
				bool worked = int.TryParse(RedABox.Text, out id);
				return id;
			}
			set
			{
				RedABox.Text = value.ToString();
			}
		}
		public int RedB
		{
			get
			{
				int id = -1;
				bool worked = int.TryParse(RedBBox.Text, out id);
				return id;
			}
			set
			{
				RedBBox.Text = value.ToString();
			}
		}
		public int RedC
		{
			get
			{
				int id = -1;
				bool worked = int.TryParse(RedCBox.Text, out id);
				return id;
			}
			set
			{
				RedCBox.Text = value.ToString();
			}
		}

		public int BlueA
		{
			get
			{
				int id = -1;
				bool worked = int.TryParse(BlueABox.Text, out id);
				return id;
			}
			set
			{
				BlueABox.Text = value.ToString();
			}
		}
		public int BlueB
		{
			get
			{
				int id = -1;
				bool worked = int.TryParse(BlueBBox.Text, out id);
				return id;
			}
			set
			{
				BlueBBox.Text = value.ToString();
			}
		}
		public int BlueC
		{
			get
			{
				int id = -1;
				bool worked = int.TryParse(BlueCBox.Text, out id);
				return id;
			}
			set
			{
				BlueCBox.Text = value.ToString();
			}
		}
		#endregion

		public NewMatchDialog(FrcEvent e)
		{
			frc = e;
			InitializeComponent();
		}

		private void AnythingChanged(object sender, TextChangedEventArgs e)
		{
			RedABox.Text = RedA.ToString();
			RedBBox.Text = RedB.ToString();
			RedCBox.Text = RedC.ToString();
			BlueABox.Text = BlueA.ToString();
			BlueBBox.Text = BlueB.ToString();
			BlueCBox.Text = BlueC.ToString();

			if (RedA == -1 || RedB == -1 || RedC == -1 ||
				BlueA == -1 || BlueB == -1 || BlueC == -1 ||
				MatchID == -1)
			{
				OKBtn.IsEnabled = false;
			}
			else
			{
				OKBtn.IsEnabled = true;
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

		public Match MakeMatch()
		{
			Alliance red = new Alliance(frc.LoadTeam(RedA), 
				frc.LoadTeam(RedB), frc.LoadTeam(RedC));
			Alliance blue = new Alliance(frc.LoadTeam(BlueA),
				frc.LoadTeam(BlueB), frc.LoadTeam(BlueC));
			return new Match(MatchID, red, blue);
		}
	}
}
