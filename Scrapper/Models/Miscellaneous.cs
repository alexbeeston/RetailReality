using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class Counter
	{
		public int lastId;
		public int GetNextId()
		{
			lastId++;
			return lastId;
		}
	}
}
