using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Scrapper
{
	class Seed
	{
		public Dictionary<string, string> pairs = new Dictionary<string, string>();
		public int id = -1;

		public string ToUrl()
		{
			return "https://usu.edu";
		}
	}

	class Combination 
	{
		public int id;
		public List<int> include;
	}

	class Pair
	{
		public int id;
		public string key;
		public string value;
	}

	class Configs 
	{
		public List<Combination> combinations = new List<Combination>();
		public List<Pair> pairs = new List<Pair>();
		private List<Seed> seeds;

		public List<Seed> GetSeeds()
		{
			if (seeds != null) return seeds;

			seeds = new List<Seed>();
			foreach (Combination combination in combinations)
			{
				Seed seed = new Seed();
				seed.id = combination.id;
				foreach (int idToInclude in combination.include)
				{
					Pair pair = pairs.Find(x => x.id == idToInclude);
					seed.pairs.Add(pair.key, pair.value);
				}
				seeds.Add(seed);
			}

			return seeds;
		}
	}

	static class LoadConfigurations
	{
		public static List<Seed> GetSeeds()
		{
			Configs configs = JsonConvert.DeserializeObject<Configs>(File.ReadAllText(@"..\..\..\seedConfigs.json"));
			var seeds = configs.GetSeeds();
			return seeds;
		}
	}
}
