using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace KPO_Cursovaya.Tests
{
    public class GUITests
    {
        private string chromeDriverPath = @"C:\Users\Влад\Downloads\chromedriver-win32\chromedriver-win32";

        private IWebDriver _driver;
        private readonly string _baseUrl = "https://localhost:7249/";

        public GUITests()
        {
            var chromeOptions = new ChromeOptions();

            // Создание экземпляра ChromeDriver с указанием порта
            var chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);
            chromeDriverService.Port = 7248; // Измененный порт

            _driver = new ChromeDriver(chromeDriverService, chromeOptions);
        }

        [Fact]
        public void TestAuthorizationPage()
        {
            // Открытие страницы в браузере
            _driver.Navigate().GoToUrl(_baseUrl);

            // Дождемся загрузки элементов
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Name("Email")));

            // Нахождение элементов на странице и взаимодействие с ними
            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("vladgus02@inbox.ru");

            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("1111");

            IWebElement loginButton = _driver.FindElement(By.ClassName("login100-form-btn"));
            loginButton.Click();

            // Дождемся перехода на другую страницу
            var profilePageTitle = wait.Until(driver => driver.FindElement(By.TagName("h2")));
            Assert.Equal("Профиль пользователя", profilePageTitle.Text);
        }
        [Fact]
        public void TestRegistrationPage()
        {
            // Открытие страницы регистрации в браузере
            _driver.Navigate().GoToUrl(_baseUrl + "Home/Register"); // Перейти на страницу регистрации

            // Дождаться загрузки элементов
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Name("Email")));

            // Нахождение элементов на странице и взаимодействие с ними
            IWebElement nameField = _driver.FindElement(By.Name("Name"));
            nameField.SendKeys("John Doe");

            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("john@example.com");

            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("password");

            IWebElement passwordRepeatField = _driver.FindElement(By.Name("pass_repeat"));
            passwordRepeatField.SendKeys("password");

            IWebElement registerButton = _driver.FindElement(By.ClassName("login100-form-btn"));
            registerButton.Click();

            // Проверка успешной регистрации (можно проверить переход на другую страницу или появление сообщения об успешной регистрации)
            var successMessage = wait.Until(driver => driver.FindElement(By.TagName("h2")));
            Assert.Equal("Подтверждение почты", successMessage.Text);
        }
        [Fact]
        public void TestVkUserCreate()
        {
            // Открытие страницы в браузере
            _driver.Navigate().GoToUrl(_baseUrl);

            // Дождемся загрузки элементов
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Name("Email")));

            // Нахождение элементов на странице и взаимодействие с ними
            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("vladgus02@inbox.ru");

            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("1111");

            IWebElement loginButton = _driver.FindElement(By.ClassName("login100-form-btn"));
            loginButton.Click();

            // Дождемся перехода на другую страницу
            var profilePageTitle = wait.Until(driver => driver.FindElement(By.TagName("h2")));
            _driver.Navigate().GoToUrl(_baseUrl + "Vk/AddVkUser");

            IWebElement nameField = _driver.FindElement(By.Name("Name"));
            nameField.SendKeys("NewTest");

            IWebElement idField = _driver.FindElement(By.Name("Url"));
            idField.SendKeys("https://vk.com/id152798193");

            IWebElement confirmButton = _driver.FindElement(By.ClassName("btn-primary"));
            confirmButton.Click();

            var vkUsersPageTitle = wait.Until(driver => driver.FindElement(By.CssSelector($".dataPanel.search_vk[data-vk-id='NewTest']")));
            
            Assert.NotNull(vkUsersPageTitle);
        }
        [Fact]
        public void TestCheckStatistic()
        {
            // Открытие страницы в браузере
            _driver.Navigate().GoToUrl(_baseUrl);

            // Дождемся загрузки элементов
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Name("Email")));

            // Нахождение элементов на странице и взаимодействие с ними
            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("admin");

            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("1111");

            IWebElement loginButton = _driver.FindElement(By.ClassName("login100-form-btn"));
            loginButton.Click();

            // Дождемся перехода на другую страницу
            var profilePageTitle = wait.Until(driver => driver.FindElement(By.TagName("h2")));
            _driver.Navigate().GoToUrl(_baseUrl + "Vk");

            // Дождемся загрузки страницы Vk
            wait.Until(driver => driver.FindElement(By.CssSelector(".dataPanel.search_vk")));

            // Найдем элемент кнопки "Статистика" для запроса "For statistic"
            IWebElement statisticButton = _driver.FindElement(By.XPath("//span[text()='Статистика']"));

            // Кликнем на кнопку "Статистика"
            statisticButton.Click();

            // Дождемся загрузки страницы статистики
            wait.Until(driver => driver.FindElement(By.Id("entityChart")));

            Assert.True(true);
        }
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
