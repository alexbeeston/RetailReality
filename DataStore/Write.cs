using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace DataStore
{
	public static class Write
	{
		public static void AddProducts(MySqlCommand command, List<Offer> offers)
		{
			var productsToBeAdded = new List<Product>();
			foreach (var offer in offers)
			{
				command.CommandText = $"SELECT COUNT(*) FROM products WHERE id='{offer.product.id}'";
				object result = command.ExecuteScalar();
				if (result == null) throw new Exception("got a null response when trying to figure out if the product is already in the database.");
				if (Convert.ToInt32(result) != 1) productsToBeAdded.Add(offer.product);
			}

			if (productsToBeAdded.Count == 0)
			{
				Console.WriteLine("No new products to add. Returning.");
				return;
			}

			string sqlInsertProductsCommand = "INSERT INTO products VALUES\n";
			int counter = 0;
			var singleQuoteRegex = new Regex("'");
			foreach (var product in productsToBeAdded)
			{
				sqlInsertProductsCommand += $"('{product.id}', ";
				sqlInsertProductsCommand += $"'{DateTime.Now.ToUniversalTime():yyyy-MM-dd}', ";
				sqlInsertProductsCommand += $"'{singleQuoteRegex.Replace(product.title, "''")}', ";
				sqlInsertProductsCommand += $"'{(product.searchCriteria.gender == Gender.Male ? "m" : "f")}', ";
				sqlInsertProductsCommand += SqlInsertNullableStringType(product.brand, singleQuoteRegex) + ", ";
				sqlInsertProductsCommand += SqlInsertNullableStringType(product.searchCriteria.department, singleQuoteRegex) + ", ";
				sqlInsertProductsCommand += SqlInsertNullableStringType(product.searchCriteria.category, singleQuoteRegex) + ", ";
				sqlInsertProductsCommand += SqlInsertNullableStringType(product.searchCriteria.silhouette, singleQuoteRegex) + ", ";
				sqlInsertProductsCommand += SqlInsertNullableStringType(product.searchCriteria.product, singleQuoteRegex) + ", ";
				sqlInsertProductsCommand += SqlInsertNullableStringType(product.searchCriteria.occasion, singleQuoteRegex) + ")";
				if (counter != productsToBeAdded.Count - 1) sqlInsertProductsCommand += ",\n";
				counter++;
			}
			command.CommandText = sqlInsertProductsCommand;
			var rows = command.ExecuteNonQuery();
			Console.WriteLine($"Inserted {rows} products.");
		}

		public static void AddOffers(MySqlCommand command, List<Offer> offers)
		{
			string insertOffersCommand = "INSERT INTO offers (productId, dateTime, stars, reviews, primaryPrice, primaryType, primaryLabel, primaryNum1, primaryNum2, alternatePrice, alternateType, alternateLabel, alternateNum1, alternateNum2) VALUES\n";
			int counter = 1;
			foreach (var offer in offers)
			{
				insertOffersCommand += $"('{offer.product.id}', ";
				insertOffersCommand += $"'{offer.date:yyyy-MM-dd HH:mm:ss}', ";
				insertOffersCommand += $"{ConvertFloatToSqlNumber(offer.stars, "F1")}, ";
				insertOffersCommand += $"{offer.reviews ?? 0}, ";
				insertOffersCommand += $"{ConvertFloatToSqlNumber(offer.primaryPrice.price)}, ";
				insertOffersCommand += $"'{PriceTypeToChar(offer.primaryPrice.type)}', ";
				insertOffersCommand += $"'{LabelToChar(offer.primaryPrice.label)}', ";
				insertOffersCommand += $"{ConvertFloatToSqlNumber(offer.primaryPrice.price1)}, ";
				insertOffersCommand += $"{ConvertFloatToSqlNumber(offer.primaryPrice.price2)}, ";
				insertOffersCommand += $"{ConvertFloatToSqlNumber(offer.alternatePrice.price)}, ";
				insertOffersCommand += $"'{PriceTypeToChar(offer.alternatePrice.type)}', ";
				insertOffersCommand += $"'{LabelToChar(offer.alternatePrice.label)}', ";
				insertOffersCommand += $"{ConvertFloatToSqlNumber(offer.alternatePrice.price1)}, ";
				insertOffersCommand += $"{ConvertFloatToSqlNumber(offer.alternatePrice.price2)})";

				if (counter != offers.Count) insertOffersCommand += ",\n";
				counter++;
			}
			command.CommandText = insertOffersCommand;
			var rows = command.ExecuteNonQuery();
			Console.WriteLine($"Inserted {rows} offers.");
		}

		private static string ConvertFloatToSqlNumber(float? number, string formatter = "F2")
		{
			if (number == null) return "null";
			return number.Value.ToString(formatter);
		}

		private static char PriceTypeToChar(PriceType? type)
		{
			switch (type)
			{
				case PriceType.Single:
					return 's';
				case PriceType.Range:
					return 'r';
				case PriceType.Hybrid:
					return 'h';
				case PriceType.Bulk:
					return 'b';
				case PriceType.NoPrice:
					return 'n';
				default:
					return 'e';
			}
		}
		
		private static char LabelToChar(LabelType? label)
		{
			switch (label)
			{
				case LabelType.Sale:
					return 's';
				case LabelType.Regular:
					return 'r';
				case LabelType.Original:
					return 'o';
				case LabelType.Clearance:
					return 'c';
				case LabelType.Group:
					return 'g';
				case LabelType.None:
					return 'n';
				case LabelType.NoPrice:
					return 'p';
				default:
					return 'e';
			}
		}

		private static string SqlInsertNullableStringType(string value, Regex singleQuote)
		{
			if (value == null) return "null";
			else return $"'{singleQuote.Replace(value, "''")}'";
		}
	}
}
