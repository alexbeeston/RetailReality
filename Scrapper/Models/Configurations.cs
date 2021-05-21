using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Configurations
	{
		public bool logOffersToConsole;
		public bool logOffersToCsv;
		public bool logOffersToJson;
		public bool logScrapReportToConsole;
		public int pagesToScrapPerSeed;
		public bool randomizeSeeds;
		public string mySqlUserName;
		public string mySqlPassword;
		public string mySqlHostIp;
		public uint mySqlTimeout;
		public bool skipToFlushOffers;
		public List<Combination> combinations = new List<Combination>();
		public List<Pair> pairs = new List<Pair>();
	}
	
	class Combination
	{
		public int id;
		public List<int> include;
	}

	class Pair
	{
		public int id;
		public string key;
		public string value;
	}
}
