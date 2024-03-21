using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbData.Models;
using DbData.StorageInterfaces;
namespace DbData.Implements
{
    public class RoleStorage : IRoleStorage
    {
        public void Insert(Role role)
        {
            using var db = new DiplomContext();
            db.Roles.Add(role);
            db.SaveChanges();
        }
        public void Update(Role role)
        {
            using var db = new DiplomContext();
            var element = db.Roles.FirstOrDefault(rec => rec.Name == role.Name);
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
            using var db = new DiplomContext();
            return db.Roles.ToList();
        }
        public Role GetById(int id)
        {
            using var db = new DiplomContext();
            return db.Roles.Find(id);
        }
        public void Delete(int id)
        {
            using var db = new DiplomContext();
            db.Roles.Remove(db.Roles.Find(id));
            db.SaveChanges();
        }
        public void Delete(Role role)
        {
            using var db = new DiplomContext();
            db.Roles.Remove(role);
            db.SaveChanges();
        }
    }
}
