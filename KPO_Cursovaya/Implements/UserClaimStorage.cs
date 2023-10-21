using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
namespace DB.Implements
{
    public class UserClaimStorage
    {
        public void Insert(UserClaim userClaim)
        {
            using var db = new CourseWorkContext();
            db.UserClaims.Add(userClaim);
            db.SaveChanges();
        }
        public void Update(UserClaim userClaim)
        {
            using var db = new CourseWorkContext();
            var element = db.UserClaims.FirstOrDefault(rec => rec.Id == userClaim.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(userClaim, element);
            db.SaveChanges();
        }
        private UserClaim CreateModel(UserClaim model, UserClaim userClaim)
        {
            userClaim.UserId = model.UserId;
            userClaim.ClaimValue = model.ClaimValue;
            userClaim.ClaimType = model.ClaimType;
            return userClaim;

        }
        public List<UserClaim> GetFullList()
        {
            using var db = new CourseWorkContext();
            return db.UserClaims.ToList();
        }
        public UserClaim GetById(string id)
        {
            using var db = new CourseWorkContext();
            return db.UserClaims.Find(id);
        }
        public void Delete(string id)
        {
            using var db = new CourseWorkContext();
            db.UserClaims.Remove(db.UserClaims.Find(id));
            db.SaveChanges();
        }
        public void Delete(UserClaim userClaim)
        {
            using var db = new CourseWorkContext();
            db.UserClaims.Remove(userClaim);
            db.SaveChanges();
        }
    }
}
