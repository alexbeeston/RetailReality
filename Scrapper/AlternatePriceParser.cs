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

			if (Utils.IsPerfectMatch(text, "Original " + Utils.rangeRegEx))
			{
				informant = Utils.BuildRangeInformant(text);
				informant.label = LabelType.Original;
			}
			else if (Utils.IsPerfectMatch(text, "Original " + Utils.priceRegEx) || Utils.IsPerfectMatch(text, "or Original " + Utils.priceRegEx + " each"))
			{
				informant = Utils.BuildSingle(text);
				informant.label = LabelType.Original;
			}
			else if (Utils.IsPerfectMatch(text, "Regular " + Utils.rangeRegEx))
			{
				informant = Utils.BuildRangeInformant(text);
				informant.label = LabelType.Regular;
			}
			else if (Utils.IsPerfectMatch(text, "Regular " + Utils.priceRegEx) || Utils.IsPerfectMatch(text, "or Regular " + Utils.priceRegEx + " each"))
			{
				informant = Utils.BuildSingle(text);
				informant.label = LabelType.Regular;
			}
			else if (Utils.IsPerfectMatch(text, "or group " + Utils.priceRegEx + " each"))
			{
				informant = Utils.BuildSingle(text);
				informant.label = LabelType.Group;
			}
			else if (Utils.IsPerfectMatch(text, Utils.rangeRegEx))
			{
				informant = Utils.BuildRangeInformant(text);
				informant.label = LabelType.None;
			}
			else if (Utils.IsPerfectMatch(text, Utils.priceRegEx))
			{
				informant = Utils.BuildSingle(text);
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

