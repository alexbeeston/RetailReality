using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace Scrapper
{
	class Worker
	{
		private readonly IWebDriver driver;
		private readonly Seed seed;
		private readonly bool writeToConsole;
		private readonly bool writeToFile;
		private readonly WebDriverWait wait;
		private readonly StreamWriter file;
		private readonly List<SnapShot> snapShots;
		private readonly int numPagesToScrap;

		public Worker(IWebDriver driver, Seed seed, bool writeToConsole, bool writeToFile, int numPagesToScrap = 2)
		{
			this.driver = driver;
			this.seed = seed;
			this.writeToConsole = writeToConsole;
			this.writeToFile = writeToFile;
			const int sufficientlyLong = 99;
			wait = new WebDriverWait(driver, TimeSpan.FromDays(sufficientlyLong));
			snapShots = new List<SnapShot>();
			this.numPagesToScrap = numPagesToScrap;
			if (writeToFile) file = File.CreateText(@$"..\..\..\Data\data_{seed.id}.csv");
		}

		public void ProcessSeed()
		{
			int pageNumber = 1;
			driver.Navigate().GoToUrl(seed.ToUrl());
			do
			{
				var status = ProcessPage(wait);
				status.PrintReport(driver.Url, pageNumber);
				pageNumber++;
			} while (ClickNextArrow(wait) && pageNumber <= numPagesToScrap);

			if (writeToFile) file.Close();
		}

		private ScrapPageStatus ProcessPage(WebDriverWait wait)
		{
			int attempts = 0;
			var exceptions = new List<Exception>();

			return wait.Until(driver =>
			{
				if (attempts > 5) return new ScrapPageStatus(exceptions, attempts, false);

				try
				{
					var products = driver.FindElements(By.ClassName("product-description"));
					foreach (IWebElement product in products)
					{
						string id = product.GetAttribute("id");
						id = id.Remove(id.IndexOf('_'));
						if (!IdAlreadyAdded(id) && !product.Text.Contains("For Price, Add to Cart")) snapShots.Add(ParseProduct(product, id));
					}
					return new ScrapPageStatus(exceptions, attempts, true);
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

		private SnapShot ParseProduct(IWebElement product, string id)
		{
			string stars = SafeFindChildElement(product, By.ClassName("stars"))?.GetAttribute("title").Trim();
			stars = stars?.Remove(stars.IndexOf(' ')) ?? null;
			string reviews = SafeFindChildElement(product, By.ClassName("prod_ratingCount"))?.Text.TrimStart('(').TrimEnd(')') ?? null;
			string primaryLabelText = SafeFindChildElement(product, By.ClassName("prod_price_label"))?.Text ?? null;
			string primaryPriceText = SafeFindChildElement(product, By.ClassName("prod_price_amount"))?.Text ?? null;
			string alternateText = SafeFindChildElement(product, By.ClassName("prod_price_original"))?.Text ?? null;
			PriceInformant primaryPrice = PriceParsers.ParsePrimaryPrice(primaryLabelText, primaryPriceText);
			PriceInformant alternatePrice = PriceParsers.ParseAlternatePrice(alternateText);

			var snapShot = new SnapShot(
				id,
				"offerId",
				NullableStringToNullableFloat(stars),
				(int?)NullableStringToNullableFloat(reviews),
				primaryPrice,
				alternatePrice,
				DateTime.Now
			);

			if (writeToConsole) snapShot.PrintToScreen();
			if (writeToFile) snapShot.WriteToFile(file);

			return snapShot;
		}

		private static float? NullableStringToNullableFloat(string input)
		{
			if (input != null) return float.Parse(input);
			else return null;
		}

		private bool IdAlreadyAdded(string id)
		{
			foreach (SnapShot snapShot in snapShots)
			{
				if (snapShot.offerId == id)
				{
					return true;
				}
			}
			return false;
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
	}
}
