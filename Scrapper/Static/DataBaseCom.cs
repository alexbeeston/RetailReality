using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	static class DataBaseCom
	{
		public static void FlushOffers(List<Offer> offers)
		{
			foreach (var offer in offers)
			{
				if (!ProductInDatabase(offer.product)) AddProduct(offer.product);
				AddSnapShot(offer);
			}
		}

		public static void AddSnapShot(Offer offer)
		{

		}

		public static void AddProduct(Product product)
		{
			// a Product doesn't have a 'first offer date', but add it to the database in this method;
			// we know we haven't yet added this product to the database, so we can add DateTime.Now
			// as the first recorded offer, because we're adding an offer
		}

		public static bool ProductInDatabase(Product product)
		{
			return true;
		}

		public static bool DatabaseExists()
		{
			// we'll need to get all the keys in the search parameters and make sure that the product table has attributes for them all
			return true;
		}

		public static void VerifyDataBaseConfiguration(List<Seed> seeds)
		{
			// verify the "RetailReality" database exists

			// verify it has the following tables, Offers, Products, Derived Prices, with the following attributes on each table (....)
			foreach (var i in Enum.GetNames(typeof(LabelType)))
			{
				Console.WriteLine(i);
			}

			var uniqueKeys = new List<string>();
			foreach (var seed in seeds)
			{
				foreach (var key in seed.pairs.Keys) if (!uniqueKeys.Contains(key)) uniqueKeys.Add(key);
			}

			// make sure the products table has fields for all unique Keys, and throw an exception or log or something if a field doesn't exist; we don't expect this to change that much

		}
	}
}
