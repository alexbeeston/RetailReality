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
			if (logData) file = File.CreateText(@"..\..\..\data.csv");
			if (logData) file.WriteLine("id,stars,reviews,firstLabel,firstPrice,secondPrice");

			var options = new ChromeOptions();
			//options.AddArgument("--headless");
			IWebDriver driver = new ChromeDriver(options);
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
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

			foreach (Seed seed in seeds.GetRange(4, 5))
			{
				ProcessSeed(driver, wait, seed);
			}
			driver.Quit();
			if (logData) file.Close();
		}

		static void ProcessSeed(IWebDriver driver, WebDriverWait wait, Seed seed)
		{
			int pageNumber = 1;
			driver.Navigate().GoToUrl(seed.ToUrl());
			do
			{
				ProcessPage(driver, wait, seed.id, pageNumber);
				Console.WriteLine();
				pageNumber++;
			} while (ClickNextArrow(wait));
		}

		static void ProcessPage(IWebDriver driver, WebDriverWait wait, int seedNumber, int pageNumber, int attempts = 1, List<string> idsAlreadyProcessed = null)
		{
			if (idsAlreadyProcessed == null) idsAlreadyProcessed = new List<string>();
			const int MAX_ATTEMPTS = 5;
			if (attempts > MAX_ATTEMPTS)
			{
				Console.WriteLine($"Couldn't avoid the stale element exception in fewer than {MAX_ATTEMPTS} attempts for url {driver.Url}, which was page {pageNumber} of for seed {seedNumber}.");
				return;
			}

			Console.WriteLine($"***Processing seed {seedNumber} page {pageNumber} on attempt {attempts}.***");
			var products = wait.Until(e =>
			{
				try
				{
					// verify length of found elements?
					return e.FindElements(By.ClassName("product-description"));
				}
				catch
				{
					return null;
				}
			});
			foreach (IWebElement product in products)
			{
				try
				{
					string id = product.GetAttribute("id");
					id = id.Remove(id.IndexOf('_'));

					if (!idsAlreadyProcessed.Contains(id) && !product.Text.Contains("For Price, Add to Cart"))
					{
						string stars = SafeFindChildElement(product, By.ClassName("stars"))?.GetAttribute("title").Trim();
						stars = stars?.Remove(stars.IndexOf(' ')) ?? null;
						string reviews = SafeFindChildElement(product, By.ClassName("prod_ratingCount"))?.Text.TrimStart('(').TrimEnd(')') ?? null;
						string primaryLabelText = SafeFindChildElement(product, By.ClassName("prod_price_label"))?.Text ?? null;
						string primaryPriceText = SafeFindChildElement(product, By.ClassName("prod_price_amount"))?.Text ?? null;
						string alternateText = SafeFindChildElement(product, By.ClassName("prod_price_original"))?.Text ?? null;
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
					ProcessPage(driver, wait, seedNumber, pageNumber, ++attempts, idsAlreadyProcessed);
					return;
				}
			}
		}

		static IWebElement SafeFindChildElement(IWebElement element, By locator)
		{
			try
			{
				return element.FindElement(locator);
			}
			catch
			{
				return null;
			}
		}

		static IWebElement SafeFindElement(WebDriverWait wait, By locator)
		{
			return wait.Until(driver =>
			{
				try
				{
					return driver.FindElement(locator);
				}
				catch
				{
					return null;
				}
			});
		}

		static bool ClickNextArrow(WebDriverWait wait)
		{
			IWebElement nextArrow = SafeFindElement(wait, By.ClassName("nextArw"));
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
