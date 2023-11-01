using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPO_Cursovaya.Models;
using KPO_Cursovaya.StorageInterfaces;
namespace KPO_Cursovaya.Implements
{
    public class VkuserStorage : IVkuserStorage
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
        public List<Vkuser> GetListByUser(int id)
        {
            using var db = new DiplomContext();
            return db.Vkusers.Where(c => c.UserId == id).ToList();
        }
        public void Delete(int id)
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
