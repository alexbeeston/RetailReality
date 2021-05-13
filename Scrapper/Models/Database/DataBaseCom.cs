using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace Scrapper
{
	class DataBaseCom
	{
		private readonly MySqlConnection connection;
		private readonly MySqlCommand command;

		public DataBaseCom(string serverIp, string userId, string password, uint timeout, string database = "RetailReality")
		{
			var helper = new MySqlConnectionStringBuilder();
			helper.Server = serverIp;
			helper.UserID = userId;
			helper.Password = password;
			helper.Database = database;
			helper.DefaultCommandTimeout = timeout;
			connection = new MySqlConnection(helper.ToString());
			connection.Open();

			command = new MySqlCommand();
			command.Connection = connection;
		}

		public void FlushOffers(List<Offer> offers)
		{

		}

		public void AddProduct(Product product)
		{
			// a Product doesn't have a 'first offer date', but add it to the database in this method;
			// we know we haven't yet added this product to the database, so we can add DateTime.Now
			// as the first recorded offer, because we're adding an offer

		}

		public bool ProductInDatabase(Product product)
		{
			return true;
		}
	}
}
