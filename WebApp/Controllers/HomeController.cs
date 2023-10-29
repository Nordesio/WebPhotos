using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using DB.Implements;
using DB.StorageInterfaces;
using DB.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net;
using EO.WebBrowser.DOM;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserStorage _userStorage;
        public static User auth_user = null;
        private static User pre_registration_user;
        private static int code_ver;
        public static string pass = "12345";
        public static string zero_patient = "test411";
        public static string hash_func = "90";
        private static string hash = "complex_key_2212101321";
        private static string EncryptionKey = "MAKV2SPBNI99212";
        public static string role;
        public static string Encrypt(string clearText)
        {
            
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        private static string dehashPasswordmd5(string pass)
        {
            byte[] data = Convert.FromBase64String(pass); // decrypt the incrypted text
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(results);
                }
            }
        }
        private static string hashPasswordmd5(string pass)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(pass);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }
        public HomeController(IUserStorage userStorage)
        {
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
        public async Task<IActionResult> Index(string Email, string PasswordHash)
        {
            if(Email != null && PasswordHash != null)
            {
                User user = new User();
                if (hash_func.Equals("md5"))
                {
                    user.PasswordHash = hashPasswordmd5(PasswordHash);
                }
                if (hash_func.Equals("PBKDF2"))
                {
                    user.PasswordHash = Encrypt(PasswordHash);
                }
                user.Email = Email;
               
                auth_user = _userStorage.GetByEmailAndPass(user);
                if(auth_user == null)
                {
                    ViewBag.Message = "User doesn't exist!";
                    return Index();
                    //ModelState.AddModelError("", "Имя и электронный адрес не должны совпадать.");
                    //throw new Exception("Такого пользователя не существует");

                }
                role = _userStorage.GetRole(auth_user);
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
        public async Task<IActionResult> Register(string id, string surname, string name, string patronymic, string birthday, string email,
            string passwordHash, string phoneNumber, string userName)
        {
            if(surname != null && name != null && patronymic != null && birthday != null && email != null && passwordHash != null && phoneNumber != null && userName != null)
            {
                Random rnd = new Random();
                User user = new User();
                user.Id = id;
                user.Surname = surname;
                user.Name = name;
                user.Patronymic = patronymic;
                user.Birthday = DateOnly.ParseExact(birthday, "dd/MM/yyyy", null);
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
                if (hash_func.Equals("md5"))
                {
                    user.PasswordHash = hashPasswordmd5(passwordHash);
                }
                if (hash_func.Equals("PBKDF2"))
                {
                    user.PasswordHash = Encrypt(passwordHash);
                }
                Regex validatePhoneNumberRegex = new Regex("^\\+?[1-9][0-9]{7,14}$");
                if (validatePhoneNumberRegex.IsMatch(phoneNumber))
                {
                    user.PhoneNumber = phoneNumber;

                }
                else
                {
                    ViewBag.Message = "Wrong phone number format";
                    return Register();
                    //throw new Exception("Неверный формат номера телефона");
                }
                user.UserName = userName;
                User _user = new User();
                _user = _userStorage.GetByEmail(user);
                if (_user != null)
                {
                    ViewBag.Message = "User with such email is already exist";
                    return Register();
                    //throw new Exception("Пользователь с таким Email уже есть");

                }
                pre_registration_user = user;
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
 
        public IActionResult Registration()
        {
            return View();
        }
        public IActionResult Info()
        {
            if(auth_user == null)
            {
                ViewBag.Message = "You are not authorised";
                return Info();
               // throw new Exception("Вы не авторизованы!");
            }
            return View(auth_user);
        }
        [HttpPost]
        public IActionResult Info(User user)
        {
            return RedirectToAction(nameof(EditUser));
        }
        public IActionResult EditUser()
        {
            return View(auth_user);
        }
        [HttpPost]
        public IActionResult EditUser(User user)
        {
            user.Id = auth_user.Id;
            _userStorage.Update(user);
            auth_user = user;
            return RedirectToAction(nameof(Info));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
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
        public IActionResult Success()
        {
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