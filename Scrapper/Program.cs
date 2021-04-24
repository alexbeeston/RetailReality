using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Scrapper
{
	class Program
	{
		static async Task Main()
		{
			// configuration
			bool doAsync = false;
			bool writeToConsole = false;
			bool writeToFile = true;

			// code
			List<Seed> seeds = Miscellaneous.GetSeeds().GetRange(0, 3);
			if (doAsync) await DoMainAsync(seeds, writeToConsole, writeToFile);
			else DoMainSerial(seeds, writeToConsole, writeToFile);
		}

		static void DoMainSerial(List<Seed> seeds, bool writeToConsole, bool writeToFile)
		{
			IWebDriver driver = new ChromeDriver();
			foreach (Seed seed in seeds)
			{
				Worker worker = new Worker(driver, seed, writeToConsole, writeToFile);
				worker.ProcessSeed();
			}
			driver.Quit();
		}

		static async Task DoMainAsync(List<Seed> seeds, bool writeToConsole, bool writeToFile)
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
						Worker worker = new Worker(driver, seeds[tasksStarted], writeToConsole, writeToFile);
						worker.ProcessSeed();
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
