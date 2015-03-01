using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScoutingData.Data;
using System.Windows.Input;
using System.Windows.Data;

namespace ScoutingIO.ViewModel
{
	public class MatchViewModel : INotifyPropertyChanged
	{
		bool loadingMatch = false;

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

				if (!loadingMatch)
				{
					Match_Number_String = value.Number.ToString();
				}

				OnPropertyChanged("SelectedMatch");
			}
		}
		Match _selectedMatch;

		public string Match_Number_String
		{
			get
			{
				return _match_Number_String;
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					if (i > 0)
					{
						_match_Number = i;
						_match_Number_String = value;
						OnPropertyChanged("Match_Number_String");
					}
				}
			}
		}
		string _match_Number_String;
		int _match_Number;

		// Over 250 lines to do what is pretty easy to explain
		// in less than 5. THIS is why #region exists.
		#region TeamNumbers

		#region Red A
		public string RedA_Number_String
		{
			get
			{
				return _redA_Number_String;
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					_redA_Number = i;
					_redA_Number_String = value;
					OnPropertyChanged("RedA_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					if (t != null)
					{
						RedA_Tooltip = t.GetDescription();
					}
					else
					{
						RedA_Tooltip = "Team does not exist.";
					}

					SelectedMatch.RedAlliance.A = t;
				}
			}
		}
		string _redA_Number_String;
		int _redA_Number;
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
				return _redB_Number_String;
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					_redB_Number = i;
					_redB_Number_String = value;
					OnPropertyChanged("RedB_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					if (t != null)
					{
						RedB_Tooltip = t.GetDescription();
					}
					else
					{
						RedB_Tooltip = "Team does not exist.";
					}

					SelectedMatch.RedAlliance.B = t;
				}
			}
		}
		string _redB_Number_String;
		int _redB_Number;
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
				return _redC_Number_String;
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					_redC_Number = i;
					_redC_Number_String = value;
					OnPropertyChanged("RedA_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					if (t != null)
					{
						RedC_Tooltip = t.GetDescription();
					}
					else
					{
						RedC_Tooltip = "Team does not exist.";
					}

					SelectedMatch.RedAlliance.C = t;
				}
			}
		}
		string _redC_Number_String;
		int _redC_Number;
		public string RedC_Tooltip
		{
			get
			{
				return _redC_Tooltip;
			}
			set
			{
				_redC_Tooltip = value;
				OnPropertyChanged("RedA_Tooltip");
			}
		}
		string _redC_Tooltip;
		#endregion

		#region Blue A
		public string BlueA_Number_String
		{
			get
			{
				return _blueA_Number_String;
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					_blueA_Number = i;
					_blueA_Number_String = value;
					OnPropertyChanged("BlueA_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					if (t != null)
					{
						BlueA_Tooltip = t.GetDescription();
					}
					else
					{
						BlueA_Tooltip = "Team does not exist.";
					}

					SelectedMatch.BlueAlliance.A = t;
				}
			}
		}
		string _blueA_Number_String;
		int _blueA_Number;
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
				return _blueB_Number_String;
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					_blueB_Number = i;
					_blueB_Number_String = value;
					OnPropertyChanged("BlueB_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					if (t != null)
					{
						BlueB_Tooltip = t.GetDescription();
					}
					else
					{
						BlueB_Tooltip = "Team does not exist.";
					}

					SelectedMatch.BlueAlliance.B = t;
				}
			}
		}
		string _blueB_Number_String;
		int _blueB_Number;
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
				return _blueC_Number_String;
			}
			set
			{
				int i = -1;
				bool worked = int.TryParse(value, out i);
				if (worked)
				{
					_blueC_Number = i;
					_blueC_Number_String = value;
					OnPropertyChanged("BlueA_Number_String");

					Team t = EventVM.Event.LoadTeam(i);
					if (t != null)
					{
						BlueC_Tooltip = t.GetDescription();
					}
					else
					{
						BlueC_Tooltip = "Team does not exist.";
					}

					SelectedMatch.BlueAlliance.A = t;
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
		{ get; private set; }

		List<MatchModel> Matches_List
		{
			get
			{
				List<MatchModel> res = EventVM.Event.Matches.ConvertAll<MatchModel>(
					(m) => new MatchModel(this, m));
				OnPropertyChanged("Matches_List");
				Matches = CollectionViewSource.GetDefaultView(res);
				return res;
			}
		}

		public ICommand TeamPickCmd
		{ get; private set; }

		public ICommand NewMatchCmd
		{ get; private set; }

		public ICommand DeleteMatchCmd
		{ get; private set; }

		// DEBUG
		public ICommand BreakCmd
		{ get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		// CTOR
		public MatchViewModel()
		{
			PropertyChanged += OnMatchIDChanged;
			//PropertyChanged += OnEventChanged;

			TeamPickCmd = new DoStuffWithStuffCommand((stuffWith) => 
				{ OnTeamPick(stuffWith as string); }, obj => true);

			BreakCmd = new DoStuffCommand(Break, obj => true);
		}

		void Break()
		{
			System.Diagnostics.Debugger.Break();
		}

		public void OnPropertyChanged(string prop)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}

		public void OnMatchIDChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Match_Number_String" || loadingMatch)
			{
				return; // none of my business
			}

			loadingMatch = true;
			SelectedMatch = EventVM.Event.LoadMatch(_match_Number);

			if (SelectedMatch != null)
			{
				RedA_Number_String = SelectedMatch.RedAlliance.TeamA_ID.ToString();
				RedB_Number_String = SelectedMatch.RedAlliance.TeamB_ID.ToString();
				RedC_Number_String = SelectedMatch.RedAlliance.TeamC_ID.ToString();

				BlueA_Number_String = SelectedMatch.BlueAlliance.TeamA_ID.ToString();
				BlueB_Number_String = SelectedMatch.BlueAlliance.TeamB_ID.ToString();
				BlueC_Number_String = SelectedMatch.BlueAlliance.TeamC_ID.ToString();
			}

			loadingMatch = false;
		}

		public void OnEventChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "EventVM")
			{
				return;
			}

			SelectedMatch = EventVM.Event.Matches.FirstOrDefault();
			Match_Number_String = SelectedMatch.Number.ToString();
		}
		
		public void SendData(object sender, EventArgs<EventViewModel> e)
		{
			EventVM = e.Arg;
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

			public int No
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
					OnPropertyChanged("No");
				}
			}

			public int RedA
			{
				get
				{
					return vm._redA_Number;
				}
				set
				{
					// kinda stupid but it keeps the line count for the
					// team numbers region out of the thousands
					vm.RedA_Number_String = value.ToString();
					OnPropertyChanged("RedA");
				}
			}
			public int RedB
			{
				get
				{
					return vm._redB_Number;
				}
				set
				{
					// kinda stupid but it keeps the line count for the
					// team numbers region out of the thousands
					vm.RedB_Number_String = value.ToString();
					OnPropertyChanged("RedB");
				}
			}
			public int RedC
			{
				get
				{
					return vm._redC_Number;
				}
				set
				{
					// kinda stupid but it keeps the line count for the
					// team numbers region out of the thousands
					vm.RedC_Number_String = value.ToString();
					OnPropertyChanged("RedC");
				}
			}
			public int BlueA
			{
				get
				{
					return vm._blueA_Number;
				}
				set
				{
					// kinda stupid but it keeps the line count for the
					// team numbers region out of the thousands
					vm.BlueA_Number_String = value.ToString();
					OnPropertyChanged("BlueA");
				}
			}
			public int BlueB
			{
				get
				{
					return vm._blueB_Number;
				}
				set
				{
					// kinda stupid but it keeps the line count for the
					// team numbers region out of the thousands
					vm.BlueB_Number_String = value.ToString();
					OnPropertyChanged("BlueB");
				}
			}
			public int BlueC
			{
				get
				{
					return vm._blueC_Number;
				}
				set
				{
					// kinda stupid but it keeps the line count for the
					// team numbers region out of the thousands
					vm.BlueC_Number_String = value.ToString();
					OnPropertyChanged("BlueC");
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public MatchModel(MatchViewModel arg_vm, Match arg_match)
			{
				vm = arg_vm;
				match = arg_match;
			}

			public void OnPropertyChanged(string propName)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propName));
				}
			}
		}
	}
}
