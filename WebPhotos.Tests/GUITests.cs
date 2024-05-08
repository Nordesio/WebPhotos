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

            // Нахождение поля почты
            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("vladgus02@inbox.ru");
            // Нахождение поля пароля
            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("1111");
            // Ищем и нажимаем кнопку логина
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

            // Ищем поле имени и заполяем
            IWebElement nameField = _driver.FindElement(By.Name("Name"));
            nameField.SendKeys("John Doe");
            // Ищем поле почты и заполяем
            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("john@example.com");
            // Ищем поле пароля и заполяем
            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("password");
            // Ищем поле повтора пароля и заполяем
            IWebElement passwordRepeatField = _driver.FindElement(By.Name("pass_repeat"));
            passwordRepeatField.SendKeys("password");
            // Ищем кнопку регистрации и нажимаем её
            IWebElement registerButton = _driver.FindElement(By.ClassName("login100-form-btn"));
            registerButton.Click();

            // Проверка успешной регистрации (переход на страницу двойоной аутентификации)
            var successMessage = wait.Until(driver => driver.FindElement(By.TagName("h2")));
            Assert.Equal("Подтверждение почты", successMessage.Text);
        }
        [Fact]
        public void TestVkUserCreate()
        {
            // Открытие страницы в браузере
            _driver.Navigate().GoToUrl(_baseUrl);

            // Ждем загрузки элементов
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.Name("Email")));

            // Ищем поле почты и заполняем
            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("vladgus02@inbox.ru");
            // Ищем поле пароля и заполняем
            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("1111");
            // Ищем кнопку входа и нажимаем её
            IWebElement loginButton = _driver.FindElement(By.ClassName("login100-form-btn"));
            loginButton.Click();

            // Ждем переход на другую страницу
            var profilePageTitle = wait.Until(driver => driver.FindElement(By.TagName("h2")));
            _driver.Navigate().GoToUrl(_baseUrl + "Vk/AddVkUser");
            // Ищем поле названия запроса и заполняем
            IWebElement nameField = _driver.FindElement(By.Name("Name"));
            nameField.SendKeys("NewTest");
            // Ищем поле ссылки на пользователя и заполняем
            IWebElement idField = _driver.FindElement(By.Name("Url"));
            idField.SendKeys("https://vk.com/id152798193");
            // Ищем кнопку подтверждения создания запроса и нажимаем
            IWebElement confirmButton = _driver.FindElement(By.ClassName("btn-primary"));
            confirmButton.Click();
            // Ищем в списке новый запрос с таким названием
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

            // Ищем поле почты и заполняем
            IWebElement emailField = _driver.FindElement(By.Name("Email"));
            emailField.SendKeys("admin");
            // Ищем поле пароля и заполняем
            IWebElement passwordField = _driver.FindElement(By.Name("Password"));
            passwordField.SendKeys("1111");
            // Ищем кнопку входа и нажимаем
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
