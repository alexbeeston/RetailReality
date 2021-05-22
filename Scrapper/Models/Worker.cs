using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Text;

namespace Scrapper
{
	class Worker
	{
		private readonly IWebDriver driver;
		private readonly SearchCriteria searchCriteria;
		private readonly ExecutionPreferences executionPreferences;

		private readonly WebDriverWait infinateWait;
		private readonly WebDriverWait shortWait;
		private readonly List<Offer> offers;
		private MySqlConnection connection;
		private MySqlCommand command;

		public Worker(IWebDriver driver, SearchCriteria searchCriteria, ExecutionPreferences executionPreferences)
		{
			this.driver = driver;
			this.searchCriteria = searchCriteria;
			this.executionPreferences = executionPreferences;

			const int sufficientlyLong = 99;
			infinateWait = new WebDriverWait(driver, TimeSpan.FromDays(sufficientlyLong));
			shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
			offers = new List<Offer>();
		}

		public void GetOffers()
		{
			int pageNumber = 1;
			driver.Navigate().GoToUrl(GenerateUrl());
			do
			{
				var status = ScrapPage();
				if (executionPreferences.logScrapReportToConsole) status.PrintReport(searchCriteria.id, driver.Url, pageNumber);
				pageNumber++;
			} while (ClickNextArrow(infinateWait) && (executionPreferences.pagesToScrapPerSeed == -1 || pageNumber <= executionPreferences.pagesToScrapPerSeed));
		}

		private string GenerateUrl()
		{
			var url = new StringBuilder("https://www.kohls.com/catalog.jsp?CN=");
			string genderUrlParam = null;
			if (searchCriteria.gender != Gender.NotSpecified) genderUrlParam = searchCriteria.gender == Gender.Male ? "Mens" : "Womens";
			bool isFirstParamter = true;
			isFirstParamter = AddOrIgnoreParameter(url, "Gender", genderUrlParam, isFirstParamter);
			isFirstParamter = AddOrIgnoreParameter(url, "Department", searchCriteria.department, isFirstParamter);
			isFirstParamter = AddOrIgnoreParameter(url, "Category", searchCriteria.category, isFirstParamter);
			isFirstParamter = AddOrIgnoreParameter(url, "Product", searchCriteria.product, isFirstParamter);
			isFirstParamter = AddOrIgnoreParameter(url, "Silhouette", searchCriteria.silhouette, isFirstParamter);
			AddOrIgnoreParameter(url, "Occasion", searchCriteria.occasion, isFirstParamter);
			url.Append("&PPP=120");
			return url.ToString();
		}

		private bool AddOrIgnoreParameter(StringBuilder builder, string key, string value, bool isFirstParameter)
		{
			if (value != null)
			{
				if (!isFirstParameter) builder.Append("+");
				builder.Append(Encode(key) + ":" + Encode(value));
				return false;
			}
			return isFirstParameter;
		}

		private static string Encode(string theString)
		{
			theString = theString.Replace(" ", "%20");
			theString = theString.Replace("&", "%26");
			theString = theString.Replace("'", "%27");
			return theString;
		}

		public void LogOffers()
		{
			const string dashedDateFormat = "yyyy-MM-dd";
			if (executionPreferences.logOffersToCsv) File.WriteAllText(@$"..\..\..\Data\csv\{searchCriteria.id}_{DateTime.Now.ToString(dashedDateFormat)}.csv", ConvertOffersToCsv());
			if (executionPreferences.logOffersToJson) File.WriteAllText(@$"..\..\..\Data\serializations\{searchCriteria.id}_{DateTime.Now.ToString(dashedDateFormat)}.json", JsonConvert.SerializeObject(offers));
		}

		private string ConvertOffersToCsv()
		{
			var builder = new StringBuilder();
			int counter = 1;
			builder.Append("fakeOfferId,productId,stars,reviews,primaryPrice,alternatePrice\n");
			foreach (var offer in offers)
			{
				builder.Append(counter + ",");
				builder.Append(offer.product.id + ",");
				builder.Append(offer.stars + ",");
				builder.Append(offer.reviews + ",");
				builder.Append(offer.primaryPrice.individualPrice + ",");
				builder.Append(offer.alternatePrice.individualPrice + "\n");
				counter++;
			}
			return builder.ToString();
		}

