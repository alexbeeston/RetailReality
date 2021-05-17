using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Scrapper
{
	class Seed
	{
		public Dictionary<string, string> searchCriteria = new Dictionary<string, string>();
		public int id = -1;

		public Seed(int id)
		{
			this.id = id;
		}
	
		public string PairsToString()
		{
			string theString = "";
			int counter = 0;
			foreach (var pair in searchCriteria)
			{
				theString += pair.Key + "=" + pair.Value;
				if (counter != searchCriteria.Count -1) theString += "&";
				counter++;
			}
			return theString;
		}

		public Seed() { }

		public string ToUrl()
		{
			string url= "https://www.kohls.com/catalog.jsp?CN=";
			int counter = 1;
			foreach (KeyValuePair<string, string> pair in searchCriteria)
			{
				url += pair.Key + ":" + Encode(pair.Value);
				if (counter != searchCriteria.Count) url += "+";
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
