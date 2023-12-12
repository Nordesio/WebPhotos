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

    }

}
