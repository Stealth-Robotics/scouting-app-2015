using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScoutingIO.ViewModel
{
	public class DoStuffCommand : ICommand
	{
		public Action WhatToDo
		{ get; private set; }
		public Predicate<object> IfYouCanDoIt
		{ get; private set; }

		public DoStuffCommand(Action what, Predicate<object> when)
		{
			WhatToDo = what;
			IfYouCanDoIt = when;
		}

		public bool CanExecute(object parameter)
		{
			return IfYouCanDoIt(parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			WhatToDo();
		}
	}
}
