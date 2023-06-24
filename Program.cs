using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main(string[] args)
    {
        // Инициализация ChromeDriver
        var options = new ChromeOptions();
        //options.AddArgument("--headless"); // для выполнения в фоновом режиме
        var driver = new ChromeDriver(options);

        try
        {
            // Зайти на https://www.nseindia.com
            driver.Navigate().GoToUrl("https://www.nseindia.com");

            driver.Manage().Cookies.DeleteAllCookies();

            // Навестись (hover) на MARKET DATA
            var marketDataMenu = driver.FindElement(By.XPath("//*[@id='link_2']"));
            var actions = new OpenQA.Selenium.Interactions.Actions(driver);
            actions.MoveToElement(marketDataMenu).Perform();
            driver.Manage().Cookies.DeleteAllCookies();

            // Кликнуть на Pre-Open Market
            var preOpenMarketLink = driver.FindElement(By.XPath("//*[@id='main_navbar']/ul/li[3]/div/div[1]/div/div[1]/ul/li[1]/a"));
            preOpenMarketLink.Click();


            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            // Спарсить данные Final Price и записать их в CSV файл
            var finalPriceElements = driver.FindElements(By.XPath("//*[@id='livePreTable']/tbody/tr/td[7]"));
            var nameElements = driver.FindElements(By.XPath("//*[@id='livePreTable']/tbody/tr/td[2]"));

            using (var writer = new StreamWriter("data.csv"))
            {
                for (int i = 0; i < finalPriceElements.Count; i++)
                {
                    var name = nameElements[i].Text.Trim();
                    var price = finalPriceElements[i].Text.Trim();

                    writer.WriteLine($"{name};{price}");
                }
            }

            Console.WriteLine("Данные успешно спарсены и записаны в файл data.csv.");

            // Имитация пользовательского сценария
            driver.Manage().Cookies.DeleteAllCookies();
            // Зайти на главную страницу
            driver.Navigate().GoToUrl("https://www.nseindia.com");

            // Пролистать вниз до графика
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 1000)");

            // Выбрать график 
            var niftyBankChartLink = driver.FindElement(By.XPath("/html/body/div[10]/div[1]/div[1]/div/div/section/div/div/div/div/div[1]/nav/div/div/a[3]"));
            new Actions(driver).MoveToElement(niftyBankChartLink).Click().Perform();

            driver.Manage().Cookies.DeleteAllCookies();

            // Нажать "View all"
            var viewAllLink = driver.FindElement(By.XPath("//*[@id=\"gainers_loosers\"]/div[3]/a"));
            new Actions(driver).MoveToElement(viewAllLink).Click().Perform();
           // driver.Manage().Cookies.DeleteAllCookies();


            // Пролистать таблицу до конца
            WebDriverWait webDriver = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            var freezedRow = webDriver.Until(e => e.FindElement(By.CssSelector("#equityStockTable > tbody > tr.freezed-row")));
            var table = driver.FindElement(By.XPath("//*[@id=\"equity-stock\"]/div[4]/div"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollTop = arguments[0].scrollHeight", table);

            Console.WriteLine("Имитация пользовательского сценария завершена.");

            // Закрытие браузера
            driver.Quit();
        }
        catch (Exception ex)
        {
            // Обработка исключений
            Console.WriteLine($"Ошибка: {ex.Message}");
            driver.Quit();
        }
    }
}