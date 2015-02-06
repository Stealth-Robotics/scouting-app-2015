﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingData.Analysis
{
	public struct FiveNumberSummary
	{
		public double Min
		{ get; private set; }

		public double Q1
		{ get; private set; }

		public double Med
		{ get; private set; }

		public double Q3
		{ get; private set; }

		public double Max
		{ get; private set; }

		public FiveNumberSummary(double min, double q1, double med, double q3, double max)
			: this()
		{
			Min = min;
			Q1 = q1;
			Med = med;
			Q3 = q3;
			Max = max;
		}

		public override string ToString()
		{
			return "[" + Min.ToString() + " " + Q1.ToString() + " " + Med.ToString() + 
				" " + Q3.ToString() + " " + Max.ToString() + "]";
		}
	}
}
