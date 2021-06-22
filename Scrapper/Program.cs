using System;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using Newtonsoft.Json;
using DataStore;

namespace Scrapper
{
	class Program
	{
		public static void Main()
		{
			Logging.InitLogging();
			var configs = GetConfigs();
			if (configs.executionPreferences.skipToFlushOffers)
			{
				string pathToSerializedFile;
				if (configs.executionPreferences.pickRandomSerialization)
				{
					var pathToSerializedOffers = Directory.GetFiles(@"..\..\..\Data\serializations").ToList();
					var random = new Random();
					pathToSerializedFile = pathToSerializedOffers.OrderBy(x => random.Next()).ToList().First();
				}
				else
				{
					pathToSerializedFile = @"..\..\..\Data\serializations\" + configs.executionPreferences.serializationToUse;
				}

				string statusMessage = $"Skipping to 'flush offers' for {pathToSerializedFile}";
				Console.WriteLine(statusMessage);
				Logging.Info(statusMessage);

				var offers = JsonConvert.DeserializeObject<List<Offer>>(File.ReadAllText(pathToSerializedFile));
				var worker = new Worker(configs, offers);
				worker.FlushOffers();
			}
			else
			{
				string statusMessage = "Starting to scrap pages";
				Console.WriteLine(statusMessage);
				Logging.Info(statusMessage);

				IWebDriver driver;
				try
				{
					driver = new ChromeDriver();
				}
				catch (Exception e)
				{
					Logging.Error($"Could not instantiate Chrome driver (returning): {e}");
					return;
				}
				int counter = 1;
				foreach (var searchCriteria in configs.searchCriterion)
				{
					if (configs.executionPreferences.maxSeedsToScrap != -1 && counter > configs.executionPreferences.maxSeedsToScrap) break;
					Worker worker = new Worker(driver, searchCriteria, configs.executionPreferences);
					Logging.Info($"Processing search criteria {searchCriteria.id} ({searchCriteria})");
					worker.GetOffers();
					worker.LogOffers();
					worker.FlushOffers();
					counter++;
				}
				driver.Quit();
				Logging.Info("Successfully quite Chrome driver. Exiting program.");
			}
		}

		static Configurations GetConfigs()
		{
			var configs = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(@"..\..\..\configurations.json"));
			Console.WriteLine($"Enter password for MySql user {configs.executionPreferences.mySqlUserName}:");
			configs.executionPreferences.mySqlPassword = Console.ReadLine();
			if (configs.executionPreferences.randomizeSeeds)
			{
				var random = new Random();
				configs.searchCriterion = configs.searchCriterion.OrderBy(x => random.Next()).ToList();
			}
			return configs;
		}
	}
}
