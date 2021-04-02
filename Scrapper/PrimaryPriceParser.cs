using System;

namespace Scrapper
{
	class PrimaryPriceParser 
	{
		public static PriceInformant ParsePrimaryPrice(string labelText, string priceText)
		{
			PriceInformant informant;

			if (RegexUtils.IsPerfectMatch(priceText, RegexUtils.hybridRegEx))
			{
				informant = RegexUtils.BuildHybridInformant(priceText);
			}
			else if (RegexUtils.IsPerfectMatch(priceText, RegexUtils.bulkRegEx))
			{
				informant = RegexUtils.BuildBulkInformant(priceText);
			}
			else if (RegexUtils.IsPerfectMatch(priceText, RegexUtils.rangeRegEx))
			{
				informant = RegexUtils.BuildRangeInformant(priceText);
			}
			else if (RegexUtils.IsPerfectMatch(priceText, RegexUtils.priceRegEx))
			{
				informant = RegexUtils.BuildSingle(priceText);
			}
			else
			{
				throw new Exception($"Could not find a perfect match for the primary price with label \"{labelText}\" and price \"{priceText}\"");
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
				case "Original":
					label = LabelType.Original;
					break;
				case "Clearance":
					label = LabelType.Clearance;
					break;
				case "":
					label = LabelType.None;
					break;
				default:
					throw new Exception($"Could not find a priceLabel for {labelText}.");
			}
			informant.label = label;
			return informant;
		}
	}
}

