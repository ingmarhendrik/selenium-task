using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Drawing;

namespace UITestingPlaygroundTests
{
    [TestFixture]
    public class HiddenLayersTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver(@"C:\Users\Kasutaja\Documents\Programmeerimine\Testing Class\UITestingPlayground\UITestingPlayground\drivers");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Test]
        public void TestHiddenLayers()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/hiddenlayers");

            var greenButton = driver.FindElement(By.Id("greenButton"));
            greenButton.Click();

            try
            {
                greenButton.Click();
                Assert.Fail("The green button should be disabled after the first click");
            }
            catch (WebDriverException)
            {
                Assert.Pass("Check passed, as the green button cannot be clicked twice.");
            }
        }

        [Test]
        public void TestClick()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/click");

            var button = driver.FindElement(By.Id("badButton"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(button).Click().Build().Perform();

            string updatedClass = button.GetAttribute("class");
            Assert.That(updatedClass.Contains("btn-success"), Is.True, "The button did not change to green after being clicked.");
        }

        [Test]
        public void TestInput()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/textinput");

            var textInput = driver.FindElement(By.Id("newButtonName"));
            var button = driver.FindElement(By.Id("updatingButton"));

            Actions actions = new Actions(driver);
            actions.Click(textInput)
                .SendKeys("Cookie")
                .Build()
                .Perform();

            button.Click();

            string buttonText = button.Text;
            Assert.That(buttonText, Is.EqualTo("Cookie"), "The button text did not update correctly.");
        }

        [Test]
        public void TestAlerts()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/alerts");

            var alertBtn = driver.FindElement(By.Id("alertButton"));
            alertBtn.Click();
            driver.SwitchTo().Alert().Accept();

            var confirmBtn = driver.FindElement(By.Id("confirmButton"));
            confirmBtn.Click();
            Thread.Sleep(1000);
            driver.SwitchTo().Alert().Accept();

            var promptBtn = driver.FindElement(By.Id("promptButton"));
            promptBtn.Click();
            Thread.Sleep(1000);
            var promptAlert = driver.SwitchTo().Alert();
            promptAlert.SendKeys("hello");
            promptAlert.Accept();
        }


        [Test]
        public void TestProgressBar()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/progressbar");

            var startButton = driver.FindElement(By.Id("startButton"));
            startButton.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            int progressValue = 0;
            wait.Until(d =>
            {
                var progressBar = driver.FindElement(By.Id("progressBar"));
                string progressText = progressBar.Text.Replace("%", "");
                progressValue = int.Parse(progressText);
                return progressValue >= 73;
            });

            if (progressValue >= 75)
            {
                var stopButton = driver.FindElement(By.Id("stopButton"));
                stopButton.Click();
            }

            var finalProgressBar = driver.FindElement(By.Id("progressBar"));
            string finalProgressText = finalProgressBar.Text.Replace("%", "");
            int finalProgressValue = int.Parse(finalProgressText);

            Assert.That(finalProgressValue, Is.EqualTo(75).Within(5),
                        $"Expected the progress bar to be close to 75%, but it was {finalProgressValue}%.");
        }

        [Test]
        public void TestAjaxData()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/ajax");

            var ajaxButton = driver.FindElement(By.Id("ajaxButton"));
            ajaxButton.Click();

            wait.Until(d => !d.FindElement(By.Id("spinner")).Displayed);

            var label = wait.Until(d => d.FindElement(By.CssSelector("#content .bg-success")));
            label.Click();

            string labelText = label.Text;
            Assert.That(labelText, Is.Not.Empty, "The label text should not be empty after AJAX request.");
        }



        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
