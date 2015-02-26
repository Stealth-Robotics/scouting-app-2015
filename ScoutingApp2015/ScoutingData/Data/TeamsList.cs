using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TeamsList : IList<Team>
	{
		[JsonProperty]
		List<Team> Teams
		{ get; set; }

		public TeamsList()
		{
			Teams = new List<Team>();
		}

		public bool IsCorrectlyLoaded()
		{
			if (Teams == null)
			{
				return false;
			}

			return Teams.Count > 0;
		}

		#region IList
		public int IndexOf(Team item)
		{
			return Teams.IndexOf(item);
		}

		public void Insert(int index, Team item)
		{
			Teams.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Teams.RemoveAt(index);
		}

		public Team this[int index]
		{
			get
			{
				return Teams[index];
			}
			set
			{
				Teams[index] = value;
			}
		}

		public void Add(Team item)
		{
			Teams.Add(item);
		}

		public void Clear()
		{
			Teams.Clear();
		}

		public bool Contains(Team item)
		{
			return Teams.Contains(item);
		}

		public void CopyTo(Team[] array, int arrayIndex)
		{
			Teams.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{ 
				return Teams.Count;
			}
		}

		public bool IsReadOnly
		{
			get 
			{
				return false;
			}
		}

		public bool Remove(Team item)
		{
			return Teams.Remove(item);
		}

		public IEnumerator<Team> GetEnumerator()
		{
			return new TeamsListEnumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new TeamsListEnumerator(this);
		}

		public class TeamsListEnumerator : IEnumerator<Team>
		{
			public TeamsList Teams
			{ get; private set; }

			int id;

			public Team Current
			{
				get 
				{
					return Teams[id];
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get 
				{
					return Current;
				}
			}

			public TeamsListEnumerator(TeamsList list)
			{
				Teams = list;
				Reset();
			}

			public void Dispose()
			{
				Dispose(true);
			}
			protected virtual void Dispose(bool cleanManaged)
			{ 
				if (cleanManaged)
				{
					Teams = null;
				}
			}

			public bool MoveNext()
			{
				if (id == Teams.Count - 1)
				{
					return false;
				}

				id++;
				return true;
			}

			public void Reset()
			{
				id = -1;
			}
		}

		#endregion

		public Team Find(Predicate<Team> foundItComparer)
		{
			return Teams.Find(foundItComparer);
		}
	}
}
