using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace Scrapper
{
	class Program
	{
		static void Main()
		{
			IWebDriver driver = new FirefoxDriver();
			driver.Navigate().GoToUrl("https://usu.edu");
			Console.WriteLine(driver.Title);
		}
	}
}
