using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
using DB.StorageInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DB.Implements
{
    public class UserStorage : IUserStorage
    {
        public void InsertUser(User user)
        {
            using var db = new CourseWorkContext();
            user.Roles.Add(db.Roles.Find("user"));
            db.Users.Add(user);
            db.SaveChanges();
        }
        public string GetRole(User user)
        {
            using var db = new CourseWorkContext();
            var users = db.Users.Include(c => c.Roles).Where(rec => rec.Id == user.Id).ToList();
            string role = "";
            foreach(var u in users)
            {
                foreach(Role r in u.Roles)
                {
                    role = r.Name;
                }
            }
            return role;

        }
        public void Update(User user)
        {
            using var db = new CourseWorkContext();
            var element = db.Users.FirstOrDefault(rec => rec.Id == user.Id);
            if(element == null)
                throw new Exception("Не найдено");
            CreateModel(user, element);
            db.SaveChanges();
        }
        private User CreateModel(User model, User user)
        {
            user.Surname = model.Surname;
            user.Name = model.Name;
            user.Patronymic = model.Patronymic;
            user.Birthday = model.Birthday;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PasswordHash = model.PasswordHash;
            user.SecurityStamp = model.SecurityStamp;
            user.PhoneNumber = model.PhoneNumber;
            user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
            user.TwoFactorEnabled = model.TwoFactorEnabled;
            user.LockoutEndDateUtc = model.LockoutEndDateUtc;
            user.LockoutEnabled = model.LockoutEnabled;
            user.AccessFailedCount = model.AccessFailedCount;
            user.UserName = model.UserName;
            user.UserGroupId = model.UserGroupId;
            return user;

        }
        public void InsertAdmin(User user)
        {
            using var db = new CourseWorkContext();
            user.Roles.Add(db.Roles.Find("admin"));
            db.Users.Add(user);
            db.SaveChanges();
        }

        public List<User> GetFullList()
        {
            using var db = new CourseWorkContext();
            return db.Users.ToList();
        }
        public User GetById(string id)
        {
            using var db = new CourseWorkContext();
            return db.Users.Find(id);
        }
        public User GetByEmail(User model)
        {
            using var db = new CourseWorkContext();
            return db.Users.FirstOrDefault(rec => rec.Email == model.Email);
        }
        public User GetByEmailAndPass(User model)
        {
            using var db = new CourseWorkContext();
            return db.Users.FirstOrDefault(rec => rec.Email == model.Email && rec.PasswordHash == model.PasswordHash);
        }
        public void Delete(string id)
        {
            using var db = new CourseWorkContext();
            db.Users.Remove(db.Users.Find(id));
            db.SaveChanges();
        }
        public void Delete(User user)
        {
            using var db = new CourseWorkContext();
            db.Users.Remove(user);
            db.SaveChanges();
        }


    }
}
