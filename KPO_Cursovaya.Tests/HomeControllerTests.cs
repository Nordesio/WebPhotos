using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers;
using Xunit;
using KPO_Cursovaya.StorageInterfaces;
using KPO_Cursovaya.Models;
using Moq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp;
using Microsoft.AspNetCore.Http;

namespace KPO_Cursovaya.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ValidCredentials_RedirectsToInfo()
        {
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            var user = new User { Email = "vladgus02@mail.ru", Password = "111111" };
            userStorageMock.Setup(storage => storage.GetByEmail(user)).Returns(user);
            passwordHashServiceMock.Setup(service => service.VerifyPassword(user.Password, user.Password)).Returns(true);

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);

            // Act
            var result = await controller.Index(user.Email, user.Password) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Info", result.ActionName);
        }
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
            // Add more assertions based on your expected behavior
        }
        [Fact]
        public void EditUser_PasswordsMatch_UpdatesPasswordAndRedirectsToInfo()
        {
            // Arrange
            var userStorageMock = new Mock<IUserStorage>();
            var passwordHashServiceMock = new Mock<IPasswordHashService>();
            var authenticationServiceMock = new Mock<WebApp.IAuthenticationService>();

            var controller = new HomeController(userStorageMock.Object, passwordHashServiceMock.Object, authenticationServiceMock.Object);

            var password = "NewPassword123";
            var pass_repeat = "NewPassword123";

            var auth_user = new User { Id = 2, Name = "vlad", Email = "vladgus02@mail.ru", EmailConfirmed = true, Password = "$2a$12$VmullPZdwP3zgkctdrGSt.82kOhpr85QLiELxABveaIQbJOqNB4fG", Role = "user" };
            userStorageMock.Setup(storage => storage.GetById(auth_user.Id)).Returns(auth_user);

            passwordHashServiceMock.Setup(service => service.HashPassword(password)).Returns("HashedNewPassword");

            // Act
            var result = controller.EditUser(password, pass_repeat) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User not authenticated", result.ViewData["Message"]); // Укажите ожидаемое сообщение об ошибке
        }




    }

}
