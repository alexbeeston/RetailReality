using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Scrapper
{
	static class Miscellaneous
	{
		public static List<Seed> GetSeeds()
		{
			Configs configs = JsonConvert.DeserializeObject<Configs>(File.ReadAllText(@"..\..\..\seedConfigs.json"));
			var seeds = configs.GetSeeds();
			return seeds;
		}
	}
}
