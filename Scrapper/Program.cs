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
			SetTerminateProcessOnEnter(driver);
			List<string> seeds = new List<string>
			{
				"https://www.kohls.com/catalog/mens-tops-tees-tops-clothing.jsp?CN=Gender:Mens+Product:Tops%20%26%20Tees+Category:Tops+Department:Clothing&cc=mens-TN3.0-S-shirtstees&kls_sbp=34312085268895173668447991324068535941"
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
				ProcessPage(driver, DateTime.Now.ToUniversalTime(), file);
			} while (ClickNextArrow(driver));
		}

		static void ProcessPage(IWebDriver driver, DateTime requestTime, StreamWriter file)
		{
			System.Threading.Thread.Sleep(10000); // todo: move to explicit wait
			IReadOnlyList<IWebElement> products = driver.FindElements(By.CssSelector(".product-description"));
			foreach (IWebElement product in products)
			{
				string id = product.GetAttribute("id");
				id = id.Remove(id.IndexOf('_'));
				string stars = SafeFindElement(product, ".stars")?.GetAttribute("title").Trim();
				stars = stars?.Remove(stars.IndexOf(' ')) ?? null;
				string reviews = SafeFindElement(product, ".prod_ratingCount")?.Text.TrimStart('(').TrimEnd(')') ?? null;
				string price1LabelRaw = SafeFindElement(product, ".prod_price_label")?.Text ?? null;
				string price1AmountRaw = SafeFindElement(product, ".prod_price_amount")?.Text ?? null;
				string price2Raw = product.FindElement(By.CssSelector(".prod_price_original")).Text ?? null;

				// debugging
				Console.WriteLine(id);
				Console.Write("  " + stars);
				Console.Write("  " + reviews);
				Console.Write("  " + price1LabelRaw);
				Console.Write("  " + price1AmountRaw);
				Console.WriteLine("  " + price2Raw);

				// logging
				if (logData) file.Write(id + ",");
				if (logData) file.Write(stars + ",");
				if (logData) file.Write(reviews + ",");
				if (logData) file.Write(price1LabelRaw + ",");
				if (logData) file.Write(price1AmountRaw + ",");
				if (logData) file.Write(price2Raw + "\n");
				if (logData) file.Flush();
			}
		}
	
		static IWebElement SafeFindElement(IWebElement element, string cssSelector)
		{
			try
			{
				// todo: explicit wait
				return element.FindElement(By.CssSelector(cssSelector));
			}
			catch (NoSuchElementException)
			{
				return null;
			}
		}

		static PriceInformant ProcessPrice1(string rawLabel, string rawAmount)
		{
			rawLabel = rawLabel?.Trim().ToLower() ?? null;
			Label labelType;	
			switch (rawLabel)
			{
				case "original":
					labelType = Label.Original;
					break;
				case "sale":
					labelType = Label.Sale;
					break;
				case "regular":
					labelType = Label.Regular;
					break;
				case "reg.":
					labelType = Label.Regular;
					break;
				case "clearance":
					labelType = Label.Clearance;
					break;
				case "group":
					labelType = Label.Group;
					break;
				case "":
					labelType = Label.None;
					break;
				case null:
					labelType = Label.Unknown;
					break;
				default:
					labelType = Label.Unknown;
					break;
			}

			PriceType priceType = PriceType.Hybrid;
			float amount = 10;
			if (rawAmount == null)
			{
				priceType = PriceType.Unknown;
				amount = -1;
			}
			else if (rawAmount.Contains("or"))
			{
				if (logData) file.Write($"<<<<<<< detecting a hybrid price for {rawAmount}.");
				priceType = PriceType.Hybrid;
				const int expectedElements = 2;
				string[] heyMan = rawAmount.Split("or", expectedElements);
			}
			else if (rawAmount.Contains('-'))
			{
				priceType = PriceType.Hybrid;
			}
			else
			{
				priceType = PriceType.Single;
			}

			return new PriceInformant(priceType, labelType, amount);
		}

		static PriceInformant ProcessPrice2(string rawText)
		{
			return new PriceInformant();
		}

		static bool ClickNextArrow(IWebDriver driver)
		{
			// todo: explicit wait
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

	struct PriceInformant
	{
		public PriceType type;
		public Label label;
		public float amount;

		public PriceInformant(PriceType type, Label label, float amount)
		{
			this.type = type;
			this.label = label;
			this.amount = amount;
		}
	}

}
