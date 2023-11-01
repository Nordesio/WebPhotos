using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPO_Cursovaya.Models;
using Microsoft.EntityFrameworkCore;
using KPO_Cursovaya.StorageInterfaces;
namespace KPO_Cursovaya.Implements
{
    public class UserStorage : IUserStorage
    {
        public void InsertUser(User user)
        {
            using var db = new DiplomContext();
            user.Role = "user";
            db.Users.Add(user);
            db.SaveChanges();
        }
        public void Update(User user)
        {
            using var db = new DiplomContext();
            var element = db.Users.FirstOrDefault(rec => rec.Id == user.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(user, element);
            db.SaveChanges();
        }
        private User CreateModel(User model, User user)
        {
            user.Name = model.Name;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.Password = model.Password;
            return user;

        }
        public void InsertAdmin(User user)
        {
            using var db = new DiplomContext();
            user.Role = "admin";
            db.Users.Add(user);
            db.SaveChanges();
        }

        public List<User> GetFullList()
        {
            using var db = new DiplomContext();
            return db.Users.ToList();
        }
        public User GetById(int id)
        {
            using var db = new DiplomContext();
            return db.Users.Find(id);
        }
        public User GetByEmail(User model)
        {
            using var db = new DiplomContext();
            return db.Users.FirstOrDefault(rec => rec.Email == model.Email);
        }
        public User GetByEmailAndPass(User model)
        {
            using var db = new DiplomContext();
            return db.Users.FirstOrDefault(rec => rec.Email == model.Email && rec.Password == model.Password);
        }
        public void Delete(int id)
        {
            using var db = new DiplomContext();
            db.Users.Remove(db.Users.Find(id));
            db.SaveChanges();
        }
        public void Delete(User user)
        {
            using var db = new DiplomContext();
            db.Users.Remove(user);
            db.SaveChanges();
        }
    }
}
