using System;
using System.Text.RegularExpressions;
using DataStore;

namespace Scrapper
{
	static class PriceParsers
	{
		public static PriceInformant ParsePrimaryPrice(string labelText, string priceText)
		{
			PriceInformant informant;

			if (IsPerfectMatch(priceText, hybridRegEx))
			{
				informant = BuildHybridInformant(priceText);
			}
			else if (IsPerfectMatch(priceText, bulkRegEx))
			{
				informant = BuildBulkInformant(priceText);
			}
			else if (IsPerfectMatch(priceText, rangeRegEx))
			{
				informant = BuildRangeInformant(priceText);
			}
			else if (IsPerfectMatch(priceText, priceRegEx))
			{
				informant = BuildSingle(priceText);
			}
			else
			{
				throw new Exception($"Could not match \"{priceText}\" to a primary price");
			}

			LabelType label;
			switch (labelText)
			{
				case "Sale":
					label = LabelType.Sale;
					break;
				case "Regular":
					label = LabelType.Regular;
					break;
				case "Reg.":
					label = LabelType.Regular;
					break;
				case "Original":
					label = LabelType.Original;
					break;
				case "Clearance":
					label = LabelType.Clearance;
					break;
				case "Group":
					label = LabelType.Group;
					break;
				case "":
					label = LabelType.None;
					break;
				default:
					throw new Exception($"Could not match \"{labelText}\" to a primary price label.");
			}
			informant.label = label;
			return informant;
		}

		public static PriceInformant ParseAlternatePrice(string text)
		{
			PriceInformant informant;

			if (IsPerfectMatch(text, "Original " + rangeRegEx))
			{
				informant = BuildRangeInformant(text);
				informant.label = LabelType.Original;
			}
			else if (IsPerfectMatch(text, "Original " + priceRegEx) || IsPerfectMatch(text, "or Original " + priceRegEx + " each"))
			{
				informant = BuildSingle(text);
				informant.label = LabelType.Original;
			}
			else if (IsPerfectMatch(text, "Regular " + rangeRegEx))
			{
				informant = BuildRangeInformant(text);
				informant.label = LabelType.Regular;
			}
			else if (IsPerfectMatch(text, "Regular " + priceRegEx) || IsPerfectMatch(text, "or Regular " + priceRegEx + " each"))
			{
				informant = BuildSingle(text);
				informant.label = LabelType.Regular;
			}
			else if (IsPerfectMatch(text, "or group " + priceRegEx + " each"))
			{
				informant = BuildSingle(text);
				informant.label = LabelType.Group;
			}
			else if (IsPerfectMatch(text, rangeRegEx))
			{
				informant = BuildRangeInformant(text);
				informant.label = LabelType.None;
			}
			else if (IsPerfectMatch(text, priceRegEx))
			{
				informant = BuildSingle(text);
				informant.label = LabelType.None;
			}
			else if (text.CompareTo(string.Empty) == 0)
			{
				informant = new PriceInformant()
				{
					individualPrice = 0,
					label = LabelType.NoPrice,
					type = PriceType.NoPrice
				};
			}
			else
			{
				throw new Exception($"Could not match \"{text}\" to an alternate price");
			}
			return informant;
		}

		private static bool IsPerfectMatch(string text, string pattern)
		{
			return Regex.IsMatch(text, pattern) && Regex.Match(text, pattern).Length == text.Length;
		}

		private static PriceInformant BuildSingle(string text)
		{
			if (Regex.Matches(text, financialQuantityRegex).Count != 1) throw new Exception("Found more than one match, expected only one.");
			float amount = float.Parse(Regex.Match(text, financialQuantityRegex).Value);
			return new PriceInformant()
			{
				individualPrice = amount,
				type = PriceType.Single
			};
		}

		private static DataStore.Range BuildRangeInformant(string text)
		{
			MatchCollection amounts = Regex.Matches(text, financialQuantityRegex);
			if (amounts.Count != 2) throw new Exception("Parsed a range text, but did not get 2 financial quantities for input text\"" + text + "\"");
			float low = float.Parse(amounts[0].Value);
			float high = float.Parse(amounts[1].Value);
			return new DataStore.Range(low, high);
		}

		private static Bulk BuildBulkInformant(string text)
		{
			int quantity = int.Parse(Regex.Match(text, @"\d+").Value);
			float totalPrice = float.Parse(Regex.Match(text, financialQuantityRegex).Value);
			return new Bulk(quantity, totalPrice);
		}

		private static Hybrid BuildHybridInformant(string text)
		{
			float individualPrice = float.Parse(Regex.Match(text, financialQuantityRegex).Value);
			Bulk tempBulkInformant = BuildBulkInformant(Regex.Match(text, bulkRegEx).Value);
			float quantity = (float)tempBulkInformant.quantity;
			float bulkPrice = (float)tempBulkInformant.bulkPrice;
			return new Hybrid(individualPrice, quantity, bulkPrice);
		}

		private static readonly string financialQuantityRegex = @"\d+.\d{2}";
		private static readonly string priceRegEx = @"\$" + financialQuantityRegex;
		private static readonly string rangeRegEx = priceRegEx + " - " + priceRegEx;
		private static readonly string bulkRegEx = @"\d / " + priceRegEx;
		private static readonly string hybridRegEx = priceRegEx + " OR " + bulkRegEx;
	}
}

