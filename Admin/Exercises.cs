using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin
{
    public static class Exercises
    {
		public static IWebElement TryFindElement(this IWebElement we, By by)
		{
			try
			{
				return we.FindElement(by);
			}
			catch (NoSuchElementException)
			{
				return null;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
