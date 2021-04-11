using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using System.IO;

namespace Scrapper
{
	class Program
	{
		static readonly bool logData = true;
		static StreamWriter file;

		static void printDictionary(Dictionary<string, string> pairs)
		{
			foreach (var pair in pairs)
			{
				Console.WriteLine($"{pair.Key}:  { pair.Value}");
			}
		}

		static void Main()
		{
			if (logData) Directory.SetCurrentDirectory(@"..\..\..\");
			if (logData) file = File.CreateText("data.csv");
			if (logData) file.WriteLine("id,stars,reviews,firstLabel,firstPrice,secondPrice");

			var options = new ChromeOptions();
			//options.AddArgument("--headless");
			IWebDriver driver = new ChromeDriver(options);
			SetTerminateProcessOnEnter(driver);

			List<Seed> seeds = LoadConfigurations.GetSeeds();
			//var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
			//foreach (Seed seed in seeds)
			//{
			//	driver.Navigate().GoToUrl(seed.ToUrl());
			//	printDictionary(seed.pairs);
			//	string counts = wait.Until<string>(e =>
			//	{
			//		try
			//		{
			//			return e.FindElement(By.CssSelector(".result_count")).Text;
			//		}
			//		catch
			//		{
			//			return null;
			//		}
			//	});
			//	Console.WriteLine($"Number of results: {counts}\n");
			//}
			//return;

			foreach (Seed seed in seeds)
			{
				ProcessSeed(seed, driver);
			}
			driver.Quit();
			if (logData) file.Close();
		}

		static void ProcessSeed(Seed seed, IWebDriver driver)
		{
			int pageNumber = 1;
			driver.Navigate().GoToUrl(seed.ToUrl());
			int initialWait = 3000;
			do
			{
				ProcessPage(driver, seed.id, pageNumber, initialWait);
				Console.WriteLine();
				pageNumber++;
			} while (ClickNextArrow(driver));
		}

		static void ProcessPage(IWebDriver driver, int seedNumber, int pageNumber, int waitTime = 0, int attempts = 1, List<string> idsAlreadyProcessed = null)
		{
			if (idsAlreadyProcessed == null) idsAlreadyProcessed = new List<string>();
			const int MAX_ATTEMPTS = 5;
			if (attempts > MAX_ATTEMPTS)
			{
				Console.WriteLine($"Couldn't avoid the stale element exception in fewer than {MAX_ATTEMPTS} attempts for url {driver.Url}, which was page {pageNumber} of for seed {seedNumber}. Final wait was {waitTime} milliseconds.");
				return;
			}

			Console.WriteLine($"***Processing seed {seedNumber} page {pageNumber} on attempt {attempts}.***");
			System.Threading.Thread.Sleep(waitTime);
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitTime * 1000));
			IReadOnlyList<IWebElement> products = wait.Until(e => e.FindElements(By.CssSelector(".product-description"))); // could do a safe -find that waits until the length of this list is 48
			foreach (IWebElement product in products)
			{
				try
				{
					string id = null;
					id = product.GetAttribute("id");
					id = id.Remove(id.IndexOf('_'));

					if (!idsAlreadyProcessed.Contains(id) && !product.Text.Contains("For Price, Add to Cart"))
					{
						string stars = SafeFindElement(product, ".stars")?.GetAttribute("title").Trim();
						stars = stars?.Remove(stars.IndexOf(' ')) ?? null;
						string reviews = SafeFindElement(product, ".prod_ratingCount")?.Text.TrimStart('(').TrimEnd(')') ?? null;
						string primaryLabelText = SafeFindElement(product, ".prod_price_label")?.Text ?? null;
						string primaryPriceText = SafeFindElement(product, ".prod_price_amount")?.Text ?? null;
						string alternateText = product.FindElement(By.CssSelector(".prod_price_original")).Text ?? null;
						PriceInformant primaryPrice = PrimaryPriceParser.ParsePrimaryPrice(primaryLabelText, primaryPriceText);
						PriceInformant alternatePrice = AlternatePriceParser.ParseAlternatePrice(alternateText);

						//alternatePrice.Validate();
						//alternatePrice.DataBaseCom();
						//primaryPrice.Validate();
						//primaryPrice.DataBaseCom();

						if (logData) file.Write(id + ",");
						if (logData) file.Write(stars + ",");
						if (logData) file.Write(reviews + ",");
						if (logData) file.Write(primaryLabelText + ",");
						if (logData) file.Write(primaryPriceText + ",");
						if (logData) file.Write(alternateText + "\n");
						if (logData) file.Flush();

						Console.WriteLine($"id: {id}");
						Console.WriteLine($"stars: {stars}");
						Console.WriteLine($"reviews: {reviews}");
						Console.WriteLine($"price 1: {primaryPrice.individualPrice}");
						Console.WriteLine($"price 2: {alternatePrice.individualPrice}");

						idsAlreadyProcessed.Add(id);
					}
				}
				catch (StaleElementReferenceException)
				{
					driver.Navigate().Refresh();
					const int WAIT_TIME_INCREASE = 8000;
					waitTime += WAIT_TIME_INCREASE;
					ProcessPage(driver, seedNumber, pageNumber, waitTime, ++attempts, idsAlreadyProcessed);
					return;
				}
			}
		}

		static IWebElement SafeFindElement(IWebElement element, string cssSelector)
		{
			// do an explicit wait here. Yes, I'll have to pass in the driver to the lamdba, but who says I have to use it if the element is in scope? // could do a safe -find that waits until the length of this list is 48
			try
			{
				return element.FindElement(By.CssSelector(cssSelector));
			}
			catch (NoSuchElementException)
			{
				return null;
			}
		}

		static bool ClickNextArrow(IWebDriver driver)
		{
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			IWebElement nextArrow = wait.Until(e => e.FindElement(By.CssSelector(".nextArw")));
			if (nextArrow.Displayed)
			{
				nextArrow.Click();
				return true;
			}
			else return false;
		}
		
		static void SetTerminateProcessOnEnter(IWebDriver driver)
		{
			Task.Run(() => {
				Console.ReadLine();
				Console.WriteLine("Terminating process...");
				driver.Quit();
				Console.WriteLine("Driver Quit.");
				Environment.Exit(0);
				if (logData) file.Close();
			});
		}
	}
}
