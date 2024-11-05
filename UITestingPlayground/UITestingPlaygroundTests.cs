using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void TestButtonHiddenLayers()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/hiddenlayers");

            var greenButton = driver.FindElement(By.Id("greenButton"));
            greenButton.Click();

            try
            {
                greenButton.Click();
                Assert.Fail("Green button should not be clickable again after the first click.");
            }
            catch (WebDriverException)
            {
                Assert.Pass("Green button cannot be clicked twice as expected.");
            }
        }

        [Test]
        public void TestClick()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/click");

            var button = driver.FindElement(By.Id("badButton"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(button).Click().Perform();

            string buttonClass = button.GetAttribute("class");
            Assert.That(buttonClass.Contains("btn-success"), Is.True, "The button did not turn green after clicking.");
        }

        [Test]
        public void TextInput()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/textinput");

            var inputField = driver.FindElement(By.Id("newButtonName"));
            var button = driver.FindElement(By.Id("updatingButton"));

            Actions actions = new Actions(driver);
            actions.Click(inputField)
                .SendKeys("Hello World")
                .Perform();

            button.Click();

            string buttonText = button.Text;
            Assert.That(buttonText, Is.EqualTo("Hello World"), "The button name did not change as expected.");
        }

        [Test]
        public void TestAlerts()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/alerts");

            var alertButton = driver.FindElement(By.Id("alertButton"));
            alertButton.Click();
            driver.SwitchTo().Alert().Accept();

            var confirmButton = driver.FindElement(By.Id("confirmButton"));
            confirmButton.Click();
            Thread.Sleep(1000);
            driver.SwitchTo().Alert().Accept();

            var promptButton = driver.FindElement(By.Id("promptButton"));
            promptButton.Click();
            Thread.Sleep(1000);
            var alert = driver.SwitchTo().Alert();
            alert.SendKeys("hello");
            alert.Accept();
        }

        [Test]
        public void TestDisabledInput()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/disabledinput");

            var enableButton = driver.FindElement(By.Id("enableButton"));
            enableButton.Click();

            var inputField = driver.FindElement(By.Id("inputField"));
            var waitTime = TimeSpan.FromSeconds(0);
            while (inputField.GetAttribute("disabled") == "true" && waitTime.TotalSeconds < 10)
            {
                Thread.Sleep(500);
                waitTime += TimeSpan.FromMilliseconds(500);
            }

            inputField.SendKeys("Hello World!");

            Assert.That(inputField.GetAttribute("value"), Is.EqualTo("Hello World!"), "The input field value was not set correctly.");
        }

        [Test]
        public void TestAnimatedButton()
        {
            driver.Navigate().GoToUrl("http://www.uitestingplayground.com/animation");

            var startAnimationButton = driver.FindElement(By.Id("animationButton"));
            startAnimationButton.Click();

            var movingTargetButton = driver.FindElement(By.Id("movingTarget"));
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => !movingTargetButton.GetAttribute("class").Contains("spin"));

            movingTargetButton.Click();

            var statusLabel = driver.FindElement(By.Id("opstatus"));
            string statusText = statusLabel.Text;

            Assert.That(statusText, Does.Not.Contain("spin"), "Moving Target should not have the 'spin' class after clicking.");
        }


        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
