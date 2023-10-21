using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
namespace DB.Implements
{
    public class RoleStorage
    {
        public void Insert(Role role)
        {
            using var db = new CourseWorkContext();
            db.Roles.Add(role);
            db.SaveChanges();
        }
        public void Update(Role role)
        {
            using var db = new CourseWorkContext();
            var element = db.Roles.FirstOrDefault(rec => rec.Id == role.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(role, element);
            db.SaveChanges();
        }
        private Role CreateModel(Role model, Role role)
        {
            role.Name = model.Name;
            return role;

        }
        public List<Role> GetFullList()
        {
            using var db = new CourseWorkContext();
            return db.Roles.ToList();
        }
        public Role GetById(string id)
        {
            using var db = new CourseWorkContext();
            return db.Roles.Find(id);
        }
        public void Delete(string id)
        {
            using var db = new CourseWorkContext();
            db.Roles.Remove(db.Roles.Find(id));
            db.SaveChanges();
        }
        public void Delete(Role role)
        {
            using var db = new CourseWorkContext();
            db.Roles.Remove(role);
            db.SaveChanges();
        }

    }
}
