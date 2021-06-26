using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Tests
{
    public class Tests
    {
        private IWebDriver _driver;
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("In setup.");
            _driver = new ChromeDriver("C:\\Users\\No\\Downloads");
        }

        [TearDown]
        public void CloseBrowser()
        {
            _driver.Close();
        }

        [Test]
        public void ListOfMoviesExists()
        {
            _driver.Url = "http://localhost:4200/movies";

            try
            {
                _driver.FindElement(By.XPath("//ion-list"));
                Assert.Pass();
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("The list of movies was not found.");
            }
        }

        [Test]
        public void LoginFormExists()
        {
            _driver.Url = "http://localhost:4200/login";
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            try
            {
                IWebElement name = _driver.FindElement(By.XPath("//ion-input[@name='email']"));
                IWebElement password = _driver.FindElement(By.XPath("//ion-input[@name='password']"));
                Assert.Pass();
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("Login inputs not found!");
            }
        }
    }
}