using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Product
	{
		public string Id;
		public string name;
		public string brand;
		public Dictionary<string, string> searchParameters;

		public Product(string id, string name, Dictionary<string, string> searchParameters)
		{
			this.Id = id;
			this.name = name;
			this.searchParameters = searchParameters;
			brand = "[brand here]";
		}
	}
}
