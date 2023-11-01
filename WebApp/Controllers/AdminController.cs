using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using KPO_Cursovaya.Models;
using KPO_Cursovaya.Implements;
using KPO_Cursovaya.StorageInterfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserStorage _userStorage;
        public static User auth_user_admin;
        
    
        public AdminController(IUserStorage userStorage)
        {
            _userStorage = userStorage;
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Index()
        {
            auth_user_admin = HomeController.auth_user;
            if (auth_user_admin == null)
            {
                ViewBag.Message = "You are not authorised";
                return Index();
                //throw new Exception("Вы не авторизованы!");
            }
            if (auth_user_admin.Role == "user")
            {
                ViewBag.Message = "You aren't admin";
                return Index();
                //throw new Exception("Вы не админ!");
            }
          
            return View(_userStorage.GetFullList());
        }


        /*
        [HttpGet]
        public async Task<IActionResult> UserDelele(string? id, string? surname, string? patronymic, DateOnly? birthday, string? email, bool emailConfirmed, string? passwordHash,
            string? securityStamp, string? phoneNumber, bool phoneNumberConfirmed, bool twoFactorEnabled, DateOnly? lockoutEndDateUtc, bool lockoutEnabled,
            int accessFailedCount, string userName, int? userGroupId)
        {
            var model = new User();
            model.Id = id;
            model.Surname = surname;
            model.Patronymic = patronymic;
            model.Birthday = birthday;
            model.Email = email;
            model.EmailConfirmed = emailConfirmed;
            model.PasswordHash = passwordHash;
            model.SecurityStamp = securityStamp;
            model.PhoneNumber = phoneNumber;
            model.PhoneNumberConfirmed = phoneNumberConfirmed;
            model.TwoFactorEnabled = twoFactorEnabled;
            model.LockoutEndDateUtc = lockoutEndDateUtc;
            model.LockoutEnabled = lockoutEnabled;
            model.AccessFailedCount = accessFailedCount;
            model.UserName = userName;
            model.UserGroupId = userGroupId;
            return View(model);
        }
        */
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UserDelete(int Id)
        {
            return View(_userStorage.GetById(Id));
        }
       [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(User user)
        {
            _userStorage.Delete(user);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> UserEdit(int Id)
        {
            return View(_userStorage.GetById(Id));
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(User user)
        {
            _userStorage.Update(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
