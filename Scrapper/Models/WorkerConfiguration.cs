using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class WorkerConfiguration
	{
		public bool writeOffersToConsole;
		public bool logOffersToCsv;
		public bool writeScrapResultsToStatus;
		public int pagesToScrap; // if -1, scrap all pages
	}
}
