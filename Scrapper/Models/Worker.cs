using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;
using Newtonsoft.Json;

namespace Scrapper
{
	class Worker
	{
		private readonly IWebDriver driver;
		private readonly Seed seed;
		private readonly WorkerConfiguration configs;

		private readonly WebDriverWait infinateWait;
		private readonly WebDriverWait shortWait;
		private readonly StreamWriter file;
		private readonly List<Offer> offers;

		public Worker(IWebDriver driver, Seed seed, WorkerConfiguration configs)
		{
			this.driver = driver;
			this.seed = seed;
			this.configs = configs;

			const int sufficientlyLong = 99;
			infinateWait = new WebDriverWait(driver, TimeSpan.FromDays(sufficientlyLong));
			shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
			offers = new List<Offer>();
			if (configs.logOffersToCsv) file = File.CreateText(@$"..\..\..\Data\csv\data_{seed.id}.csv");
		}


		public void GetOffers()
		{
			int pageNumber = 1;
			driver.Navigate().GoToUrl(seed.ToUrl());
			do
			{
				var status = ScrapPage();
				if (configs.writeScrapResultsToStatus) status.PrintReport(seed.id, driver.Url, pageNumber);
				pageNumber++;
			} while (ClickNextArrow(infinateWait) && (configs.pagesToScrap == -1 || pageNumber <= configs.pagesToScrap));

			if (configs.logOffersToCsv) file.Close();
		}


		private ScrapResults ScrapPage()
		{
			int attempts = 0;
			var exceptions = new List<Exception>();

			return infinateWait.Until(driver =>
			{
				if (attempts > 5) return new ScrapResults(exceptions, attempts, false);

				try
				{
					var products = GetProducts();
					foreach (IWebElement product in products)
					{
						string id = product.GetAttribute("id");
						id = id.Remove(id.IndexOf('_'));
						if (!offers.Exists(x => x.id == id) && !product.Text.Contains("For Price, Add to Cart")) offers.Add(BuildOfferFromHtmlElement(product, id));
					}
					return new ScrapResults(exceptions, attempts, true);
				}
				catch (Exception e)
				{
					exceptions.Add(e);
					attempts++;
					driver.Navigate().Refresh();
					return null;
				}
			});
		}

		
		private ReadOnlyCollection<IWebElement> GetProducts()
		{
			return shortWait.Until(driver => // validate on the number of products expected?
			{
				try
				{
					var products = driver.FindElements(By.ClassName("product-description"));
					foreach (var product in products)
					{
						if (product.Text.Contains("Reg.")) // if the product's price tag says "Reg.", the page has not fully finished loading
						{
							Console.WriteLine("Caught a \"Reg.\"");
							return null;
						}
					}
					return products;
				}
				catch
				{
					return null;
				}
			});
		}

		private Offer BuildOfferFromHtmlElement(IWebElement product, string id)
		{
			string stars = SafeFindChildElement(product, By.ClassName("stars"))?.GetAttribute("title").Trim();
			stars = stars?.Remove(stars.IndexOf(' ')) ?? null;
			string reviews = SafeFindChildElement(product, By.ClassName("prod_ratingCount"))?.Text.TrimStart('(').TrimEnd(')') ?? null;
			string primaryLabelText = SafeFindChildElement(product, By.ClassName("prod_price_label"))?.Text ?? null;
			string primaryPriceText = SafeFindChildElement(product, By.ClassName("prod_price_amount"))?.Text ?? null;
			string alternateText = SafeFindChildElement(product, By.ClassName("prod_price_original"))?.Text ?? null;
			PriceInformant primaryPrice = PriceParsers.ParsePrimaryPrice(primaryLabelText, primaryPriceText);
			PriceInformant alternatePrice = PriceParsers.ParseAlternatePrice(alternateText);

			var offer = new Offer(
				new Product(id, "name", seed.searchCriteria),
				"figure out how to generate an offer ID; use database?",
				NullableStringToNullableFloat(stars),
				(int?)NullableStringToNullableFloat(reviews),
				primaryPrice,
				alternatePrice,
				DateTime.Now
			);

			if (configs.writeOffersToConsole) offer.WriteToConsole();
			if (configs.logOffersToCsv) offer.LogToFile(file);

			return offer;
		}

		private static float? NullableStringToNullableFloat(string input)
		{
			if (input != null) return float.Parse(input);
			else return null;
		}

		private static bool ClickNextArrow(WebDriverWait wait)
		{
			IWebElement nextArrow = SafeFindElement(wait, By.ClassName("nextArw"));
			if (nextArrow.Displayed)
			{
				nextArrow.Click();
				return true;
			}
			else return false;
		}
		
		private static IWebElement SafeFindChildElement(IWebElement element, By locator)
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

		private	static IWebElement SafeFindElement(WebDriverWait wait, By locator)
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

		/// <summary>
		/// Used in dev only; can be deleted when ready for release 
		/// </summary>
		public void SeralizeOffers()
		{
			if (offers.Count == 0) throw new Exception("Attempting to serialize offers, but no offers have been parsed.");

			var serializedFile = File.CreateText($@"..\..\..\Data\serializations\{seed.PairsToString()}.txt");
			serializedFile.Write(JsonConvert.SerializeObject(offers, Formatting.Indented));
			serializedFile.Close();
		}
	}
}
