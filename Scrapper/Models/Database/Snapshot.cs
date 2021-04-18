using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Snapshot
	{
		public string productId;
		public string offerId;
		public string stars;
		public string numReviews;
		public PriceInformant primaryPrice;
		public PriceInformant alternatePrice;
		public DateTime date;
	}
}
