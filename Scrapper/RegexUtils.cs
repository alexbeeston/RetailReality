using System;
using System.Text.RegularExpressions;

namespace Scrapper
{
	class RegexUtils
	{
		public static string financialQuantityRegex = @"\d+.\d{2}";
		public static string priceRegEx = @"\$" + financialQuantityRegex;

		public static string rangeRegEx = priceRegEx + " - " + priceRegEx;
		public static string bulkRegEx = @"\d / " + priceRegEx;
		public static string hybridRegEx = priceRegEx + " OR " + bulkRegEx;

		public static Counter counter = new Counter();

		public static bool IsPerfectMatch(string text, string pattern)
		{
			return Regex.IsMatch(text, pattern) && Regex.Match(text, pattern).Length == text.Length;
		}

		public static PriceInformant BuildSingle(string text)
		{
			if (Regex.Matches(text, financialQuantityRegex).Count != 1) throw new Exception("Found more than one match, expected only one.");
			float amount = float.Parse(Regex.Match(text, financialQuantityRegex).Value);
			return new PriceInformant()
			{
				individualPrice = amount,
				type = PriceType.Single
			};
		}

		public static Range BuildRangeInformant(string text)
		{
			MatchCollection amounts = Regex.Matches(text, financialQuantityRegex);
			if (amounts.Count != 2) throw new Exception("Parsed a range text, but did not get 2 financial quantities for input text\"" + text + "\"");
			float low = float.Parse(amounts[0].Value);
			float high = float.Parse(amounts[1].Value);
			return new Range(low, high);
		}
		
		public static Bulk BuildBulkInformant(string text)
		{
			int quantity = int.Parse(Regex.Match(text, @"\d+").Value);
			float totalPrice = float.Parse(Regex.Match(text, financialQuantityRegex).Value);
			return new Bulk(quantity, totalPrice);
		}

		public static Hybrid BuildHybridInformant(string text)
		{
			float individualPrice = float.Parse(Regex.Match(text, financialQuantityRegex).Value);
			Bulk tempBulkInformant = BuildBulkInformant(Regex.Match(text, bulkRegEx).Value);
			float quantity = (float)tempBulkInformant.quantity;
			float bulkPrice = (float)tempBulkInformant.bulkPrice;
			return new Hybrid(individualPrice, quantity, bulkPrice);
		}
	}

	class Counter
	{
		public int lastId;
		public int GetNextId()
		{
			lastId++;
			return lastId;
		}
	}
}