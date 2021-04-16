﻿using System;
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
			List<Seed> seeds = LoadConfigurations.GetSeeds().FindAll(x => (x.id > 9 && x.id < 13));
			bool doAsync = false;
			if (doAsync) await DoMainAsync(seeds);
			else DoMainSerial(seeds);
		}

		static void DoMainSerial(List<Seed> seeds)
		{
			IWebDriver driver = new ChromeDriver();
			foreach (Seed seed in seeds)
			{
				Worker worker = new Worker(driver, seed, true);
				worker.ProcessSeed();
			}
		}

		static async Task DoMainAsync(List<Seed> seeds)
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
						Worker worker = new Worker(driver, seeds[tasksStarted], true);
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
