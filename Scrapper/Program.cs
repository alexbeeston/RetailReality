using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI; // used for selection drop-downs (?)
using System.Threading.Tasks;
using System.IO;


namespace Scrapper
{
	class Program
	{
		static readonly bool logData = true;
		static StreamWriter file;

		static void Main()
		{
			if (logData) Directory.SetCurrentDirectory(@"..\..\..\");
			if (logData) file = File.CreateText("data.csv");
			if (logData) file.WriteLine("id,stars,reviews,firstLabel,firstPrice,secondPrice");

			IWebDriver driver = new ChromeDriver();
			driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
			SetTerminateProcessOnEnter(driver);
			List<string> seeds = new List<string>
			{
				//"https://www.kohls.com/catalog.jsp?CN=Gender:Mens+Product:Pants+Category:Bottoms+Department:Clothing",
				//"https://www.kohls.com/catalog.jsp?CN=Gender:Womens+Product:Jeans+Category:Bottoms+Department:Clothing",
				//"https://www.kohls.com/catalog.jsp?CN=Gender:Mens+Silhouette:Button-Down Shirts+Category:Tops+Department:Clothing",
				"https://www.kohls.com/catalog.jsp?CN=Gender:Mens+Product:Shorts+Category:Bottoms+Department:Clothing"
			};
			foreach (string seed in seeds) ProcessSeed(seed, driver, file);
			driver.Quit();
			if (logData) file.Close();
		}

		static void ProcessSeed(string seed, IWebDriver driver, StreamWriter file)
		{
			driver.Navigate().GoToUrl(seed);
			do
			{
				ProcessPage(driver, DateTime.Now.ToUniversalTime(), file, true);
			} while (ClickNextArrow(driver));
		}

		static void ProcessPage(IWebDriver driver, DateTime requestTime, StreamWriter file, bool write = false)
		{
			IReadOnlyList<IWebElement> products = driver.FindElements(By.CssSelector(".product-description"));
			string nullIndicator = "null";
			foreach (IWebElement product in products)
			{
				string id = product.GetAttribute("id");
				id = id.Remove(id.IndexOf('_'));

				string stars = SafeFindElement(product, ".stars")?.GetAttribute("title").Trim();
				stars = stars?.Remove(stars.IndexOf(' ')) ?? nullIndicator;

				string reviews = SafeFindElement(product, ".prod_ratingCount")?.Text.TrimStart('(').TrimEnd(')') ?? nullIndicator;

				string firstLabel = SafeFindElement(product, ".prod_price_label")?.Text ?? nullIndicator;

				string firstPrice = SafeFindElement(product, ".prod_price_amount")?.Text ?? nullIndicator;

				string secondPrice = product.FindElement(By.CssSelector(".prod_price_original")).Text ?? nullIndicator;

				// debugging
				Console.WriteLine(id);
				Console.Write("  " + stars);
				Console.Write("  " + reviews);
				Console.Write("  " + firstLabel);
				Console.Write("  " + firstPrice);
				Console.WriteLine("  " + secondPrice);

				// logging
				if (logData) file.Write(id + ",");
				if (logData) file.Write(stars + ",");
				if (logData) file.Write(reviews + ",");
				if (logData) file.Write(firstLabel + ",");
				if (logData) file.Write(firstPrice + ",");
				if (logData) file.Write(secondPrice + "\n");
				if (logData) file.Flush();
			}
		}
		
		static IWebElement SafeFindElement(IWebElement element, string cssSelector)
		{
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
			IWebElement nextArrow = driver.FindElement(By.CssSelector(".nextArw"));
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
