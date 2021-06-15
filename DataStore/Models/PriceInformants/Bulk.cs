using System;
using System.Collections.Generic;
using System.Text;

namespace DataStore
{
	public class Bulk : PriceInformant
	{
		public float? quantity = null;
		public float? bulkPrice = null;

		public Bulk(float quantity, float bulkPrice)
		{
			type = PriceType.Bulk;
			this.quantity = quantity;
			this.bulkPrice = bulkPrice;
			price = bulkPrice / quantity;
		}

		public override void Validate()
		{
			if (quantity == null) throw new Exception("Quantity is unitialized (Bulk price informant)");
			if (bulkPrice == null) throw new Exception("BulkPrice is unitialized (Bulk price informant)");
			base.Validate();
		}

		public override float? price1 => quantity;
		public override float? price2 => bulkPrice;
	}
}
