using System;
using System.Collections.Generic;
using System.Text;

namespace DataStore
{
	public class Range : PriceInformant
	{
		public float? low = null;
		public float? high = null;

		public Range(float low, float high)
		{
			type = PriceType.Range;
			this.low = low;
			this.high = high;
			price = (low + high) / 2;
		}

		public override void Validate()
		{
			if (low == null) throw new Exception("Low price is unitialized (Range price informant)");
			if (high == null) throw new Exception("High price is unitialized (Range price informant)");
			base.Validate();
		}

		public override float? price1 => low;
		public override float? price2 => high;
	}
}
