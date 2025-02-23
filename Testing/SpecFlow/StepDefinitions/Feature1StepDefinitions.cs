using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SpecFlowBDDAutomationFramework.StepDefinitions
{
    [Binding]
    public sealed class Feature1StepDefinitions
    {
        private IWebDriver driver;

       public Feature1StepDefinitions(IWebDriver driver)
        {
            this.driver = driver;
        }


        [Given(@"Open the browser")]
        public void GivenOpenTheBrowser()
        {
            //string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            //driver = new ChromeDriver(path + @"\drivers\");
            //driver.Manage().Window.Maximize();
        }

        [When(@"Enter the URL")]
        public void WhenEnterTheURL()
        {
            driver.Url = "https://www.youtube.com/";
            Thread.Sleep(3000);
            
        }

        [Then(@"Search for the Testers Talk")]
        public void ThenSearchForTheTestersTalk()
        {
            driver.FindElement(By.XPath("//*[@name='search_query']")).SendKeys(" Testers Talk ");
            driver.FindElement(By.XPath("//*[@name='search_query']")).SendKeys(Keys.Enter);
            Thread.Sleep(5000);
            driver.Quit();
            driver.Dispose();
        }

    }
}