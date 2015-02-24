using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScoutingData
{
	public interface IDescribable
	{
		string GetDescription();
		StackPanel GetDescriptionWPF();
	}
}
