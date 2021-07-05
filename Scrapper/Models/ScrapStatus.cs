using System;
using System.Collections.Generic;
using System.Text;
using DataStore;

namespace Scrapper
{
	class ScrapStatus
	{
		public readonly List<Exception> exceptions;
		public readonly int attempts;
		public readonly bool wasSuccess;

		public ScrapStatus(List<Exception> exceptions, int attempts, bool wasSuccess)
		{
			this.exceptions = exceptions;
			this.attempts = attempts;
			this.wasSuccess = wasSuccess;
		}

		public string GenerateReport(SearchCriteria criteria, string url, int pageNumber)
		{
			string report = $"For search criteria {criteria} page {pageNumber}:\n";
			report += $"  Url: {url}\n";
			report += $"  Success: {wasSuccess}\n";
			report += $"  Attempts: {attempts}\n";
			report += $"  Exceptions: {exceptions.Count}:\n";
			int counter = 1;
			foreach (var e in exceptions)
			{
				report += $"    {e}";
				if (counter != exceptions.Count) report += "\n";
				counter++;
			}
			return report;
		}
	}
}
