using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using TechTalk.SpecFlow;


namespace Automation_Testing
{
    public class Action : SetupTearDown
    {
        private readonly Locator _locator = new Locator();
        private double Implicitwait = Convert.ToDouble(ConfigurationManager.AppSettings["Driver.implicit.wait.sec"]);

        public IWebElement ReturnElementDefaultWait(String rawLocator)
        {
                Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Implicitwait));
                for (int counter = 0;
                    counter <= Convert.ToInt32(ConfigurationManager.AppSettings["Default.wait.for.element.sec"])*2;
                    counter++)
                {
                    try
                    {
                        return Driver.FindElement(_locator.ReturnCorrectBy(rawLocator));
                    }
                    catch (NoSuchElementException)
                    {
                        Thread.Sleep(500);
                    }

                }
 
            throw new NoSuchElementException(
                string.Format("{0} Error: Element can not be found by rawlocator '{1}' for {2} seconds!",
                    DateTime.Now.ToString("yyyy.mmmm.dd hh:mm:ss"), rawLocator,
                    ConfigurationManager.AppSettings["Default.wait.for.element.sec"]));
        }

        private IWebElement ReturnElementSpecifiedtWait(String rawLocator, int waitSeconds)
        {
            Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Implicitwait));
            for (int counter = 0; counter <= waitSeconds*2; counter++)
            {
                try
                {
                    return Driver.FindElement(_locator.ReturnCorrectBy(rawLocator));
                }
                catch (NoSuchElementException)
                {
                    //Console.Write("Elenet searched by rawlocator " + rawLocator + " not found");
                    Thread.Sleep(500);
                }
            }
           throw new NoSuchElementException(
                string.Format("{0} Error: Element can not be found by rawlocator '{1}' for {2} seconds!",
                    DateTime.Now.ToString("yyyy mmmm dd hh:mm:ss t z"), rawLocator,
                    ConfigurationManager.AppSettings["Default.wait.for.element.sec"]));
        }

        private Boolean ElementExistDefaultWait(String rawLocator)
        {
            try
            {
                ReturnElementDefaultWait(rawLocator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public Boolean ElementExist(String rawLocator)
        {
            try
            {
                Driver.FindElement(_locator.ReturnCorrectBy(rawLocator));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private Boolean ElementExistSpecificWait(String rawLocator, int waitSeconds)
        {
            try
            {
                ReturnElementSpecifiedtWait(rawLocator, waitSeconds);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void GoTo(String url)
        {
            //if (url.StartsWith("https"))
            //{
            //    Driver.Navigate().GoToUrl(url);
            //}
            //else
            //{
                Driver.Navigate().GoToUrl(url);
            //}

        }

        public void ClickBy(String rawLocator)
        {
            ReturnElementDefaultWait(rawLocator).Click();
        }

        public void ClickBySpecificWait(String rawLocator, int wait)
        {
            ReturnElementSpecifiedtWait(rawLocator, wait).Click();
        }

        public void SetChechBoxTo(String rawLocator, Boolean marked)
        {
            if (marked && !ReturnElementDefaultWait(rawLocator).Selected)
            {
                ReturnElementDefaultWait(rawLocator).Click();
            }
            if (!marked && ReturnElementDefaultWait(rawLocator).Selected)
            {
                ReturnElementDefaultWait(rawLocator).Click();
            }
        }

        public void ClearAndTypeText(String rawLocator, String text)
        {
            for (int counter = 0;
                counter <= Convert.ToInt32(ConfigurationManager.AppSettings["Default.wait.for.element.sec"])*2;
                counter++)
            {
                try
                {
                    ReturnElementDefaultWait(rawLocator).Clear();
                    ReturnElementDefaultWait(rawLocator).SendKeys(text);
                    return;
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                }
            }
        }

        public void SelectOptionWithText(String rawLocator, String text)
        {
            var select = ReturnElementDefaultWait(rawLocator);
            var options = new List<IWebElement>(@select.FindElements(By.TagName("option")));
            foreach (var webElement in options.Where(webElement => webElement.Text.Equals(text)))
            {
                webElement.Click();
                return;
            }
            throw new ArgumentException(
                string.Format(
                    "{0} Error: Provided text '{1}' was not found as a option for element located by rawlocator '{2}'!",
                    DateTime.Now.ToString("yyyy mmmm dd hh:mm:ss t z"), text, rawLocator));
        }

        public void SelectOptionFromDropDown(String rawLocator)
        {
            var select = ReturnElementDefaultWait(rawLocator);
            var options = new List<IWebElement>(@select.FindElements(By.TagName("option")));
            foreach (var webElement in options)
            {
                webElement.Click();
                return;
            }
            throw new ArgumentException(
                string.Format(
                    "{0} Error: Provided text '{1}' was not found as a option for element located by rawlocator '{2}'!",
                    DateTime.Now.ToString("yyyy mmmm dd hh:mm:ss t z"), rawLocator));

        }



        public Boolean ElementIsDisplayed(String rawLocator)
        {
            if (!ElementExist(rawLocator)) return false;
            return ReturnElementDefaultWait(rawLocator).Displayed;
        }

        public Boolean ElementIsNotDisplayed(String rawLocator)
        {
            try
            {

                if (Driver.FindElement(_locator.ReturnCorrectBy(rawLocator)).Displayed)
                {
                    return false;
                }
                return true;
            }
            catch (NoSuchElementException)
            {

                return true;
            }
        }

        public Boolean ElementIsDisplayedSpecifiedWait(String rawLocator, int waitSeconds)
        {

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (ElementExistSpecificWait(rawLocator, waitSeconds))
            {
                while (stopwatch.ElapsedMilliseconds <= waitSeconds*1000)
                {
                    Console.Write("Element is displayed: " + ReturnElementIdentifiedBy(rawLocator).Displayed);
                    if (ReturnElementIdentifiedBy(rawLocator).Displayed)
                    {
                        return true;
                    }
                    Thread.Sleep(500);
                }
                return false;
            }

            stopwatch.Stop();
            return false;
        }

        public IWebElement ReturnElementIdentifiedBy(String rawLocator)
        {
            return Driver.FindElement(_locator.ReturnCorrectBy(rawLocator));
        }

        public ReadOnlyCollection<IWebElement> ReturnElementsIdentifiedBy(String rawLocator)
        {
            return Driver.FindElements(_locator.ReturnCorrectBy(rawLocator));
        }

        public String ReturnAtributValueBy(string rawLocator, string atribute)
        {
            return Driver.FindElement(_locator.ReturnCorrectBy(rawLocator)).GetAttribute(atribute);
        }

        public String ReturnCurrentUrl()
        {
            return Driver.Url;
        }
    }
 }


