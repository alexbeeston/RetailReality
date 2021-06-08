using System;
using System.Collections.Generic;
using System.Text;

namespace DataStore
{
	public class Product
	{
		public string id;
		public string title;
		public string brand;
		public SearchCriteria searchCriteria;

		public Product(string id, string title, SearchCriteria searchCriteria)
		{
			this.id = id;
			this.title = title;
			this.searchCriteria = searchCriteria;
			brand = "[brand here]";
		}
	}
}
