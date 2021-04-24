using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	static class DataBaseCom
	{
		public static void FlushSnapShots(List<SnapShot> snapshots)
		{
			foreach (var snapshot in snapshots)
			{
				if (!ProductInDatabase(snapshot.product)) AddProduct(snapshot.product);
				AddSnapShot(snapshot);
			}
		}

		public static void AddSnapShot(SnapShot snapshot)
		{

		}

		public static void AddProduct(Product product)
		{
			// a Product doesn't have a 'first offer date', but add it to the database in this method;
			// we know we haven't yet added this product to the database, so we can add DateTime.Now
			// as the first recorded offer, because we're adding an offer/snapshot
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

		public static void InitDatabase()
		{

		}
	}
}
