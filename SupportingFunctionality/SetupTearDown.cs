using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Gurock.TestRail;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using  OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow.Tracing;


namespace Automation_Testing
{
    [Binding]
    public class SetupTearDown
    {
        public static IWebDriver Driver;
        private static string runId;
        public string CaseName { get; set; }
        private static APIClient client;
        public string screenShotFilePath;
        public string fullCaseName;
        public static string projectId = ConfigurationManager.AppSettings["TestProjectId"];
        public static bool create = Convert.ToBoolean(ConfigurationManager.AppSettings["CreateTestRail"]);
        
        //Test Rail Run creation
        [BeforeTestRun]
        public static void CreateTestRun()
        {
            if (create)
            {
                client = GetApiClient();


                JArray secs = client.SendGet(string.Format("get_sections/{0}", projectId)) as JArray;


                var p = client.SendGet("get_projects");

                DateTime dateTime = DateTime.Now;
                Dictionary<String, String> d = new Dictionary<string, string>();
                d.Add("name", ConfigurationManager.AppSettings["Sprint_Number"] + " " + dateTime);

                JObject res = client.SendPost(string.Format("add_run/{0}", projectId), d) as JObject;
                runId = res.GetValue("id").Value<string>();
            }
        }

        //Test Rail Sections deletion just to get the report
        //[AfterTestRun]
        //public static void CloseTestRun()
        //{
        //    if (create)
        //    {
        //        Dictionary<String, String> data = new Dictionary<string, string>();
        //        data.Add("title", "Test Runclosed");
        //        client.SendPost(string.Format("close_run/{0}", runId), data);
        //        var sections = client.SendGet("get_sections/7") as JArray;
        //        var childSections = sections.Where(sec => ((JObject)sec).GetValue("parent_id").Value<string>() != null);
        //        var parentSections = sections.Where(sec => ((JObject)sec).GetValue("parent_id").Value<string>() == null);

        //        if (childSections != null)
        //        {
        //            foreach (var jToken in childSections)
        //            {
        //                var section = (JObject)jToken;
        //                var sectionId = section.GetValue("id").Value<string>();
        //                client.SendPost(string.Format("delete_section/{0}", sectionId), data);
        //            }
        //        }

        //        if (parentSections != null)
        //        {
        //            foreach (var jToken in parentSections)
        //            {
        //                var section = (JObject)jToken;
        //                var sectionId = section.GetValue("id").Value<string>();
        //                client.SendPost(string.Format("delete_section/{0}", sectionId), data);
        //            }
        //        }
        //    }
        //}

        private static APIClient GetApiClient()
        {
            return new APIClient("http://cax-firebuild:90/") { Password = "Mohana12!", User = "karthick.ganesan@caxtonfx.com" };
        }

       
        [BeforeScenario]
        public void SetUp()
        {
           if (Driver == null)
                {
                    //ITestRunner tr = TestRunnerManager.GetTestRunner();

                    //var f = FeatureContext.Current;
                    //var fi = f.FeatureInfo;
                    //var c = ScenarioContext.Current;
                    //var i = c.ScenarioInfo;

                    var sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["Sql.Connection"]);
                    sqlConnection.Open();

                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandText =
                        "Delete from [FirebirdFx-TEST].[Payment].[Transfer] where userid = 'karthickg36@gmail.com' " +
                        "Delete  from [CurrencyBank].[TradeDetail] where tradeid in" +
                        "(select ID from [CurrencyBank].[Trade] where userid = 'karthickg36@gmail.com')" +
                        "Delete from [CurrencyBank].[Trade] where userid = 'karthickg36@gmail.com'" +
                        "Delete  from [CurrencyBank].[AccountTransactionDetail] where AccounttransactionId in" +
                        "(select Id from [CurrencyBank].[AccountTransaction]where AccountsummaryID IN" +
                        "(select ID from [CurrencyBank].[AccountSummary] where userid = 'karthickg36@gmail.com'))" +
                        "Delete  from  [CurrencyBank].[AccountTransaction] where AccountsummaryID IN" +
                        "(select ID from [CurrencyBank].[AccountSummary] where userid = 'karthickg36@gmail.com')" +
                        "Delete  from [CurrencyBank].[AccountSummary] where userid = 'karthickg36@gmail.com'" +
                        "Delete  from [CurrencyBank].[ForwardTradeDetail]where tradeId in" +
                        "(select ID from [CurrencyBank].[ForwardTrade] where userid = 'karthickg36@gmail.com')" +
                        "Delete from [CurrencyBank].[ForwardTrade] where userid = 'karthickg36@gmail.com'" +
                        "Delete  from [CurrencyBank].[FlexTradeDetail]where FlexTradeId in" +
                        "(select ID from [CurrencyBank].[FlexTrade] where userid = 'karthickg36@gmail.com')" +
                        "Delete from [CurrencyBank].[FlexTrade] where userid = 'karthickg36@gmail.com'";

                    command.ExecuteNonQuery();
                    sqlConnection.Close();


                    Driver = new ChromeDriver(@".");
                    Driver.Manage().Window.Maximize();
                }
            }
        

