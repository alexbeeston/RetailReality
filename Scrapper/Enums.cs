namespace Scrapper
{
	enum Label
	{
		Original,
		Sale,
		Regular,
		Clearance,
		Group,
		None,
		Unknown	
	}

	enum PriceType
	{
		Single, // $15
		Range, // $15 - $20
		Hybrid, // $15 or 2/$25
		Bulk, // 2 / $25
		Unknown
	}
}