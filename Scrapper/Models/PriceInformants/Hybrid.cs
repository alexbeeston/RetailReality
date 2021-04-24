﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Hybrid : PriceInformant
	{
		public float? quantity = null;
		public float? bulkPrice = null;

		public Hybrid(float individualPrice, float quantity, float bulkPrice)
		{
			type = PriceType.Hybrid;
			this.individualPrice = individualPrice;
			this.quantity = quantity;
			this.bulkPrice = bulkPrice;
			foreignKeyToDerviedPriceTable = Global.counter.GetNextId();
		}

		public override void DataBaseCom()
		{
			Console.Write($"Num1={quantity}. Num2={bulkPrice}. ");
			base.DataBaseCom();
		}

		public override void Validate()
		{
			if (quantity == null) throw new Exception("Quantity is unitialized"); // could replace by just passing a dictionary of strings to floats and looping over checking for null
			if (bulkPrice == null) throw new Exception("Total Price is unitialiazed");
			base.Validate();
		}
	}
}