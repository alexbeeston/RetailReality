using System;
using System.Collections.Generic;
using System.Text;

namespace Scrapper
{
	class ScrapPageStatus
	{
		public readonly List<Exception> exceptions;
		public readonly int attempts;
		public readonly bool wasSuccess;

		public ScrapPageStatus(List<Exception> exceptions, int attempts, bool wasSuccess)
		{
			this.exceptions = exceptions;
			this.attempts = attempts;
			this.wasSuccess = wasSuccess;
		}

		public void PrintReport(string url, int pageNumber)
		{
			Console.WriteLine($"For url {url} on page {pageNumber}:");
			Console.WriteLine($"  Success: {wasSuccess}");
			Console.WriteLine($"  Attempts: {attempts}");
			Console.WriteLine($"  Exceptions: {exceptions.Count}:");
			foreach (var e in exceptions) Console.WriteLine($"    {e}");
			Console.WriteLine();
		}
	}
}
