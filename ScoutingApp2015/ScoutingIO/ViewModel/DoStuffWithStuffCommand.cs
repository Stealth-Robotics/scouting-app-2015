using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScoutingIO.ViewModel
{
	public class DoStuffWithStuffCommand : ICommand
	{
		Action<object> doStuffWithStuff;
		Predicate<object> canDoStuffWithStuff;

		public DoStuffWithStuffCommand(Action<object> doStuff, Predicate<object> can)
		{
			doStuffWithStuff = doStuff;
			canDoStuffWithStuff = can;
		}

		public bool CanExecute(object parameter)
		{
			return canDoStuffWithStuff(parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			doStuffWithStuff(parameter);
		}
	}
}
