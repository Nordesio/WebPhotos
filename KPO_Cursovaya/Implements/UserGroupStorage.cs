using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
namespace DB.Implements
{
    public class UserGroupStorage
    {
        public void Insert(UserGroup userGroup)
        {
            using var db = new CourseWorkContext();
            db.UserGroups.Add(userGroup);
            db.SaveChanges();
        }
        public void Update(UserGroup userGroup)
        {
            using var db = new CourseWorkContext();
            var element = db.UserGroups.FirstOrDefault(rec => rec.Id == userGroup.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(userGroup, element);
            db.SaveChanges();
        }
        private UserGroup CreateModel(UserGroup model, UserGroup userGroup)
        {
            userGroup.Name = model.Name;
            userGroup.Description = model.Description;
            userGroup.StartDate = model.StartDate;
            userGroup.FinishDate = model.FinishDate;
            userGroup.Subscription = model.Subscription;
            userGroup.TariffId = model.TariffId;
            userGroup.TelegramAccessKey = model.TelegramAccessKey;
            return userGroup;

        }
        public List<UserGroup> GetFullList()
        {
            using var db = new CourseWorkContext();
            return db.UserGroups.ToList();
        }
        public UserGroup GetById(string id)
        {
            using var db = new CourseWorkContext();
            return db.UserGroups.Find(id);
        }
        public void Delete(string id)
        {
            using var db = new CourseWorkContext();
            db.UserGroups.Remove(db.UserGroups.Find(id));
            db.SaveChanges();
        }
        public void Delete(UserGroup userGroup)
        {
            using var db = new CourseWorkContext();
            db.UserGroups.Remove(userGroup);
            db.SaveChanges();
        }
    }
}
