using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using DbData.StorageInterfaces;
using DbData.Models;
using DbData.Implements;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt;
using VkNet;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public static string token = "vk1.a.5LeMmhdBGcpI1FeTYvTbtR4i1fM15tSmaGAJQdpFuoFRgWcpgT9gNv5sJkYSP0TAdRtC5oXcy-EMFuxbYWhEz0fW8z5QYlaaApQ9UMoG4KYToJ1ECZUAxoEKI_xPJGHs-V_T_Ms1Fhp0XrPM9oaGviW5gnWwqJiSQQue9EsPYueMaj01ucHcF52ltvhnkHd-LMuZVbCXrqZv0exVdI2boA";
        private readonly IUserStorage _userStorage;
        public static User auth_user = null;
        private static User pre_registration_user;
        private static int code_ver;
        public static string role;
        public static int logged = 0;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IAuthenticationService _authenticationService;

        public HomeController(IUserStorage userStorage, IPasswordHashService passwordHashService, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _passwordHashService = passwordHashService;
            _userStorage = userStorage;       
        }
    
        public IActionResult Index()
        {
            //if (hashPasswordmd5(pass).Equals(_userStorage.GetById(zero_patient).PasswordHash))
            //{
            //    hash_func = "md5";
            //    //Console.WriteLine(hash_func);
            //}

            //if (Encrypt(pass).Equals(_userStorage.GetById(zero_patient).PasswordHash))
            //{
            //    hash_func = "PBKDF2";
            //    //Console.WriteLine(hash_func);
            //}
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
      
        [HttpPost]
        public async Task<IActionResult> Index(string Email, string Password)
        {
            if(Email != null && Password != null)
            {
                User user = new User();
                user.Email = Email;
                User test_user = _userStorage.GetByEmail(user);
                if(test_user == null)
                {
                    ViewBag.Message = "User does not exist!";
                    return View();
                }
                if(!_passwordHashService.VerifyPassword(Password, _userStorage.GetByEmail(user).Password))
                {
                    ViewBag.Message = "Password is not correct!";
                    return View();
                    //ModelState.AddModelError("", "Имя и электронный адрес не должны совпадать.");
                    //throw new Exception("Такого пользователя не существует");

                }
                auth_user = test_user;
                role = auth_user.Role;
                logged = 1;
                //getToken();
                await _authenticationService.Authenticate(auth_user); // аутентификация    
                return RedirectToAction(nameof(Info));
            }
            else
            {
                ViewBag.Message = "Input password/login";
                return View();
                //throw new Exception("Введите логин/пароль");
            }
            
        }
        public IActionResult Register()
        {
            return View();
        }
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(string name,string email, string password, string pass_repeat)
        {
            if(name != null  && email != null && password != null)
            {
                Random rnd = new Random();
                User user = new User();
                user.Name = name;
                user.Password = _passwordHashService.HashPassword(password);
                user.Role = "user";
                if (IsValid(email))
                {
                    user.Email = email;
                }
                else
                {
                    ViewBag.Message = "Wrong email format";
                    return Register();
                    //throw new Exception("Неверный формат почты");
                }
                if (password != pass_repeat)
                {
                    ViewBag.Message = "Password doesn't match";
                    return Register();
                }
                User _user = new User();
                _user = _userStorage.GetByEmail(user);
                if (_user != null)
                {
                    ViewBag.Message = "User with such email is already exist";
                    return Register();
                    //throw new Exception("Пользователь с таким Email уже есть");

                }
                pre_registration_user = user;
                _authenticationService.Authenticate(pre_registration_user);
                code_ver = rnd.Next(100000);
                var mail = DbData.SendMessage.CreateMail(pre_registration_user.Email, code_ver);
                DbData.SendMessage.SendMail(mail);
                //_userStorage.InsertUser(user);
                return RedirectToAction(nameof(DoubleAuth));
            }
            else
            {
                ViewBag.Message = "Input all fields";
                return Register();
                //throw new Exception("Заполните все поля");
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            logged = 0;
            role = "";
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Registration()
        {
            return View();
        }
        [Authorize(Roles = "user, admin")]
        public IActionResult Info()
        {
      
            return View(auth_user);
        }
        [HttpPost]
        public IActionResult Info(User user)
        {
            return RedirectToAction(nameof(EditUser));
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult EditUser()
        {
            var new_user = auth_user;
            new_user.Password = "";
            return View(new_user);
        }
        [HttpPost]
        public IActionResult EditUser(string password, string pass_repeat)
        {
            User user = new User();
            if (password != pass_repeat)
            {
                ViewBag.Message = "Passwords does not match";
                return EditUser();
            }
            if (auth_user != null) 
            {
                user = _userStorage.GetById(auth_user.Id);
                user.Password = _passwordHashService.HashPassword(password);
                _userStorage.Update(user);
                auth_user = _userStorage.GetById(user.Id);
                return RedirectToAction(nameof(Info));
            }
            else
            {
                
                ViewBag.Message = "User not authenticated";
                return View(); 
            }
         
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult DoubleAuth()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DoubleAuth(int code)
        {
            if (code == code_ver)
            {
                return RedirectToAction(nameof(Success));
            }
            else
            {
                ViewBag.Message = "Wrong code";
                return DoubleAuth();
                //throw new Exception("Неверный код");
            }
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult Success()
        {
            logged = 1;
            pre_registration_user.EmailConfirmed = true;
            _userStorage.InsertUser(pre_registration_user);
            auth_user = pre_registration_user;
            return View();
        }
        public IActionResult ToCab()
        {
            return RedirectToAction(nameof(Info));
        }

    } 
}