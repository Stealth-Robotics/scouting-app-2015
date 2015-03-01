using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingIO
{
	public delegate void EventHandler<T>(object sender, EventArgs<T> e);

	public class EventArgs<T> : EventArgs
	{
		public T Arg
		{ get; set; }

		public EventArgs(T arg) : base()
		{
			Arg = arg;
		}
	}
}
