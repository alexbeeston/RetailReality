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
			List<Seed> seeds = LoadConfigurations.GetSeeds();
			Task[] tasks = new Task[seeds.Count];
			HttpClient client = new HttpClient();
			ChromeOptions options = new ChromeOptions();

			for (int i = 0; i < seeds.Count; i++)
			{
				bool nodeAvailable;
				int numPings = 0;
				while (!(nodeAvailable = await NodeAvailable(client)))
				{
					numPings++;
					Thread.Sleep(30000);
				}

				IWebDriver driver = new RemoteWebDriver(new Uri("http://192.168.1.3:4444/wd/hub"), options);
				return;
				tasks[i] = Task.Run(() =>
				{
					Worker worker = new Worker(driver, seeds[i], true);
					worker.ProcessSeed();
					driver.Quit();
				});
			}
			Task.WaitAll(tasks);
		}

		static async Task<bool> NodeAvailable(HttpClient client)
		{
			HttpResponseMessage response = await client.GetAsync("http://localhost:4444/grid/api/hub");
			response.EnsureSuccessStatusCode();
			string body = await response.Content.ReadAsStringAsync();
			JObject json = JObject.Parse(body);
			var numNodes = int.Parse(json.SelectToken("slotCounts").SelectToken("free").ToString());
			return numNodes > 0;
		}
	}
}
