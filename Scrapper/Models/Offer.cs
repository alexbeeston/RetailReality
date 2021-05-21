using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scrapper
{
	class Offer
	{
		public Product product;
		public float? stars;
		public int? reviews;
		public PriceInformant primaryPrice;
		public PriceInformant alternatePrice;
		public DateTime date;

		public Offer(
			Product product,
			float? stars,
			int? reviews,
			PriceInformant primaryPrice,
			PriceInformant alternatePrice,
			DateTime date)
		{
			this.product = product;
			this.stars = stars;
			this.reviews = reviews;
			this.primaryPrice = primaryPrice;
			this.alternatePrice = alternatePrice;
			this.date = date;
		}

		public void LogToConsole()
		{
			Console.WriteLine($"Product Id: {product.Id}");
			Console.WriteLine($"Stars: {stars}");
			Console.WriteLine($"Reviews: {reviews}");
			Console.WriteLine($"Primary Price: {primaryPrice}");
			Console.WriteLine($"Alternate Price: {alternatePrice}\n");
		}
	}
}
