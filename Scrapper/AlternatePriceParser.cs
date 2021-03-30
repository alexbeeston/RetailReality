using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Scrapper
{
	class AlternatePriceParser
	{
		public static PriceInformant ParseAlternatePrice(string text)
		{
			PriceInformant informant;

			if (IsPerfectMatch(text, "Original " + Utils.rangeRegEx))
			{
				informant = BuildRangeInformant(text);
				informant.label = Label.Original;
			}
			else if (IsPerfectMatch(text, "Original " + Utils.priceRegEx) || IsPerfectMatch(text, "or Original " + Utils.priceRegEx + " each"))
			{
				informant = BuildSingle(text);
				informant.label = Label.Original;
			}
			else if (IsPerfectMatch(text, "Regular " + Utils.rangeRegEx))
			{
				informant = BuildRangeInformant(text);
				informant.label = Label.Regular;
			}
			else if (IsPerfectMatch(text, "Regular " + Utils.priceRegEx) || IsPerfectMatch(text, "or Regular " + Utils.priceRegEx + " each"))
			{
				informant = BuildSingle(text);
				informant.label = Label.Regular;
			}
			else if (IsPerfectMatch(text, "or group " + Utils.priceRegEx + " each"))
			{
				informant = BuildSingle(text);
				informant.label = Label.Group;
			}
			else if (IsPerfectMatch(text, Utils.rangeRegEx))
			{
				informant = BuildRangeInformant(text);
				informant.label = Label.None;
			}
			else if (IsPerfectMatch(text, Utils.priceRegEx))
			{
				informant = BuildSingle(text);
				informant.label = Label.None;
			}
			else if (text.CompareTo(string.Empty) == 0)
			{
				informant = new PriceInformant()
				{
					amount = 0,
					label = Label.NoPrice,
					type = PriceType.NoPrice
				};
			}
			else
			{
				throw new Exception($"Could not find a perfect match for {text}");
			}
			return informant;
		}

		private static bool IsPerfectMatch(string text, string pattern)
		{
			return Regex.IsMatch(text, pattern) && Regex.Match(text, pattern).Length == text.Length;
		}

		private static Range BuildRangeInformant(string rangeText)
		{
			MatchCollection amounts = Regex.Matches(rangeText, Utils.financialQuantityRegex);
			if (amounts.Count != 2) throw new Exception("Parsed a range text, but did not get 2 financial quantities for input text\"" + rangeText + "\"");
			float low = float.Parse(amounts[0].Value);
			float high = float.Parse(amounts[1].Value);
			return new Range(low, high);
		}

		private static PriceInformant BuildSingle(string text)
		{
			if (Regex.Matches(text, Utils.financialQuantityRegex).Count != 1) throw new Exception("Found more than one match, expected only one.");
			float amount = float.Parse(Regex.Match(text, Utils.financialQuantityRegex).Value);
			return new PriceInformant()
			{
				amount = amount,
				type = PriceType.Single
			};
		}
	}
}
