using System;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using Newtonsoft.Json;

namespace Scrapper
{
	class Program
	{
		public static void Main()
		{
			var configs = GetConfigs();
			if (configs.executionPreferences.skipToFlushOffers)
			{
				Console.WriteLine("Are you sure you want to skip scrapping? Enter 'y' for yes or any other key for no.");
				var selection = Console.ReadLine();
				if (selection.CompareTo("y") != 0) return;
				 
				var pathToSerializedOffers = Directory.GetFiles(@"..\..\..\Data\serializations").ToList();
				var random = new Random();
				pathToSerializedOffers.OrderBy(x => random.Next()).ToList().First();
				var offers = JsonConvert.DeserializeObject<List<Offer>>(File.ReadAllText(pathToSerializedOffers.First()));
				var worker = new Worker(configs, offers);
			}
			else
			{
				Console.WriteLine("Are you sure you want to continue onto scrap pages? Enter 'y' for yes or any other key for no.");
				var selection = Console.ReadLine();
				if (selection.CompareTo("y") != 0) return;

				IWebDriver driver = new ChromeDriver();
				int counter = 0;
				foreach (var searchCriteria in configs.searchCriterion)
				{
					if (counter > configs.executionPreferences.maxSeedsToScrap) break;
					Worker worker = new Worker(driver, searchCriteria, configs.executionPreferences);
					worker.GetOffers();
					worker.LogOffers();
					// worker.LogScrapReport();
					//worker.FlushOffers();
					counter++;
				}
				driver.Quit();
			}
		}

		static Configurations GetConfigs()
		{
			var configs = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(@"..\..\..\configurations.json"));
			Console.WriteLine($"Enter password for MySql user {configs.executionPreferences.mySqlUserName}:");
			configs.executionPreferences.mySqlPassword = Console.ReadLine();
			return configs;
		}

		//public string ToUrl()
		//{
		//	string url= "https://www.kohls.com/catalog.jsp?CN=";
		//	int counter = 1;
		//	foreach (KeyValuePair<string, string> pair in searchCriteria)
		//	{
		//		url += pair.Key + ":" + Encode(pair.Value);
		//		if (counter != searchCriteria.Count) url += "+";
		//		counter++;
		//	}
		//	url += "&PPP=120";
		//	return url;
		//}

		//private static string Encode(string theString)
		//{
		//	theString = theString.Replace(" ", "%20");
		//	theString = theString.Replace("&", "%26");
		//	theString = theString.Replace("'", "%27");
		//	return theString;
		//}
	}
}
