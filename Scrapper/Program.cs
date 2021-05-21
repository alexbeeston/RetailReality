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
			if (configs.skipToFlushOffers)
			{
				var offers = JsonConvert.DeserializeObject<List<Offer>>(File.ReadAllText(@"..\..\..\Data\serializations\sample1.json"));
				var worker = new Worker(configs, offers);
				worker.FlushOffers();
			}
			else
			{
				List<Seed> seeds = GenerateSeeds(configs.combinations, configs.pairs);
				IWebDriver driver = new ChromeDriver();

				foreach (Seed seed in seeds)
				{
					Worker worker = new Worker(driver, seed, configs);
					worker.GetOffers();
					worker.LogOffers();
					// worker.LogScrapReport();
					worker.FlushOffers();
				}
				driver.Quit();
			}
		}

		static Configurations GetConfigs()
		{
			var configs = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(@"..\..\..\configurations.json"));
			Console.WriteLine($"Enter password for MySql user {configs.mySqlUserName}:");
			configs.mySqlPassword = Console.ReadLine();
			return configs;
		}

		public static List<Seed> GenerateSeeds(List<Combination> combinations, List<Pair> pairs)
		{
			var seeds = new List<Seed>();
			foreach (Combination combination in combinations)
			{
				var seed = new Seed(combination.id);
				foreach (int pairId in combination.include)
				{
					var pair = pairs.Find(x => x.id == pairId);
					seed.searchCriteria.Add(pair.key, pair.value);
				}
				seeds.Add(seed);
			}
			var random = new Random();
			return seeds.OrderBy(x => random.Next()).ToList();
		}
	}
}