        //Test Rail
        [AfterScenario]
        public void AfterWebTest()
        {
            if (create)
            {
                if (Driver != null)
                {
                    var f = FeatureContext.Current;
                    var fi = f.FeatureInfo;
                    var featureTitle = fi.Title;
                    var c = ScenarioContext.Current;
                    var ci = c.ScenarioInfo;
                    var subsectionTitle = "";
                    if(ci.Tags != null && ci.Tags.Any())
                     subsectionTitle = ci.Tags[0];

                    // Creation of Sections
                    string secId = string.Empty;
                    string suiteid = string.Empty;
                    JArray secs = client.SendGet(string.Format("get_sections/{0}", projectId)) as JArray;
                    if (secs.Any(sec => sec.Value<string>("name") == featureTitle))
                    {
                        JToken mycase = secs.First(cc => cc.Value<string>("name") == featureTitle);
                        secId = mycase.Value<string>("id");
                        suiteid = mycase.Value<string>("suite_id");

                    }
                    else
                    {
                        Dictionary<String, String> data = new Dictionary<string, string>();
                        data.Add("name", featureTitle);
                        JObject res = client.SendPost(string.Format("add_section/{0}",projectId), data) as JObject;
                        secId = res.Value<string>("id");
                        suiteid = res.Value<string>("suite_id");
                    }

                    // Creation of Subsection
                    string subsecId = string.Empty;

                    JArray subSection = null;

                    if (ci.Tags != null && ci.Tags.Any())
                    {
                        subSection = client.SendGet(string.Format("get_sections/{0}&section_id={1}", projectId, secId)) as JArray;                        
                        foreach (var item in ci.Tags)
                        {
                            if (subSection.Any(s => s.Value<string>("name") == subsectionTitle))
                            {
                                JToken mycase = secs.First(cc => cc.Value<string>("name") == subsectionTitle);
                                subsecId = mycase.Value<string>("id");
                            }
                            else
                            {
                                Dictionary<String, String> data = new Dictionary<string, string>();
                                data.Add("name", subsectionTitle);
                                data.Add("parent_id", secId);
                                data.Add("suite_id", suiteid);
                                //data.Add("depth", "0");
                                //JObject res = client.SendPost(string.Format("add_section/13&section_id={0}", secId), data) as JObject;
                                JObject res = client.SendPost(string.Format("add_section/{0}", projectId), data) as JObject;
                                subsecId = res.Value<string>("id");
                            }

                        }                        
                    }

                    // Creation of Test case
                    if (CaseName != null)
                    {
                        fullCaseName = (ci.Title + " / " + CaseName).Trim();
                    }

                    else
                    {
                        fullCaseName = (ci.Title).Trim();
                    }


                    string caseId;
                    JArray cases = client.SendGet(string.Format("get_cases/{0}&section_id={1}", projectId, secId)) as JArray;

                    //check in main section
                    if (cases.Any(cc => cc.Value<string>("title") == fullCaseName))
                    {
                        JToken mycase = cases.First(cc => cc.Value<string>("title") == fullCaseName);
                        caseId = mycase.Value<string>("id");
                    }
                    else
                    {
                        //Not in main secion
                        //Now check in subsecion
                        if (subSection != null)
                        {
                            JArray casesInSubSection =
                                client.SendGet(string.Format("get_cases/{0}&section_id={1}", projectId, subsecId)) as JArray;
                            if (casesInSubSection.Any(cs => cs.Value<string>("title") == fullCaseName))
                            {
                                JToken mycase = casesInSubSection.First(cc => cc.Value<string>("title") == fullCaseName);
                                caseId = mycase.Value<string>("id");
                            }
                            else
                            {
                                Dictionary<String, String> data = new Dictionary<string, string>();
                                data.Add("title", fullCaseName);
                                JObject resSub =
                                    client.SendPost(string.Format("add_case/{0}", subsecId), data) as JObject;
                                caseId = resSub.GetValue("id").Value<string>();
                            }
                        }

                        //if not in subsection and main section, write in main section
                        else
                        {
                            Dictionary<String, String> data = new Dictionary<string, string>();
                            data.Add("title", fullCaseName);
                            JObject res = client.SendPost(string.Format("add_case/{0}", secId), data) as JObject;
                            caseId = res.GetValue("id").Value<string>();
                        }
                    }

                    Dictionary<String, String> testData = new Dictionary<string, string>();
                    if (c.TestError != null)
                    {
                        TakeScreenshot(Driver);
                        testData.Add("status_id", "5");
                        if (screenShotFilePath != null)
                        {
                            testData.Add("comment",
                                string.Format("{0}\n{1}", c.TestError.Message, new Uri(screenShotFilePath)));
                        }
                    }
                  
                    else
                    {
                        testData.Add("status_id", "1");
                    }

                    var result = client.SendPost("add_result_for_case/" + runId + "/" + caseId, testData); ;

                    Driver.Quit();
                    Driver = null;

                }
            }
            //Driver.Quit();
            //Driver = null;
        }

       
        private void TakeScreenshot(IWebDriver driver)
        {
            try
            {
                string fileNameBase = string.Format("error_{0}_{1}_{2}",
                                                    FeatureContext.Current.FeatureInfo.Title.ToIdentifier(),
                                                    ScenarioContext.Current.ScenarioInfo.Title.ToIdentifier(),
                                                    DateTime.Now.ToString("yyyyMMdd_HHmmss"));

                var artifactDirectory = Path.Combine(Directory.GetCurrentDirectory(), "testresults");
                var artifactDirectory1 = Path.Combine("Z:\\AutomationTestScreenshots", "testresults");
                if (!Directory.Exists(artifactDirectory))
                    Directory.CreateDirectory(artifactDirectory);

                string pageSource = driver.PageSource;
                string sourceFilePath = Path.Combine(artifactDirectory, fileNameBase + "_source.html");
                File.WriteAllText(sourceFilePath, pageSource, Encoding.UTF8);
                Console.WriteLine("Page source: {0}", new Uri(sourceFilePath));

                ITakesScreenshot takesScreenshot = driver as ITakesScreenshot;

                if (takesScreenshot != null)
                {
                    var screenshot = takesScreenshot.GetScreenshot();

                    screenShotFilePath = Path.Combine(artifactDirectory, fileNameBase + "_screenshot.png");
                    string screenshotFilePath1 = Path.Combine(artifactDirectory1, fileNameBase + "_screenshot.png");
                    screenshot.SaveAsFile(screenShotFilePath, ImageFormat.Png);
                    //screenshot.SaveAsFile(screenshotFilePath1, ImageFormat.Png);

                    Console.WriteLine("Screenshot: {0}", new Uri(screenShotFilePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while taking screenshot: {0}", ex);
            }

        }
    }
}  

        
    
   
   

