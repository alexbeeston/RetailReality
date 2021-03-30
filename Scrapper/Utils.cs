namespace Scrapper
{
	class Utils
	{
		public static string financialQuantityRegex = @"\d+.\d{2}";
		public static string priceRegEx = @"\$" + financialQuantityRegex;
		public static string rangeRegEx = priceRegEx + " - " + priceRegEx;
		public static Counter counter = new Counter();
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