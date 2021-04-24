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

		public void PrintReport(int seedId, string url, int pageNumber)
		{
			Console.WriteLine($"For seed {seedId} page {pageNumber}:");
			Console.WriteLine($"  Url: {url}");
			Console.WriteLine($"  Success: {wasSuccess}");
			Console.WriteLine($"  Attempts: {attempts}");
			Console.WriteLine($"  Exceptions: {exceptions.Count}:");
			foreach (var e in exceptions) Console.WriteLine($"    {e}");
			Console.WriteLine();
		}
	}
}
