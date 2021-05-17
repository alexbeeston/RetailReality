using System;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace Scrapper
{
	class Program
	{
		static async Task Main()
		{
			var configs = GetConfigs();
			if (configs.isDataBaseDevEnv)
			{
				DataBaseDev(configs);
			}
			else
			{
				List<Seed> seeds = GenerateSeeds(configs.combinations, configs.pairs);
				if (configs.doAsync) await DoMainAsync(seeds, configs);
				else DoMainSerial(seeds, configs);
			}
		}

		static Configurations GetConfigs()
		{
			var configs = JsonConvert.DeserializeObject<Configurations>(File.ReadAllText(@"..\..\..\configurations.json"));
			Console.WriteLine($"Enter password for MySql user {configs.mySqlUserName}:");
			configs.mySqlPassword = Console.ReadLine();
			return configs;
		}


		static void DoMainSerial(List<Seed> seeds, Configurations configs)
		{
			IWebDriver driver = new ChromeDriver();

			foreach (Seed seed in seeds)
			{
				Worker worker = new Worker(driver, seed, configs);
				worker.GetOffers();
				worker.FlushOffers();
			}
			driver.Quit();
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

		static void DataBaseDev(Configurations configs)
		{
			var offers = JsonConvert.DeserializeObject<List<Offer>>(File.ReadAllText(@"..\..\..\Data\serializations\sample1.json"));
			var worker = new Worker(configs, offers);
			worker.FlushOffers();
			return;
		}

		static async Task DoMainAsync(List<Seed> seeds, Configurations configs)
		{
			Task[] tasks = new Task[seeds.Count];
			HttpClient client = new HttpClient();
			ChromeOptions options = new ChromeOptions();

			int tasksStarted = 0;
			while (tasksStarted < seeds.Count)
			{
				int nodesAvailable;
				while ((nodesAvailable = await NodeAvailable(client)) == 0) Thread.Sleep(30000);
				Console.WriteLine($"Going to allocate {nodesAvailable} nodes.");

				for (int i = 0; i < nodesAvailable && tasksStarted < seeds.Count; i++)
				{
					IWebDriver driver = new RemoteWebDriver(new Uri("http://192.168.1.3:4444/wd/hub"), options);
					tasks[tasksStarted] = Task.Run(() =>
					{
						var worker = new Worker(driver, seeds[tasksStarted], configs);
						worker.GetOffers();
						driver.Quit();
					});
					Console.WriteLine($"{tasksStarted} tasks have been started");
					tasksStarted++;
				}
			}
			Console.WriteLine("Everything started");
			Task.WaitAll(tasks);
		}

		static async Task<int> NodeAvailable(HttpClient client)
		{
			HttpResponseMessage response = await client.GetAsync("http://localhost:4444/grid/api/hub");
			response.EnsureSuccessStatusCode();
			string body = await response.Content.ReadAsStringAsync();
			JObject json = JObject.Parse(body);
			return int.Parse(json.SelectToken("slotCounts").SelectToken("free").ToString());
		}
	}
}
