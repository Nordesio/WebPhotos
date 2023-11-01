using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using KPO_Cursovaya.StorageInterfaces;
using KPO_Cursovaya.Models;
using KPO_Cursovaya.Implements;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net;
using EO.WebBrowser.DOM;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserStorage _userStorage;
        private readonly IRoleStorage _roleStorage;
        public static User auth_user = null;
        private static User pre_registration_user;
        private static int code_ver;
        public static string role;
        public static int logged = 0;

        public HomeController(IUserStorage userStorage, IRoleStorage roleStorage)
        {
            _userStorage = userStorage;
            _roleStorage = roleStorage;
         
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
                user.Password = Password;
                auth_user = _userStorage.GetByEmailAndPass(user);
                if(auth_user == null)
                {
                    ViewBag.Message = "User doesn't exist!";
                    return Index();
                    //ModelState.AddModelError("", "Имя и электронный адрес не должны совпадать.");
                    //throw new Exception("Такого пользователя не существует");

                }
                role = auth_user.Role;
                logged = 1;
                await Authenticate(auth_user); // аутентификация
                return RedirectToAction(nameof(Info));
            }
            else
            {
                ViewBag.Message = "Input password/login";
                return Index();
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
                user.Password = password;
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
                Authenticate(pre_registration_user);
                code_ver = rnd.Next(100000);
                var mail = DB.SendMessage.CreateMail(pre_registration_user.Email, code_ver);
                DB.SendMessage.SendMail(mail);
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
            return View(auth_user);
        }
        [HttpPost]
        public IActionResult EditUser(string password, string pass_repeat)
        {
            User user = new User();
            if (password != pass_repeat)
            {
                ViewBag.Message = "Password doesn't match";
                return EditUser();
            }
            user.Id = auth_user.Id;
            user.Password = password;
            _userStorage.Update(user);
            auth_user = _userStorage.GetById(user.Id);
            return RedirectToAction(nameof(Info));
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
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
         
        }
    } 
}