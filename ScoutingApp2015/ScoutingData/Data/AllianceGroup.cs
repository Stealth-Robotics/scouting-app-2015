using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	public enum AllianceColor
	{
		Red,
		Blue
	}

	public enum AlliancePosition
	{
		A,
		B,
		C
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class AllianceGroup<T> : IEnumerable<T>
	{
		[JsonProperty]
		public AllianceColor Color
		{ get; set; }

		[JsonProperty]
		public virtual T A
		{ get; set; }

		[JsonProperty]
		public virtual T B
		{ get; set; }

		[JsonProperty]
		public virtual T C
		{ get; set; }

		public AllianceGroup(T a, T b, T c)
		{
			A = a;
			B = b;
			C = c;
		}

		#region Interfaces

		public IEnumerator<T> GetEnumerator()
		{
			return new AllianceEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal class AllianceEnumerator : IEnumerator<T>
		{
			int position;
			AllianceGroup<T> alliance;

			public AllianceEnumerator(AllianceGroup<T> al)
			{
				alliance = al;
				position = 0;
			}

			public T Current
			{
				get
				{
					switch (position)
					{
					case 0:
						return alliance.A;
					case 1:
						return alliance.B;
					case 2:
						return alliance.C;
					default:
						return alliance.A;
					}
				}
			}

			public void Dispose()
			{ }

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				position++;

				if (position > 2)
				{
					position = 2;
					return false;
				}

				return true;
			}

			public void Reset()
			{
				position = 0;
			}
		}

		#endregion
	}
}
