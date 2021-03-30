namespace Scrapper
{
	enum Label
	{
		Original,
		Sale,
		Regular,
		Clearance,
		Group,
		None, // there exists a price, but it is not labeld
		NoPrice, // there is no price
		Unknown	 // there exists a price, it doesn't have a blank label, but we couldn't identify it (will eventually be put at the final else statement of the price parser instead of throwing an error
	}

	enum PriceType
	{
		Single, // $15
		Range, // $15 - $20
		Hybrid, // $15 or 2/$25
		Bulk, // 2 / $25
		NoPrice, // no price is given
		Unknown
	}
}