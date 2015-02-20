using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Data
{
	/// <summary>
	/// Interface for objects that require additional loading after 
	/// deserialized from JSON.
	/// </summary>
	public interface IPostJson
	{
		/// <summary>
		/// Additional loading after deserialized from JSON
		/// </summary>
		/// <param name="e">Event to load teams and matches from</param>
		void PostJsonLoading(FrcEvent e);
	}
}
