using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
namespace DB.Implements
{
    public class UserLoginStorage
    {
        public void Insert(UserLogin userLogin)
        {
            using var db = new CourseWorkContext();
            db.UserLogins.Add(userLogin);
            db.SaveChanges();
        }
        public void Update(UserLogin userLogin)
        {
            using var db = new CourseWorkContext();
            var element = db.UserLogins.FirstOrDefault(rec => rec.LoginProvider == userLogin.LoginProvider);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(userLogin, element);
            db.SaveChanges();
        }
        private UserLogin CreateModel(UserLogin model, UserLogin userLogin)
        {
            userLogin.ProviderKey = model.ProviderKey;
            userLogin.UserId = model.UserId;
            return userLogin;

        }
        public List<UserLogin> GetFullList()
        {
            using var db = new CourseWorkContext();
            return db.UserLogins.ToList();
        }
        public UserLogin GetById(string id)
        {
            using var db = new CourseWorkContext();
            return db.UserLogins.Find(id);
        }
        public void Delete(string id)
        {
            using var db = new CourseWorkContext();
            db.UserLogins.Remove(db.UserLogins.Find(id));
            db.SaveChanges();
        }
        public void Delete(UserLogin userLogin)
        {
            using var db = new CourseWorkContext();
            db.UserLogins.Remove(userLogin);
            db.SaveChanges();
        }
    }
}
