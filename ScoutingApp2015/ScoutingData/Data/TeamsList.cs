using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	[JsonObject(MemberSerialization.OptIn)]
	public class TeamsList : IList<Team>, IPostJson
	{
		[JsonIgnore]
		List<Team> Teams
		{ get; set; }

		[JsonProperty]
		List<int> TeamIDs
		{ get; set; }

		public TeamsList()
		{
			Teams = new List<Team>();
			TeamIDs = new List<int>();
		}

		public Team GetTeam(int teamID)
		{
			int index = TeamIDs.IndexOf(teamID);
			return Teams[index];
		}

		#region IList
		public int IndexOf(Team item)
		{
			return Teams.IndexOf(item);
		}

		public void Insert(int index, Team item)
		{
			Teams.Insert(index, item);
			TeamIDs.Insert(index, item.Number);
		}

		public void RemoveAt(int index)
		{
			Teams.RemoveAt(index);
			TeamIDs.RemoveAt(index);
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
				TeamIDs[index] = value.Number;
			}
		}

		public void Add(Team item)
		{
			Teams.Add(item);
			TeamIDs.Add(item.Number);
		}

		public void Clear()
		{
			Teams.Clear();
			TeamIDs.Clear();
		}

		public bool Contains(Team item)
		{
			return TeamIDs.Contains(item.Number);
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
			bool t = Teams.Remove(item);
			bool id = TeamIDs.Remove(item.Number);

			return t && id;
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

		public void PostJsonLoading(FrcEvent e)
		{
			Teams.Clear();
			foreach (int i in TeamIDs)
			{
				Teams.Add(e.LoadTeam(i));
			}
		}
	}
}
