using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using DB.Implements;
using DB.StorageInterfaces;
using DB.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
namespace WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserStorage _userStorage;
        public static User auth_user_admin;
        private static string hash = "complex_key_2212101321";
        private static string EncryptionKey = "MAKV2SPBNI99212";
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
        public AdminController(IUserStorage userStorage)
        {
            _userStorage = userStorage;
        }
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
            if (_userStorage.GetRole(auth_user_admin) == "user")
            {
                ViewBag.Message = "You aren't admin";
                return Index();
                //throw new Exception("Вы не админ!");
            }
            if (hashPasswordmd5(HomeController.pass).Equals(_userStorage.GetById(HomeController.zero_patient).PasswordHash))
            {
                HomeController.hash_func = "md5";
                //Console.WriteLine(hash_func);
            }

            if (Encrypt(HomeController.pass).Equals(_userStorage.GetById(HomeController.zero_patient).PasswordHash))
            {
                HomeController.hash_func = "PBKDF2";
                //Console.WriteLine(hash_func);
            }
            return View(_userStorage.GetFullList());
        }
        public IActionResult ChangeHash()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeHash(string hash)
        {
            if (hash.Equals(HomeController.hash_func))
            {
                ViewBag.Message = "Passwords already in this hash";
                return ChangeHash();
                //throw new Exception("Пароли уже имеют эту хеш-функцию");
            }
            List<User> users = new List<User>();
            users = _userStorage.GetFullList();
            if (hash.Equals("md5"))
            {
                foreach(var u in users)
                {
                    string str = "";
                    str = u.PasswordHash;
                    u.PasswordHash = hashPasswordmd5(Decrypt(str));
                    _userStorage.Update(u);
                }
                HomeController.hash_func = "PBKDF2";
            }
            if (hash.Equals("PBKDF2"))
            {
                foreach (var u in users)
                {
                    string str = "";
                    str = u.PasswordHash;
                    u.PasswordHash = Encrypt(dehashPasswordmd5(str));
                    _userStorage.Update(u);
                }
                HomeController.hash_func = "md5";
            }
            return RedirectToAction(nameof(Index));
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
        [HttpGet]
        public async Task<IActionResult> UserDelete(string Id)
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
        [HttpGet]
        public async Task<IActionResult> UserEdit(string Id)
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
