using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Automation_Testing
{
    class BasicSteps : Action
    {
        Action action = new Action();
        Random rnd = new Random();

        public void GivenIGoToPage(string pageName)
        {
            action.GoTo(ConfigurationManager.AppSettings[pageName]);
        }

        public void ClickOnElementIfItsDisplayed(string locator)
        {
            if (action.ElementIsDisplayed(locator))
            {
                action.ClickBy(locator);
            }
        }

       
        public void TypeTextInElementIfItsDisplayed(string text, string locator)
        {
            if (action.ElementIsDisplayed(locator))
            {
                action.ClearAndTypeText(locator, text);
            }
        }     

        
        public void ClickOnElementIdentifiedBy(string locator)
        {
            action.ClickBy(locator);
        }

        
        public void SetStateOfCheckBox(string locator, bool marked)
        {
            action.SetChechBoxTo(locator, marked);
        }

        [When("I clear element found by locator '(.*)' and type text '(.*)'")]
        public void TypeTextInElementIdentifiedBy(string locator, string text)
        {
            action.ClearAndTypeText(locator, text);
        }

        
        public void SelectOptionFromElementIdentifiedBy(string text, string locator)
        {
            action.SelectOptionWithText(locator, text);
        }

        
        public void WaitForElementToAppear(string waitSeconds, string locator)
        {
            Assert.True(action.ElementIsDisplayedSpecifiedWait(locator, Convert.ToInt32(waitSeconds)), "Error element found by locator " + locator + " was not diaplayed for " + waitSeconds + " seconds");
        }

        
        public void WaitForElementToDissapear(string waitSeconds, string locator)
        {
            for (int counter = 0; counter <= Convert.ToInt32(waitSeconds) * 2; counter++)
            {
                if (!action.ElementExist(locator)) return;
                if (action.ElementIsDisplayed(locator))
                {
                    Thread.Sleep(500);
                }
                else
                {
                    return;
                }
            }
            throw new Exception("Error element located by locator " + locator + " is still displayed after " + waitSeconds + " seconds");
        }

        
        public void ElementFoundByIsDisplayed(string locator)
        {
            Assert.True(action.ReturnElementDefaultWait(locator).Displayed, "Error: Element found by locator " + locator + " is not displayed after the default wait time!");
        }

        
        public void ElementFoundByIsNotDisplayed(string locator)
        {
            Assert.True(action.ElementIsNotDisplayed(locator), "Error element found by rawlocator " + locator + " is displayed!");
        }

        public String ReturnTextFromElementBy(String locator)
        {
            return action.ReturnElementIdentifiedBy(locator).Text;
        }

        public String ReturnValueFromElementBy(String locator)
        {
            return action.ReturnElementIdentifiedBy(locator).GetAttribute("value");
        }

        public String ReturnAtributValueFromElementBy(String locator, string atribute)
        {
            return action.ReturnAtributValueBy(locator, atribute);
        }

        public int ReturnNumberOfElementBy(String locator)
        {
            return action.ReturnElementsIdentifiedBy(locator).Count;
        }

        public String CreateRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

         return builder.ToString();

        }

        public String CreateRandomNumericString(int size)

        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            int ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = random.Next(0,9);
                builder.Append(ch);
            }

         return builder.ToString();
        }

        //public void ClickEnterButtonForAlertMessage()
        //{
        //    Thread.Sleep(5000); 
        //    IAlert alert = Driver.SwitchTo().Alert(); 
        //        if(alert != null) 
        //        { 
        //           actions.SendKeys(Keys.Enter);  
        //        }  
        //        //catch (NoAlertPresentException Ex)
        //        //{
        //        //    var msg = Ex.InnerException.ToString() + " / " + Ex.Message.ToString();
        //        //    Console.WriteLine(msg);

        //        //}   
        //    }
        public double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }

        public string DatePlusOne(DateTime selectedDate, double totalIndays)
        {
            TimeSpan ts = TimeSpan.FromDays(totalIndays);
            string dateFormat = "dd-MMM-yyyy";
            return selectedDate.Add(ts).ToString(dateFormat);
        }

        public void ClickAlertMessage()
        {
            Driver.SwitchTo().Alert().Accept();
        }

        public int GetNextDate()
        {
            var nextDate = DateTime.Now.AddDays(1); //02/05/2015
            var found = false;
            var dd = 0;
            while (!found)
            {
                var dayOfNextDate = nextDate.DayOfWeek; //Saturday
                if (dayOfNextDate != DayOfWeek.Saturday && dayOfNextDate != DayOfWeek.Sunday)
                {
                    found = true;
                    dd = nextDate.Day;
                }
                else
                {
                    nextDate = nextDate.AddDays(1);
                }
            }
            return dd;
        }

        public string ReturnSelectedOptionFromDropdown(string rawlocator)
        {
            var dropdownList = new SelectElement(ReturnElementIdentifiedBy(rawlocator));
            var firstText = dropdownList.Options.First(opt => opt.Selected).Text;
            return firstText;
        }

        public DateTime GetRandomBirthDate()
        {
            var year = rnd.Next(1904, 1998);
            var month = rnd.Next(1, 12);
            var day = DateTime.DaysInMonth(year, month);

            var Day = rnd.Next(1, day);

            DateTime dt = new DateTime(year, month, Day);
            return dt;            
        }

        public string GetCurrencyFromList()
        {
            
            var list = CurrencyWithAmount;
            int ind = rnd.Next(1, 11);
            var currency = list.Keys.ElementAt(ind);

            return currency;
        }

        public Dictionary<string, string> CurrencyWithAmount
        {
            get
            {
                var list = new Dictionary<string, string>
                {
                    {"British Pound Sterling (GBP)", "500"},
                    {"Euro (EUR)", "150"},
                    {"US Dollar (USD)", "20"},
                    {"Australian Dollar (AUD)", "30"},
                    {"New Zealand Dollar (NZD)", "30"},
                    {"Hong Kong Dollar (HKD)", "150"},
                    {"Swiss Franc (CHF)", "20"},
                    {"Danish Krone (DKK)", "150"},
                    {"Swedish Krona (SEK)", "150"},
                    {"Norwegian Krone (NOK)", "150"},
                    {"Singapore Dollar (SGD)", "30"},

                };
                return list;
            }
        }

        public string GetTitleFromList()
        {
            List<string> titleList = new List<string>
            {
                "Mr",
                "Mrs",
                "Miss",
                "Ms",
                "Dr",
                "Lord",
                "Professor",
                "Reverend"
            };

            int ind = rnd.Next(1, 8);
            var title = titleList[ind];
            return title;
        }

        public string GetGenderFromList()
        {
            List<string> genderList = new List<string>
            {
                "Male",
                "Female"
            };

            int ind = rnd.Next(1, 2);
            var gender = genderList[ind];
            return gender;
        }

        public string GetAdvertFromList()
        {
            List<string> advertList = new List<string>
            {
                "Google",
                "Newspaper article",
                "Magazine",
                "Money Supermarket",
                "Money.co.uk",
                "Referral",
                "Money Saving Expert",
            };

            int ind = rnd.Next(1, 2);
            var advert = advertList[ind];
            return advert;
        }

        public string currencyAmount()
        {
            var randomCurrencyAmount = new Random().Next(10, 1000);
            return Convert.ToString(randomCurrencyAmount);
        }

        public string getlistindropdown(string Locator)
        {
            var options = action.ReturnElementIdentifiedBy(Locator).Text;
            var optionsArr = options.Split('\n');
            var arrayLength = optionsArr.Length;
            int ind = rnd.Next(1, arrayLength);
            var dropDown = optionsArr[ind].Replace("\r","");
            return dropDown;
        }

        public string GetCountryFromListInMoneyTransferRegistration()
        {
            List<string> countryList = new List<string>
            {
                "Australia",
                "Austria",
                "Belgium",
                "Bulgaria",
                "Canada",
                "Croatia",
                "Cyprus",
                "Czech Republic",
                "Denmark",
                "Estonia",
                "Finland",
                "France",
                "Germany",
                "Greece",
                "Hong Kong",
                "Hungary",
                "Ireland",
                "Israel",
                "Italy",
                "Latvia",
                "Lithuania",
                "Luxembourg",
                "Malta",
                "Netherlands",
                "New Zealand",
                "Norway",
                "Poland",
                "Portugal",
                "Romania",
                "Singapore",
                "Slovakia",
                "Slovenia",
                "South Africa",
                "Spain",
                "Sweden",
                "Switzerland",
                "United Arab Emirates",
                "Other"
            };

            int ind = rnd.Next(1, 38);
            var country = countryList[ind];
            return country;
        }
    }
}
   
