using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Configurations
	{
		public bool writeOffersToConsole;
		public bool logOffersToCsv;
		public bool writeScrapResultsToStatus;
		public int pagesToScrap;
		public string mySqlUserName;
		public string mySqlPassword;
		public string mySqlHostIp;
		public uint mySqlTimeout;
		public List<Combination> combinations = new List<Combination>();
		public List<Pair> pairs = new List<Pair>();

		// could probably delete after dev
		public bool doAsync;
		public bool isDataBaseDevEnv;
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
