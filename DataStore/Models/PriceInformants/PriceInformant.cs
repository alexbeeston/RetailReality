using System;
using System.Collections.Generic;
using System.Text;

namespace DataStore
{
	public class PriceInformant
	{
		public PriceType? type = null;
		public LabelType? label = null;
		public float? individualPrice = null;

		public virtual void Validate()
		{
			if (type == null) throw new Exception("PriceType is unitialized");
			if (label == null) throw new Exception("Label is unitialized");
			if (individualPrice == null) throw new Exception("Amount is unitialized");
		}

		public virtual float? FirstNumber => null;

		public virtual float? SecondNumber => null;

	}
}
