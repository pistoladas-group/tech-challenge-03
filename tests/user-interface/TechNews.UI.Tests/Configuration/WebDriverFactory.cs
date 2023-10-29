using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace TechNews.UI.Tests.Configuration;

public enum Browser
{
    Chrome = 1
}

public static class WebDriverFactory
{
    public static IWebDriver CreateWebDriver(Browser browser, bool headless)
    {
        IWebDriver? webDriver = null;

        switch (browser)
        {
            case Browser.Chrome:
                var options = new ChromeOptions();

                if (headless)
                {
                    options.AddArguments("--headless", "--disable-gpu", "--window-size=1920,1200", "--ignore-certificate-errors");
                }

                webDriver = new ChromeDriver(options);

                break;
        }

        if (webDriver is null)
        {
            throw new NullReferenceException("Browser not supported for creating WebDriver");
        }

        return webDriver;
    }
}
