using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scrapper
{
	class Offer
	{
		public Product product;
		public string id;
		public float? stars;
		public int? reviews;
		public PriceInformant primaryPrice;
		public PriceInformant alternatePrice;
		public DateTime date;

		public Offer(
			Product product,
			string offerId,
			float? stars,
			int? reviews,
			PriceInformant primaryPrice,
			PriceInformant alternatePrice,
			DateTime date)
		{
			this.product = product;
			this.id = offerId;
			this.stars = stars;
			this.reviews = reviews;
			this.primaryPrice = primaryPrice;
			this.alternatePrice = alternatePrice;
			this.date = date;
		}

		public void WriteToConsole()
		{
			Console.WriteLine($"Product Id: {product.id}");
			Console.WriteLine($"Offer Id: {id}");
			Console.WriteLine($"Stars: {stars}");
			Console.WriteLine($"Reviews: {reviews}");
			Console.WriteLine($"Primary Price: {primaryPrice}");
			Console.WriteLine($"Alternate Price: {alternatePrice}\n");
		}

		public void LogToFile(StreamWriter file)
		{
			file.Write(AppendComma(id));
			file.Write(AppendComma(id));
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
