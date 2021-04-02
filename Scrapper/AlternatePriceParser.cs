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

			if (RegexUtils.IsPerfectMatch(text, "Original " + RegexUtils.rangeRegEx))
			{
				informant = RegexUtils.BuildRangeInformant(text);
				informant.label = LabelType.Original;
			}
			else if (RegexUtils.IsPerfectMatch(text, "Original " + RegexUtils.priceRegEx) || RegexUtils.IsPerfectMatch(text, "or Original " + RegexUtils.priceRegEx + " each"))
			{
				informant = RegexUtils.BuildSingle(text);
				informant.label = LabelType.Original;
			}
			else if (RegexUtils.IsPerfectMatch(text, "Regular " + RegexUtils.rangeRegEx))
			{
				informant = RegexUtils.BuildRangeInformant(text);
				informant.label = LabelType.Regular;
			}
			else if (RegexUtils.IsPerfectMatch(text, "Regular " + RegexUtils.priceRegEx) || RegexUtils.IsPerfectMatch(text, "or Regular " + RegexUtils.priceRegEx + " each"))
			{
				informant = RegexUtils.BuildSingle(text);
				informant.label = LabelType.Regular;
			}
			else if (RegexUtils.IsPerfectMatch(text, "or group " + RegexUtils.priceRegEx + " each"))
			{
				informant = RegexUtils.BuildSingle(text);
				informant.label = LabelType.Group;
			}
			else if (RegexUtils.IsPerfectMatch(text, RegexUtils.rangeRegEx))
			{
				informant = RegexUtils.BuildRangeInformant(text);
				informant.label = LabelType.None;
			}
			else if (RegexUtils.IsPerfectMatch(text, RegexUtils.priceRegEx))
			{
				informant = RegexUtils.BuildSingle(text);
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
				throw new Exception($"Could not find a perfect match for {text}");
			}
			return informant;
		}
	}
}

