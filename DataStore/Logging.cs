using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DataStore
{
	static public class Logging
	{
		static private string logFilePath = @"..\..\..\..\log.txt";

		static public void Info(string info)
		{
			Log("INFO   ", info);
		}

		static public void Warning(string warning)
		{
			Log("WARNING", warning);
		}

		static public void Error(string error)
		{
			Log("ERROR  ", error);
		}

		static private void Log(string level, string message)
		{
			var now = DateTime.Now;
			message = $"{level} {now:yyyy-MM-dd HH:mm:ss} {message}\n";
			Console.WriteLine($"---> {message}");
			File.AppendAllText(logFilePath, message);
		}

		static public void InitLogging()
		{
			File.AppendAllText(logFilePath, $"\n\n=================================\nNew Instance: {(DateTime.Now):yyyy-MM-dd HH:mm:ss}\n=================================\n");
		}
	}
}
