using System;

namespace Scrapper
{
	class PriceInformant
	{
		public PriceType? type = null;
		public Label? label = null;
		public float? amount = null;
		protected int foreignKeyToDerviedPriceTable = -1;

		public virtual void DataBaseCom()
		{
			Console.WriteLine($"Label={label}. Type={type}. Amount={amount}. Key={foreignKeyToDerviedPriceTable}.");
		}

		public virtual void Validate()
		{
			if (type == null) throw new Exception("PriceType is unitialized");
			if (label == null) throw new Exception("Label is unitialized");
			if (amount == null) throw new Exception("Amount is unitialized");
		}
	}

	class Bulk : PriceInformant
	{
		public float? quantity = null;
		public float? bulkPrice = null;

		Bulk(float quantity, float bulkPrice)
		{
			type = PriceType.Bulk;
			this.quantity = quantity;
			this.bulkPrice = bulkPrice;
			amount = bulkPrice / quantity;
			foreignKeyToDerviedPriceTable = Utils.counter.GetNextId();
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

	class Range : PriceInformant
	{
		public float? low = null;
		public float? high = null;

		public Range(float low, float high)
		{
			type = PriceType.Range;
			this.low = low;
			this.high = high;
			amount = (low + high) / 2;
			foreignKeyToDerviedPriceTable = Utils.counter.GetNextId();
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