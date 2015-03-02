using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScoutingData.Data;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;

namespace ScoutingIO.ViewModel
{
	public class MatchViewModel : INotifyPropertyChanged
	{
		bool isRefreshingAboveStuff = false;

		public EventViewModel EventVM
		{
			get
			{
				return _eventVM;
			}
			set
			{
				_eventVM = value;
				OnPropertyChanged("EventVM");
			}
		}
		EventViewModel _eventVM;

		public Match SelectedMatch
		{
			get
			{
				return _selectedMatch;
			}
			set
			{
				_selectedMatch = value;

				OnPropertyChanged("SelectedMatch");
				OnPropertyChanged("Match_Number_String");
			}
		}
		Match _selectedMatch;

		public string Match_Number_String
		{
			get
			{
				if (SelectedMatch != null)
				{
					return SelectedMatch.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (value == "-NULL-")
				{
					return;
				}

				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					SelectedMatch = EventVM.Event.LoadMatch(i);
				}

				OnPropertyChanged("Match_Number_String");
			}
		}

		public SolidColorBrush Match_Number_Color
		{
			get
			{
				if (SelectedMatch == null)
				{
					return new SolidColorBrush(Colors.Red);
				}
				else
				{
					return new SolidColorBrush(Colors.Black);
				}
			}
		}

		// Over here to eliminate scrolling need
		void Break()
		{
			System.Diagnostics.Debugger.Break();
		}

		// Over 300 lines to do what is pretty easy to explain
		// in less than 3. THIS is why #region exists.
		#region TeamNumbers

