using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Product
	{
		public string id;
		public string name;
		public string brand;
		public Dictionary<string, string> searchParameters;

		public Product(string id, string name, Dictionary<string, string> searchParameters)
		{
			this.id = id;
			this.name = name;
			this.searchParameters = searchParameters;
			brand = "figure out how to get branch from title";
		}
	}
}