		private ScrapReport ScrapPage()
		{
			int attempts = 0;
			var exceptions = new List<Exception>();

			return infinateWait.Until(driver =>
			{
				if (attempts > 5) return new ScrapReport(exceptions, attempts, false);

				try
				{
					var products = GetProducts();
					foreach (IWebElement product in products)
					{
						string productId = product.GetAttribute("id");
						productId = productId.Remove(productId.IndexOf('_'));
						if (!offers.Exists(x => x.product.id == productId) && !product.Text.Contains("For Price, Add to Cart")) offers.Add(BuildOfferFromHtmlElement(product, productId));
					}
					return new ScrapReport(exceptions, attempts, true);
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
				new Product(id, "name", searchCriteria),
				NullableStringToNullableFloat(stars),
				(int?)NullableStringToNullableFloat(reviews),
				primaryPrice,
				alternatePrice,
				DateTime.Now
			); ;
			if (executionPreferences.logOffersToConsole) offer.LogToConsole();
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

		// Database
		public void FlushOffers()
		{
			Console.WriteLine("Are you sure you want to continue onto FlushOffers? Enter 'y' for yes or any other key for no.");
			var selection = Console.ReadLine();
			if (selection.CompareTo("y") != 0) return;

			InitializeConnection();
			AddProducts();
			//AddPrices();
			//AddOffers();
		}

		private void AddProducts()
		{
			var productsToBeAdded = new List<Product>();
			foreach (var offer in offers)
			{
				command.CommandText = $"SELECT COUNT(*) FROM products WHERE id='{offer.product.id}'";
				object result = command.ExecuteScalar();
				if (result == null) throw new Exception("got a null response when trying to figure out if the product is in the db already.");
				if (Convert.ToInt32(result) != 1) productsToBeAdded.Add(offer.product);
			}

			string sqlInsertProductsCommand = "INSERT INTO products VALUES\n";
			int counter = 0;
			foreach (var product in productsToBeAdded)
			{
				sqlInsertProductsCommand += $"('{product.id}', ";
				sqlInsertProductsCommand += $"'{DateTime.Now.ToUniversalTime():yyyy-MM-dd}', ";
				sqlInsertProductsCommand += $"'{product.title}', ";
				sqlInsertProductsCommand += "'m', "; // todo: change product gender to a bool or enum
				sqlInsertProductsCommand += $"'{product.brand}', ";
				sqlInsertProductsCommand += $"'clothing', ";
				sqlInsertProductsCommand += $"'tops', ";
				sqlInsertProductsCommand += $"'t-shirt', ";
				sqlInsertProductsCommand += $"null, null)";
				if (counter != productsToBeAdded.Count - 1) sqlInsertProductsCommand += ",\n";
				counter++;
			}
			command.CommandText = sqlInsertProductsCommand;
			Console.WriteLine(sqlInsertProductsCommand);
			Console.WriteLine($"Rows affected on insert: {command.ExecuteNonQuery()}");
		}

		private void InitializeConnection()
		{
			Console.WriteLine("Connecting to MySql server...");
			var helper = new MySqlConnectionStringBuilder();
			helper.Server = executionPreferences.mySqlHostIp;
			helper.UserID = executionPreferences.mySqlUserName;
			helper.Password = executionPreferences.mySqlPassword;
			helper.Database = "retailReality";
			helper.DefaultCommandTimeout = executionPreferences.mySqlTimeout;
			connection = new MySqlConnection(helper.ToString());
			connection.Open();
			Console.WriteLine("Connection successful");
			command = new MySqlCommand();
			command.Connection = connection;
		}

		private int Insert(string commandText)
		{
			command.CommandText = commandText;
			return command.ExecuteNonQuery();
		}

		private static void DisplayData(System.Data.DataTable table)
        {
            foreach (System.Data.DataRow row in table.Rows)
            {
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    Console.WriteLine("{0} = {1}", col.ColumnName, row[col]);
                }
                Console.WriteLine("============================");
            }
        }

		// Methods for dev only
		public Worker(Configurations configs, List<Offer> offers)
		{
			this.executionPreferences = configs.executionPreferences;
			this.offers = offers;
		}
	}
}
