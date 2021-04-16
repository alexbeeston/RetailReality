using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Bulk : PriceInformant
	{
		public float? quantity = null;
		public float? bulkPrice = null;

		public Bulk(float quantity, float bulkPrice)
		{
			type = PriceType.Bulk;
			this.quantity = quantity;
			this.bulkPrice = bulkPrice;
			individualPrice = bulkPrice / quantity;
			foreignKeyToDerviedPriceTable = Global.counter.GetNextId();
		}

		public override void DataBaseCom()
		{
			Console.Write($"Num1={quantity}. Num2={bulkPrice}. ");
			base.DataBaseCom();
		}

		public override void Validate()
		{
			if (quantity == null) throw new Exception("Quantity is unitialized (Bulk price informant)");
			if (bulkPrice == null) throw new Exception("BulkPrice is unitialized (Bulk price informant)");
			base.Validate();
		}
	}
}
