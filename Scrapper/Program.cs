using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace Scrapper
{
	class Program
	{
		const int WAIT_TIME = 15000;
		static void Main()
		{
			IWebDriver driver = new FirefoxDriver();
			string sampleSeed = "https://www.kohls.com/catalog.jsp?CN=Gender:Mens+Silhouette:Button-Down Shirts+Category:Tops+Department:Clothing";
			List<string> seeds = new List<string>
			{
				sampleSeed
			};
			foreach (string seed in seeds) ProcessSeed(seed, driver);
		}

		static void ProcessSeed(string seed, IWebDriver driver)
		{
			driver.Navigate().GoToUrl(seed);
			do
			{
				Console.WriteLine("New Page");
				ProcessPage(driver, DateTime.Now.ToUniversalTime());
			} while (ClickNextArrow(driver));
		}

		static void ProcessPage(IWebDriver driver, DateTime requestTime)
		{
			IReadOnlyList<IWebElement> products = driver.FindElements(By.CssSelector(".product-description"));
			foreach (IWebElement product in products)
			{
				// product ID
				string id = product.GetAttribute("id");
				id = id.Remove(id.IndexOf('_'));	
				Console.WriteLine("  " + id);

				// stars
				string stars;
				try
				{
					stars = product.FindElement(By.CssSelector(".stars")).GetAttribute("title").Trim();
					stars = stars.Remove(stars.IndexOf(' '));
				}
				catch (NoSuchElementException)
				{
					stars = "-1";
				}
				Console.WriteLine(stars);

				// reviews
				string reviews;
				try
				{
					reviews = product.FindElement(By.CssSelector(".prod_ratingCount")).Text.TrimStart('(').TrimEnd(')');
				}
				catch (NoSuchElementException)
				{
					reviews = "-1";
				}
				Console.WriteLine(reviews);

				// first label
				string firstLabel = product.FindElement(By.CssSelector(".prod_price_label")).Text;
				Console.WriteLine(firstLabel);

				// first price
				string firstPrice = product.FindElement(By.CssSelector(".prod_price_amount")).Text;
				Console.WriteLine(firstPrice);

				// second price
				string secondPrice = product.FindElement(By.CssSelector(".prod_price_original")).Text;
				Console.WriteLine(secondPrice);

			}
		}

		static bool ClickNextArrow(IWebDriver driver)
		{
			IWebElement nextArrow = driver.FindElement(By.CssSelector(".nextArw"));
			if (nextArrow.Displayed)
			{
				nextArrow.Click();
				return true;
			}
			else return false;
		}
	}
}
