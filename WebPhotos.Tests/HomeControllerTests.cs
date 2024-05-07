using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers;
using Xunit;
using DbData.StorageInterfaces;
using DbData.Models;
using Moq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp;
using Microsoft.AspNetCore.Http;
using Xunit.Runner.Reporters;


namespace KPO_Cursovaya.Tests
{


    public class HomeControllerTests
    {
        [Fact]
        public void EditUser_ValidPassword_ReturnsRedirectToInfo()
        {
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);

            var validPassword = "NewPassword123";
            var validPassRepeat = "NewPassword123";

            // Установка auth_user для проверки аутентификации пользователя
            var auth_user = new User { Id = 1, Name = "vlad", Email = "vladgus02@mail.ru", EmailConfirmed = true, Password = "$2a$12$wcOYafjgV4tHyrAgWRgGoeD6zi.EB7VT15SDH9KWLQeikta6nBP8a", Role = "user" };
            HomeController.auth_user = auth_user;

            // Настройка моков
            userStorageMock.Setup(storage => storage.GetById(auth_user.Id)).Returns(auth_user);
            passwordHashServiceMock.Setup(service => service.HashPassword(validPassword)).Returns("someHashedPassword");

            // Act
            var result = controller.EditUser(validPassword, validPassRepeat) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(HomeController.Info), result.ActionName);
        }


        // Тестирование с неверным Email
        [Fact]
        public async Task Index_InvalidEmail_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "password";
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            var user = new User { Email = email, Password = password };
            userStorageMock.Setup(storage => storage.GetByEmail(user)).Returns(user);
            passwordHashServiceMock.Setup(service => service.VerifyPassword(user.Password, user.Password)).Returns(true);

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);
            // Act
            var result = await controller.Index(email, password) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User does not exist!", result.ViewData["Message"]);
        }
        // Тестирование неверного пароля
        [Fact]
        public async Task Index_InvalidPassword_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var email = "vladgus02@mail.ru";
            var correctPassword = "correctPassword"; // Правильный пароль

            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<WebApp.IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            var user = new User { Email = email, Password = correctPassword }; // Подставляем правильный пароль
            userStorageMock.Setup(storage => storage.GetByEmail(It.IsAny<User>())).Returns(user);

            // Подставляем хэшированный пароль
            passwordHashServiceMock.Setup(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hashedPassword) => password == correctPassword); // Сравниваем входной пароль с правильным

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);

            // Act
            var result = await controller.Index(email, "incorrectPassword") as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Password is not correct!", result.ViewData["Message"]);
        }


        // Тестирование незаполненных Email и Password
        [Fact]
        public async Task Index_EmptyEmailOrPassword_ReturnsViewWithErrorMessage()
        {
            // Arrange

            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            var user = new User { Email = "", Password = "" };
            userStorageMock.Setup(storage => storage.GetByEmail(user)).Returns(user);
            passwordHashServiceMock.Setup(service => service.VerifyPassword(user.Password, user.Password)).Returns(true);

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);

            // Act
            var result = await controller.Index(null, null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Input password/login", result.ViewData["Message"]);
        }
        // Тестирование успешной аутентификации
        [Fact]
        public async Task Index_ValidCredentials_RedirectsToInfo()
        {
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            // Подстановка любого объекта User при вызове GetByEmail
            userStorageMock.Setup(storage => storage.GetByEmail(It.IsAny<User>())).Returns(new User());

            // Проверка на совпадение пароля
            passwordHashServiceMock.Setup(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);

            // Act
            var result = await controller.Index("vladgus02@mail.ru", "1111") as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Info", result.ActionName);
        }
        // Тестирование регистрации
        [Fact]
        public async Task Register_ValidInput_RedirectsToDoubleAuth()
        {
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);

            var name = "John";
            var email = "john@example.com";
            var password = "Password123";
            var pass_repeat = "Password123";

            userStorageMock.Setup(storage => storage.GetByEmail(It.IsAny<User>())).Returns((User)null);
            passwordHashServiceMock.Setup(service => service.HashPassword(password)).Returns("HashedPassword");

            // Act
            var result = await controller.Register(name, email, password, pass_repeat) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("DoubleAuth", result.ActionName);

        }
        // Тестирование удаления через администратора
        [Fact]
        public async Task Delete_ValidInput_ViaAdministrator()
        {
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();

            var controller = new AdminController(userStorageMock.Object, passwordHashServiceMock.Object);

            var userId = 1; // Замените на ID пользователя, которого вы хотите удалить

            userStorageMock.Setup(storage => storage.GetById(userId)).Returns(new User { Id = userId });

            // Act
            var result = await controller.UserDelete(userId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result.Model); // Проверяем, что модель передана в представление
        }
        // Тестирование редактирования через администратора
        [Fact]
        public async Task Edit_ValidInput_ViaAdministrator()
        {
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();

            var controller = new AdminController(userStorageMock.Object, passwordHashServiceMock.Object);

            // Создаем фиктивного пользователя для редактирования
            var userToEdit = new User
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Password = "oldpassword"
            };

            // Устанавливаем ожидаемое поведение метода GetById для возврата фиктивного пользователя
            userStorageMock.Setup(storage => storage.GetById(userToEdit.Id)).Returns(userToEdit);

            // Act: Выполняем редактирование пользователя
            var result = await controller.UserEdit(userToEdit) as RedirectToActionResult;

            // Assert: Проверяем, что контроллер перенаправляет на нужную страницу после редактирования
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            // Проверяем, что редактирование успешно обновило данные пользователя
            userStorageMock.Verify(storage => storage.Update(userToEdit), Times.Once);
        }

    }

}
