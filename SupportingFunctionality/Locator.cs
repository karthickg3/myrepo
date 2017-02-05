using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Automation_Testing
{
    
    class Locator
    {

        public By ReturnCorrectBy(String rawLocator)
        {

            if (rawLocator.ToLower().StartsWith("xpath:"))
            {
                return By.XPath(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            if (rawLocator.ToLower().StartsWith("id:"))
            {
                return By.Id(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            if (rawLocator.ToLower().StartsWith("classname:"))
            {
                return By.ClassName(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            if (rawLocator.ToLower().StartsWith("cssselector:"))
            {
                return By.CssSelector(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            if (rawLocator.ToLower().StartsWith("linktext:"))
            {
                return By.LinkText(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            if (rawLocator.ToLower().StartsWith("name:"))
            {
                return By.Name(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            if (rawLocator.ToLower().StartsWith("partiallinktext:"))
            {
                return By.PartialLinkText(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            if (rawLocator.ToLower().StartsWith("tagname:"))
            {
                return By.TagName(rawLocator.Substring(rawLocator.IndexOf(':') + 1).TrimStart(new[] { ' ' }));
            }
            throw new ArgumentException(string.Format("{0} : Error: Provided rawlocator '" + rawLocator + "' was invalid!", DateTime.Now.ToString("yyyy mm dd hh:mm:ss t z")));

        }

    }
}

