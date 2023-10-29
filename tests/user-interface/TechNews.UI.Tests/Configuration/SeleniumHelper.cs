using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TechNews.UI.Tests.Configuration;

public class SeleniumHelper : IDisposable
{
    private readonly IWebDriver WebDriver;
    public WebDriverWait Wait;

    public SeleniumHelper(Browser browser, bool headless = true)
    {
        WebDriver = WebDriverFactory.CreateWebDriver(browser, headless);

        WebDriver.Manage().Window.Maximize();
        WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(EnvironmentVariables.MaxSecondsWaitingForPage);

        Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(EnvironmentVariables.MaxSecondsWaitingForPage));
    }

    public string GetUrl()
    {
        return WebDriver.Url;
    }

    public bool ContainsInUrl(string text)
    {
        return Wait.Until(ExpectedConditions.UrlContains(text));
    }

    public void GoToUrl(string path)
    {
        WebDriver.Navigate().GoToUrl($"{EnvironmentVariables.TechNewsWebUri}/{path}");
    }

    public void ClickLinkByText(string linkText)
    {
        Wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(linkText)))
            .Click();
    }

    public IWebElement GetElementByClassName(string className)
    {
        return Wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(className)));
    }

    public IEnumerable<IWebElement> GetElementsByClassName(string className)
    {
        return Wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName(className)));
    }

    public IWebElement GetElementByXPath(string xPath)
    {
        return Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
    }

    public IWebElement GetElementById(string id)
    {
        return Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));
    }

    public string GetElementTextById(string id)
    {
        return Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id))).Text;
    }
    
    public string GetElementAttribute(string elementId, string attributeName)
    {
        return Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(elementId))).GetAttribute(attributeName);
    }

    public string GetElementTextByXPath(string xPath)
    {
        return Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath))).Text;
    }

    public string GetElementValueById(string id)
    {
        return GetElementAttribute(id, "value");
    }

    public string GetElementInnerHtmlById(string id)
    {
        return GetElementAttribute(id, "innerHTML");
    }

    public bool ElementExistsById(string id)
    {
        return ElementExists(By.Id(id));
    }

    public void FillTextInputById(string id, string value)
    {
        Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)))
            .SendKeys(value);
    }

    public void FillDropDownById(string id, string value)
    {
        var element = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));

        var selectElement = new SelectElement(element);
        selectElement.SelectByValue(value);
    }

    public void ClickElementById(string id)
    {
        Wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)))
            .Click();
    }

    public void ClickElementByXPath(string xPath)
    {
        Wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xPath)))
            .Click();
    }

    public void ReturnNavigation(int numberOfTimes = 1)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            WebDriver.Navigate().Back();
        }
    }

    public void GetScreenshot(string fileName)
    {
        SaveScreenshot(WebDriver.TakeScreenshot(), $"{DateTime.UtcNow.ToFileTimeUtc}_{fileName}.png");
    }

    private bool ElementExists(By by)
    {
        try
        {
            WebDriver.FindElement(by);
            return true;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    private void SaveScreenshot(Screenshot screenshot, string fileName)
    {
        screenshot.SaveAsFile($"{EnvironmentVariables.ScreenshotsFolderPath}{fileName}");
    }
    
    public bool CheckElementHasClass(By by, string className)
    {
        return Wait.Until(ExpectedConditions.ElementIsVisible(by))
                .GetAttribute("class")
                .Contains(className);
    }
    
    public void Dispose()
    {
        WebDriver.Quit();
        WebDriver.Dispose();
        GC.SuppressFinalize(this);
    }
}
