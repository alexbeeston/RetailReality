using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class PriceInformant
	{
		public PriceType? type = null;
		public LabelType? label = null;
		public float? individualPrice = null;
		protected int foreignKeyToDerviedPriceTable = -1;

		public virtual void DataBaseCom()
		{
			Console.WriteLine($"Label={label}. Type={type}. Amount={individualPrice}. Key={foreignKeyToDerviedPriceTable}.");
		}

		public virtual void Validate()
		{
			if (type == null) throw new Exception("PriceType is unitialized");
			if (label == null) throw new Exception("Label is unitialized");
			if (individualPrice == null) throw new Exception("Amount is unitialized");
		}
	}
}
