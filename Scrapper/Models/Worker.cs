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

		public Worker(IWebDriver driver, Seed seed, bool writeToConsole, bool writeToFile)
		{
			this.driver = driver;
			this.seed = seed;
			this.writeToConsole = writeToConsole;
			this.writeToFile = writeToFile;
			wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
			snapShots = new List<SnapShot>();
			if (writeToFile) file = File.CreateText(@$"..\..\..\Data\data_{seed.id}.csv");
		}

		public void ProcessSeed()
		{
			int pageNumber = 1;
			driver.Navigate().GoToUrl(seed.ToUrl());
			do
			{
				ProcessPage(driver, wait, seed.id, pageNumber);
				Console.WriteLine();
				pageNumber++;
			} while (ClickNextArrow(wait));
			file.Close();
		}

		private void ProcessPage(IWebDriver driver, WebDriverWait wait, int seedNumber, int pageNumber, int attempts = 1)
		{
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
					// put this in the until (???) Yes... maybe in the catch I can just refresh the page, and in the try put the brakes somehow
					string id = product.GetAttribute("id");
					id = id.Remove(id.IndexOf('_'));

					if (!IdAlreadyAdded(id) && !product.Text.Contains("For Price, Add to Cart"))
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
						snapShots.Add(snapShot);
					}
				}
				catch (StaleElementReferenceException)
				{
					driver.Navigate().Refresh();
					ProcessPage(driver, wait, seedNumber, pageNumber, ++attempts);
					return;
				}
			}

			if (file != null) file.Close();
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
