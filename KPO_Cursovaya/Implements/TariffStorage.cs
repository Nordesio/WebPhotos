using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
namespace DB.Implements
{
    public class TariffStorage
    {
        public void Insert(Tariff tariff)
        {
            using var db = new CourseWorkContext();
            db.Tariffs.Add(tariff);
            db.SaveChanges();
        }
        public void Update(Tariff tariff)
        {
            using var db = new CourseWorkContext();
            var element = db.Tariffs.FirstOrDefault(rec => rec.Id == tariff.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(tariff, element);
            db.SaveChanges();
        }
        private Tariff CreateModel(Tariff model, Tariff tariff)
        {
            tariff.Name = model.Name;
            tariff.QueryCount = model.QueryCount;
            return tariff;

        }
        public List<Tariff> GetFullList()
        {
            using var db = new CourseWorkContext();
            return db.Tariffs.ToList();
        }
        public Tariff GetById(string id)
        {
            using var db = new CourseWorkContext();
            return db.Tariffs.Find(id);
        }
        public void Delete(string id)
        {
            using var db = new CourseWorkContext();
            db.Tariffs.Remove(db.Tariffs.Find(id));
            db.SaveChanges();
        }
        public void Delete(Tariff tariff)
        {
            using var db = new CourseWorkContext();
            db.Tariffs.Remove(tariff);
            db.SaveChanges();
        }
    }
}
