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
				ProcessPage(driver);
			} while (ClickNextArrow(driver));
		}

		static void ProcessPage(IWebDriver driver)
		{
			System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> stuff = driver.FindElements(By.CssSelector(".product-description"));
			foreach (IWebElement product in stuff)
			{
				string id = product.GetAttribute("id").Replace("_prod_price", "");
				Console.WriteLine("  " + id);
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
