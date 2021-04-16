using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Range : PriceInformant
	{
		public float? low = null;
		public float? high = null;

		public Range(float low, float high)
		{
			type = PriceType.Range;
			this.low = low;
			this.high = high;
			individualPrice = (low + high) / 2;
			foreignKeyToDerviedPriceTable = Global.counter.GetNextId();
		}

		public override void DataBaseCom()
		{
			Console.Write($"Num1={low}. Num2={high}. ");
			base.DataBaseCom();
		}

		public override void Validate()
		{
			if (low == null) throw new Exception("Low price is unitialized (Range price informant)");
			if (high == null) throw new Exception("High price is unitialized (Range price informant)");
			base.Validate();
		}
	}
}
