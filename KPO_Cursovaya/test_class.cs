using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using DB.Models;
using DB.Implements;
using System.Collections;
using System.Security.Cryptography;
namespace DB
{
    public class test_class
    {
        private static string hash = "complex_key_2212101321";
        private static string hashPassword(string pass)
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

        public void Add_user(string str)
        {
            if (str.Equals("delete"))
            {
                Console.WriteLine("Удаляем ^^");
                string id = "test5";
                UserStorage us = new UserStorage();
                us.Delete(id);
            }
            if (str.Equals("insert"))
            {
                for (int i = 5; i < 20; i++)
                {
                    Console.WriteLine("Записываем ^^");
                    User user = new User();
                    user.Id = "test4" + i;
                    user.Name = "test";
                    user.Surname = "test";
                    user.Patronymic = "test";
                    user.Birthday = DateOnly.Parse("1 Декабря 1990");
                    user.Email = "test@bk.ru";
                    user.EmailConfirmed = true;
                    user.PasswordHash = hashPassword("12345");
                    user.SecurityStamp = "12345";
                    user.PhoneNumber = "+7 (123) 456 78 90";
                    user.PhoneNumberConfirmed = false;
                    user.TwoFactorEnabled = true;
                    user.LockoutEndDateUtc = DateOnly.FromDateTime(DateTime.Now);
                    user.LockoutEnabled = false;
                    user.AccessFailedCount = 0;
                    user.UserName = "test";
                    UserStorage us = new UserStorage();
                    us.InsertUser(user);
                }
                /*
                var roles = us.GetFullList();
                foreach (var r in roles)
                {
                    Console.WriteLine(r.Id);
                    Console.WriteLine(r.Email);
                    Console.WriteLine(r.EmailConfirmed);
                    Console.WriteLine(r.AccessFailedCount);
                    Console.WriteLine(r.LockoutEndDateUtc);
                    Console.WriteLine(r.UserName);
                }
                */
            }
            if (str.Equals("update"))
            {
                User user = new User();
                user.Id = "test";
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = false;
                user.TwoFactorEnabled = true;
                user.LockoutEnabled = false;
                user.AccessFailedCount = 0;
                user.UserName = "test_updated";
                UserStorage us = new UserStorage();
                us.Update(user);
                Console.WriteLine("Updated! {0}",user.UserName);
            }
        }
        
        public async void Add_Tarrif(string str)
        {
            if (str.Equals("find"))
            {
                Console.WriteLine("Читаем ^^");
                string id = "1";
                using (var db = new CourseWorkContext())
                {
                    Role r = db.Roles.Find(id);
                    Console.WriteLine(r.Name);
                }
            }
            if (str.Equals("insert"))
            {
                Console.WriteLine("Записываем ^^");
                Role rol = new Role();
                rol.Id = "admin";
                rol.Name = "admin";
                Console.WriteLine("Имя = {0}, Id = {1}", rol.Name, rol.Id);
                RoleStorage rs = new RoleStorage();
                rs.Insert(rol);
                var roles = rs.GetFullList();
                foreach(var r in roles)
                {
                    Console.WriteLine(r.Name +"\n");
                }
                

            }
        }
        
    }
}
