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
		public int include = -1;

		public string ToUrl()
		{
			return "dog";
		}
	}


	class Configs 
	{

		public List<Seed> terminals = new List<Seed>();
		public List<Seed> nonTerminals = new List<Seed>();

		public void PopulateTerminals()
		{
			foreach (var terminal in terminals)
			{
				AddPairs(terminal.include, terminal);
			}
		}

		private void AddPairs(int includeId, Seed seed)
		{
			if (includeId == -1) return;
			Seed nonTerminal = nonTerminals.Find(x => x.id == includeId);
			foreach (var pair in nonTerminal.pairs) seed.pairs.Add(pair.Key, pair.Value);
			AddPairs(nonTerminal.include, seed);
		}
	}

	static class LoadConfigurations
	{
		public static List<Seed> GetSeeds()
		{
			Configs configurations = JsonConvert.DeserializeObject<Configs>(File.ReadAllText(@"..\..\..\seedConfigs.json"));
			configurations.PopulateTerminals();
			return configurations.terminals;
		}
	}
}
