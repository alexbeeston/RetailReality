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
				// shows hybrid price types
				// "https://www.kohls.com/catalog/mens-tops-tees-tops-clothing.jsp?CN=Gender:Mens+Product:Tops%20%26%20Tees+Category:Tops+Department:Clothing&cc=mens-TN3.0-S-shirtstees&kls_sbp=34312085268895173668447991324068535941",

				// has been tough with stale elements
				// have added all price types to composite data
				 "https://www.kohls.com/catalog/womens-casual-athletic-shoes-sneakers-shoes.jsp?CN=Gender:Womens+Occasion:Casual+Product:Athletic%20Shoes%20%26%20Sneakers+Department:Shoes&icid=sh-a-womenscasualsneakers&kls_sbp=34312085268895173668447991324068535941"

				// men's jeans; typical price informants
				// "https://www.kohls.com/catalog/mens-bottoms-jeans-clothing.jsp?CN=Gender:Mens+Category:Bottoms+Product:Jeans+Department:Clothing&cc=mens-LN3.0-S-jeans&kls_sbp=13815131106824211630787049671079564736"

				// women's dresses
				//"https://www.kohls.com/catalog/womens-dresses-clothing.jsp?CN=Gender:Womens+Category:Dresses+Department:Clothing&cc=wms-TN3.0-S-dresses&kls_sbp=13815131106824211630787049671079564736"
			};
			int seedNumber = 1;
			foreach (string seed in seeds)
			{
				ProcessSeed(seed, seedNumber, driver);
				seedNumber++;
			}
			driver.Quit();
			if (logData) file.Close();
		}

		static void ProcessSeed(string seed, int seedNumber, IWebDriver driver)
		{
			int pageNumber = 1;
			driver.Navigate().GoToUrl(seed);
			int initialWait = 3000;
			do
			{
				ProcessPage(driver, seedNumber, pageNumber, initialWait);
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

			Console.WriteLine($"Processing seed {seedNumber} page {pageNumber} on attempt {attempts}.");
			System.Threading.Thread.Sleep(waitTime);
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitTime * 1000));
			IReadOnlyList<IWebElement> products = wait.Until(e => e.FindElements(By.CssSelector(".product-description")));
			foreach (IWebElement product in products)
			{
				try
				{
					string id = null;
					id = product.GetAttribute("id");
					id = id.Remove(id.IndexOf('_'));
					string stars = SafeFindElement(product, ".stars")?.GetAttribute("title").Trim();
					stars = stars?.Remove(stars.IndexOf(' ')) ?? null;
					string reviews = SafeFindElement(product, ".prod_ratingCount")?.Text.TrimStart('(').TrimEnd(')') ?? null;
					string price1LabelRaw = SafeFindElement(product, ".prod_price_label")?.Text ?? null;
					string price1AmountRaw = SafeFindElement(product, ".prod_price_amount")?.Text ?? null;
					string price2Raw = product.FindElement(By.CssSelector(".prod_price_original")).Text ?? null;

					//PriceInformant price2 = ProcessPrice2(price2Raw);

					if (logData) file.Write(id + ",");
					if (logData) file.Write(stars + ",");
					if (logData) file.Write(reviews + ",");
					if (logData) file.Write(price1LabelRaw + ",");
					if (logData) file.Write(price1AmountRaw + ",");
					if (logData) file.Write(price2Raw + "\n");
					if (logData) file.Flush();

					if (!idsAlreadyProcessed.Contains(id)) // move before we try to parse everything else
					{
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
			try
			{
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
			string original = (string)rawText.Clone();
			PriceInformant informat = new PriceInformant();
			if (rawText.Contains("Original"))
			{
				informat.label = Label.Original;
				if (rawText.Contains("-")) // matches "Original $26.00 - $30.00"
				{
					informat.type = PriceType.Range;
					rawText = rawText.Remove(0, rawText.IndexOf("$"));

				}
				else // matches "or Original $15.00 each" and "Original $20.00"
				{
					informat.type = PriceType.Single;
					rawText = rawText.Remove(0, rawText.IndexOf("$"));
					if (rawText.Contains("each")) rawText = rawText.Remove(rawText.IndexOf(" "));
					rawText = rawText.Remove(rawText.IndexOf("$"), 1);
					informat.amount = float.Parse(rawText);
				}
			}
			return informat;
		}

		static bool ClickNextArrow(IWebDriver driver)
		{
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			IWebElement nextArrow = wait.Until(e => e.FindElement(By.CssSelector(".nextArw"))); // have seen no such element exception here on page 8 of 10 for women's shoes; maybe refresh the page, wait, the call the function again
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
