using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using System.IO;


namespace Scrapper
{
	class Program
	{
		static void Main()
		{
			Directory.SetCurrentDirectory(@"..\..\..\");
			StreamWriter file = File.CreateText("data.csv");
			file.WriteLine("datetime,id,stars,reviews,label_1,price_1,price_2");

			IWebDriver driver = new FirefoxDriver();
			SetTerminateProcessOnEnter(driver);
			List<string> seeds = new List<string>
			{
				"https://www.kohls.com/catalog.jsp?CN=Gender:Mens+Product:Pants+Category:Bottoms+Department:Clothing",
				"https://www.kohls.com/catalog.jsp?CN=Gender:Womens+Product:Jeans+Category:Bottoms+Department:Clothing",
				"https://www.kohls.com/catalog.jsp?CN=Gender:Mens+Silhouette:Button-Down Shirts+Category:Tops+Department:Clothing",
				"https://www.kohls.com/catalog.jsp?CN=Gender:Mens+Product:Shorts+Category:Bottoms+Department:Clothing"
			};
			foreach (string seed in seeds) ProcessSeed(seed, driver, file);
			driver.Quit();
		}

		static void ProcessSeed(string seed, IWebDriver driver, StreamWriter file)
		{
			driver.Navigate().GoToUrl(seed);
			do
			{
				Console.WriteLine("New Page");
				ProcessPage(driver, DateTime.Now.ToUniversalTime(), file, true);
			} while (ClickNextArrow(driver));
		}

		static void ProcessPage(IWebDriver driver, DateTime requestTime, StreamWriter file, bool write = false)
		{
			Console.WriteLine("Waiting...");
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
			Console.WriteLine("Done Waiting");
			IReadOnlyList<IWebElement> products = driver.FindElements(By.CssSelector(".product-description"));
			Console.Write($"Length of products: {products.Count}");
			foreach (IWebElement product in products)
			{
				//file.Write(requestTime.ToLocalTime().ToString() + ",");

				// product ID
				string id;
				try
				{
					id = product.GetAttribute("id");
					id = id.Remove(id.IndexOf('_'));	
				}
				catch (Exception e)
				{
					id = e.ToString();
				}
				//file.Write(id + ",");

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
				catch (Exception e)
				{
					stars = e.ToString();
				}
				//file.Write(stars + ",");

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
				catch (Exception e)
				{
					reviews = e.ToString();
				}
				//file.Write(reviews + ",");

				// label 1
				string firstLabel = product.FindElement(By.CssSelector(".prod_price_label")).Text;
				//file.Write(firstLabel + ",");

				// price 1
				string firstPrice = product.FindElement(By.CssSelector(".prod_price_amount")).Text;
				//file.Write(firstPrice + ",");

				// price 2
				string secondPrice = product.FindElement(By.CssSelector(".prod_price_original")).Text;
				//file.Write(secondPrice + "\n");
				//file.Flush();

				// debugging
				if (write)
				{
					Console.WriteLine(id);
					Console.Write("  " + stars);
					Console.Write("  " + reviews);
					Console.Write("  " + firstLabel);
					Console.Write("  " + firstPrice);
					Console.WriteLine("  " + secondPrice);
				}
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
				Console.WriteLine("Terminating process");
				driver.Quit();
				Console.WriteLine("Just quit the driver");
				Environment.Exit(0);
			});
		}
	}
}
