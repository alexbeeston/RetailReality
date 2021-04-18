using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scrapper
{
	class SnapShot
	{
		public string productId;
		public string offerId;
		public float? stars;
		public int? reviews;
		public PriceInformant primaryPrice;
		public PriceInformant alternatePrice;
		public DateTime date;

		public SnapShot(
			string productId,
			string offerId,
			float? stars,
			int? reviews,
			PriceInformant primaryPrice,
			PriceInformant alternatePrice,
			DateTime date)
		{
			this.productId = productId;
			this.offerId = offerId;
			this.stars = stars;
			this.reviews = reviews;
			this.primaryPrice = primaryPrice;
			this.alternatePrice = alternatePrice;
			this.date = date;
		}

		public void PrintToScreen()
		{
			Console.WriteLine($"Product Id: {productId}");
			Console.WriteLine($"Offer Id: {offerId}");
			Console.WriteLine($"Stars: {stars}");
			Console.WriteLine($"Reviews: {reviews}");
			Console.WriteLine($"Primary Price: {primaryPrice}");
			Console.WriteLine($"Alternate Price: {alternatePrice}\n");
		}

		public void WriteToFile(StreamWriter file)
		{
			file.Write(AppendComma(productId));
			file.Write(AppendComma(offerId));
			file.Write(AppendComma(stars));
			file.Write(AppendComma(reviews));
			file.Write(AppendComma(primaryPrice.individualPrice));
			file.Write(AppendComma(alternatePrice.individualPrice) + "\n");
		}

		private string AppendComma(string word)
		{
			return word + ",";
		}

		private string AppendComma(float? word)
		{
			if (word == null) return ",";
			else return word + ",";
		}
	}
}
