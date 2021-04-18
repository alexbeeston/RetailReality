using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper.Models.Database
{
	class Product
	{
		public string productId;
		public string name;
		public string brand;
		public Dictionary<string, string> searchParameters;
		public DateTime firstRecordedOffer;
	}
}
