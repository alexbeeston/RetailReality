using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
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
				Seed seed = new Seed(combination.id);
				foreach (int pairId in combination.include)
				{
					Pair pair = pairs.Find(x => x.id == pairId);
					seed.searchCriteria.Add(pair.key, pair.value);
				}
				seeds.Add(seed);
			}

			return seeds;
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
}
