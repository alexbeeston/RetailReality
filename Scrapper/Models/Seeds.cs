using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Scrapper
{
	class Seed
	{
		public Dictionary<string, string> pairs = new Dictionary<string, string>();
		public int id = -1;

		public Seed(int id)
		{
			this.id = id;
		}
		
		public Seed() { }

		public string ToUrl()
		{
			string url= "https://www.kohls.com/catalog.jsp?CN=";
			int counter = 1;
			foreach (KeyValuePair<string, string> pair in pairs)
			{
				url += pair.Key + ":" + Encode(pair.Value);
				if (counter != pairs.Count) url += "+";
				counter++;
			}
			url += "&PPP=120";
			return url;
		}

		private static string Encode(string theString)
		{
			theString = theString.Replace(" ", "%20");
			theString = theString.Replace("&", "%26");
			theString = theString.Replace("'", "%27");
			return theString;
		}
	}
}
