using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPO_Cursovaya.Models;

namespace KPO_Cursovaya.Implements
{
    public class VkuserStorage
    {
        public void Insert(Vkuser vkuser)
        {
            using var db = new DiplomContext();
            db.Vkusers.Add(vkuser);
            db.SaveChanges();
        }
        public void Update(Vkuser vkuser)
        {
            using var db = new DiplomContext();
            var element = db.Vkusers.FirstOrDefault(rec => rec.Id == vkuser.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(vkuser, element);
            db.SaveChanges();
        }
        private Vkuser CreateModel(Vkuser model, Vkuser vkuser)
        {
            vkuser.Url = model.Url;
            return vkuser;

        }
        public List<Vkuser> GetFullList()
        {
            using var db = new DiplomContext();
            return db.Vkusers.ToList();
        }
        public Vkuser GetById(string id)
        {
            using var db = new DiplomContext();
            return db.Vkusers.Find(id);
        }
        public void Delete(string id)
        {
            using var db = new DiplomContext();
            db.Vkusers.Remove(db.Vkusers.Find(id));
            db.SaveChanges();
        }
        public void Delete(Vkuser vkuser)
        {
            using var db = new DiplomContext();
            db.Vkusers.Remove(vkuser);
            db.SaveChanges();
        }
    }
}
