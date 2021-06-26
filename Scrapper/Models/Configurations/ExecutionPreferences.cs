using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class ExecutionPreferences
	{
		public bool logOffersToConsole;
		public bool logOffersToCsv;
		public bool logOffersToJson;
		public bool logScrapReportToConsole;
		public int pagesToScrapPerSeed;
		public bool randomizeSeeds;
		public int maxPagesToScrapBeforeFlushing;
		public string mySqlUserName;
		public string mySqlPassword;
		public string mySqlHostIp;
		public uint mySqlTimeout;
		public bool skipToFlushOffers;
		public int maxSeedsToScrap;
		public bool pickRandomSerialization;
		public string serializationToUse;
	}
}