		#region Red A
		public string RedA_Number_String
		{
			get
			{
				if (SelectedMatch != null)
				{
					return SelectedMatch.RedAlliance.A.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (value == "-NULL-")
				{
					return;
				}

				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					OnPropertyChanged("RedA_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					SelectedMatch.RedAlliance.A = t;

					Match m = SelectedMatch;
					OnPropertyChanged("Match_Adjusted");
					SelectedMatch = m;

					if (t != null)
					{
						RedA_Tooltip = t.GetDescription();
					}
					else
					{
						RedA_Tooltip = "Team does not exist.";
					}
				}
			}
		}
		public string RedA_Tooltip
		{
			get
			{
				return _redA_Tooltip;
			}
			set
			{
				_redA_Tooltip = value;
				OnPropertyChanged("RedA_Tooltip");
			}
		}
		string _redA_Tooltip;
		#endregion

		#region Red B
		public string RedB_Number_String
		{
			get
			{
				if (SelectedMatch != null)
				{
					return SelectedMatch.RedAlliance.B.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (value == "-NULL-")
				{
					return;
				}

				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					OnPropertyChanged("RedC_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					SelectedMatch.RedAlliance.B = t;

					Match m = SelectedMatch;
					OnPropertyChanged("Match_Adjusted");
					SelectedMatch = m;

					if (t != null)
					{
						RedB_Tooltip = t.GetDescription();
					}
					else
					{
						RedB_Tooltip = "Team does not exist.";
					}
				}
			}
		}
		public string RedB_Tooltip
		{
			get
			{
				return _redB_Tooltip;
			}
			set
			{
				_redB_Tooltip = value;
				OnPropertyChanged("RedB_Tooltip");
			}
		}
		string _redB_Tooltip;
		#endregion

		#region Red C
		public string RedC_Number_String
		{
			get
			{
				if (SelectedMatch != null)
				{
					return SelectedMatch.RedAlliance.C.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (value == "-NULL-")
				{
					return;
				}

				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					OnPropertyChanged("RedC_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					SelectedMatch.RedAlliance.C = t;

					Match m = SelectedMatch;
					OnPropertyChanged("Match_Adjusted");
					SelectedMatch = m;

					if (t != null)
					{
						RedB_Tooltip = t.GetDescription();
					}
					else
					{
						RedB_Tooltip = "Team does not exist.";
					}
				}
			}
		}
		public string RedC_Tooltip
		{
			get
			{
				return _redC_Tooltip;
			}
			set
			{
				_redC_Tooltip = value;
				OnPropertyChanged("RedC_Tooltip");
			}
		}
		string _redC_Tooltip;
		#endregion

		#region Blue A
		public string BlueA_Number_String
		{
			get
			{
				if (SelectedMatch != null)
				{
					return SelectedMatch.BlueAlliance.A.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (value == "-NULL-")
				{
					return;
				}

				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					OnPropertyChanged("BlueA_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					SelectedMatch.BlueAlliance.A = t;

					Match m = SelectedMatch;
					OnPropertyChanged("Match_Adjusted");
					SelectedMatch = m;

					if (t != null)
					{
						BlueA_Tooltip = t.GetDescription();
					}
					else
					{
						BlueA_Tooltip = "Team does not exist.";
					}
				}
			}
		}
		public string BlueA_Tooltip
		{
			get
			{
				return _blueA_Tooltip;
			}
			set
			{
				_blueA_Tooltip = value;
				OnPropertyChanged("BlueA_Tooltip");
			}
		}
		string _blueA_Tooltip;
		#endregion

		#region Blue B
		public string BlueB_Number_String
		{
			get
			{
				if (SelectedMatch != null)
				{
					return SelectedMatch.BlueAlliance.B.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (value == "-NULL-")
				{
					return;
				}

				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					OnPropertyChanged("BlueB_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					SelectedMatch.BlueAlliance.B = t;

					Match m = SelectedMatch;
					OnPropertyChanged("Match_Adjusted");
					SelectedMatch = m;

					if (t != null)
					{
						BlueB_Tooltip = t.GetDescription();
					}
					else
					{
						BlueB_Tooltip = "Team does not exist.";
					}
				}
			}
		}
		public string BlueB_Tooltip
		{
			get
			{
				return _blueB_Tooltip;
			}
			set
			{
				_blueB_Tooltip = value;
				OnPropertyChanged("BlueB_Tooltip");
			}
		}
		string _blueB_Tooltip;
		#endregion

		#region Blue C
		public string BlueC_Number_String
		{
			get
			{
				if (SelectedMatch != null)
				{
					return SelectedMatch.BlueAlliance.C.Number.ToString();
				}
				else
				{
					return "-NULL-";
				}
			}
			set
			{
				if (value == "-NULL-")
				{
					return;
				}

				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					OnPropertyChanged("BlueC_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					SelectedMatch.BlueAlliance.C = t;

					Match m = SelectedMatch;
					OnPropertyChanged("Match_Adjusted");
					SelectedMatch = m;

					if (t != null)
					{
						BlueC_Tooltip = t.GetDescription();
					}
					else
					{
						BlueC_Tooltip = "Team does not exist.";
					}
				}
			}
		}
		string _blueC_Number_String;
		int _blueC_Number;
		public string BlueC_Tooltip
		{
			get
			{
				return _blueC_Tooltip;
			}
			set
			{
				_blueC_Tooltip = value;
				OnPropertyChanged("BlueA_Tooltip");
			}
		}
		string _blueC_Tooltip;
		#endregion

		#endregion

		public ICollectionView Matches
		{
			get
			{
				return _matches;
			}
			private set
			{
				_matches = value;
				OnPropertyChanged("Matches");
			}
		}
		ICollectionView _matches;

		public MatchModel SelectedMatchModel
		{
			get
			{
				return _selectedMatchModel;
			}
			set
			{
				_selectedMatchModel = value;
				OnPropertyChanged("SelectedMatchModel");
			}
		}
		MatchModel _selectedMatchModel;

		#region commands
		public ICommand TeamPickCmd
		{ get; private set; }

		public ICommand NewMatchCmd
		{ get; private set; }

		public ICommand DeleteMatchCmd
		{ get; private set; }

		public ICommand CellEditedCmd
		{ get; private set; }

		// DEBUG
		public ICommand BreakCmd
		{ get; private set; }
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		// CTOR
		public MatchViewModel()
		{
			//

			TeamPickCmd = new DoStuffWithStuffCommand((stuffWith) => 
				{ OnTeamPick(stuffWith as string); }, obj => true);

			BreakCmd = new DoStuffCommand(Break, obj => true);

			CellEditedCmd = new DoStuffCommand(OnCellEdited, obj => true);
		}

		public void OnPropertyChanged(string prop)
		{
			if (PropertyChanged == null)
			{
				return;
			}

			PropertyChanged(this, new PropertyChangedEventArgs(prop));

			if (prop == "SelectedMatch")
			{
				OnSelectedMatchChanged();
			}

			if (prop == "EventVM")
			{
				OnPropertyChanged("SelectedMatch");
			}

			if (prop == "SelectedMatchModel")
			{
				OnSelectedMatchModelChanged();
			}

			if (prop == "Match_Adjusted")
			{
				RefreshDatagrid();
			}
		}

		public void OnSelectedMatchChanged()
		{
			OnPropertyChanged("Match_Number_String");
			OnPropertyChanged("Match_Number_Color");

			OnPropertyChanged("RedA_Number_String");
			OnPropertyChanged("RedB_Number_String");
			OnPropertyChanged("RedC_Number_String");
			OnPropertyChanged("BlueA_Number_String");
			OnPropertyChanged("BlueB_Number_String");
			OnPropertyChanged("BlueC_Number_String");

			SaveAll();
		}

		public void OnSelectedMatchModelChanged()
		{
			SelectedMatch = SelectedMatchModel.GetMatch();
		}

		public void DoInit()
		{
			SelectedMatch = EventVM.Event.Matches.FirstOrDefault();
			OnPropertyChanged("Match_Number_String");
			RefreshDatagrid();
		}

		private void RefreshDatagrid()
		{
			List<MatchModel> lmm = new List<MatchModel>();
			foreach (Match m in EventVM.Event.Matches)
			{
				lmm.Add(new MatchModel(this, m));
			}

			Matches = CollectionViewSource.GetDefaultView(lmm);
		}
		
		public void SendData(object sender, EventArgs<EventViewModel> e)
		{
			EventVM = e.Arg;
			DoInit();
		}

		public void OnCellEdited()
		{
			OnPropertyChanged("Match_Number_String");
		}

		public void SaveAll()
		{
			EventVM.SaveAll();
		}

		public static bool IsPropertyTeamNumber(string p)
		{
			return p == "RedA_Number_String" ||
				p == "RedB_Number_String" || 
				p == "RedC_Number_String" || 
				p == "BlueA_Number_String" || 
				p == "BlueB_Number_String" || 
				p == "BlueC_Number_String";
		}

		public void OnTeamPick(string teamPosition)
		{
			if (teamPosition == null)
			{
				return;
			}

			switch (teamPosition)
			{
			case "RedA":
				break;
			case "RedB":
				break;
			case "RedC":
				break;
			case "BlueA":
				break;
			case "BlueB":
				break;
			case "BlueC":
				break;
			default:
				break;
			}
		}

		public class MatchModel : INotifyPropertyChanged
		{
			MatchViewModel vm;
			Match match;

			public int Number
			{
				get
				{
					return match.Number;
				}
				set
				{
					// this is where you can edit it.
					match.Number = value;

					// it also switches to it too
					vm.Match_Number_String = value.ToString();
					OnPropertyChanged("Number");
				}
			}

			public int RedA
			{
				get
				{
					return match.RedAlliance.A.Number;
				}
				set
				{
					match.RedAlliance.A = LoadTeam(value);
					OnPropertyChanged("RedA");
				}
			}
			public int RedB
			{
				get
				{
					return match.RedAlliance.B.Number;
				}
				set
				{
					match.RedAlliance.B = LoadTeam(value);
					OnPropertyChanged("RedB");
				}
			}
			public int RedC
			{
				get
				{
					return match.RedAlliance.C.Number;
				}
				set
				{
					match.RedAlliance.C = LoadTeam(value);
					OnPropertyChanged("RedC");
				}
			}
			public int BlueA
			{
				get
				{
					return match.BlueAlliance.A.Number;
				}
				set
				{
					match.BlueAlliance.A = LoadTeam(value);
					OnPropertyChanged("BlueA");
				}
			}
			public int BlueB
			{
				get
				{
					return match.BlueAlliance.B.Number;
				}
				set
				{
					match.BlueAlliance.B = LoadTeam(value);
					OnPropertyChanged("BlueB");
				}
			}
			public int BlueC
			{
				get
				{
					return match.BlueAlliance.C.Number;
				}
				set
				{
					match.BlueAlliance.C = LoadTeam(value);
					OnPropertyChanged("BlueC");
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public MatchModel(MatchViewModel arg_vm, Match arg_match)
			{
				vm = arg_vm;
				match = arg_match;
			}

			public Team LoadTeam(int id)
			{
				return vm.EventVM.Event.LoadTeam(id);
			}

			public void OnPropertyChanged(string propName)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propName));
				}
			}

			// Java style to avoid the DataGrid seeing this as a property
			public Match GetMatch()
			{
				return match;
			}

			public override string ToString()
			{
				return RedA.ToString() + "-" +
					RedB.ToString() + "-" +
					RedC.ToString() + " vs " +
					BlueA.ToString() + "-" +
					BlueB.ToString() + "-" +
					BlueC.ToString();
			}
		}
	}
}
