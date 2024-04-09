using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbData.Models;
using DbData.StorageInterfaces;
namespace DbData.Implements
{
    public class EntityStorage : IEntityStorage
    {
        public void Insert(Entity entity)
        {
            using var db = new DiplomContext();
            db.Entities.Add(entity);
            db.SaveChanges();
        }
        public void Update(Entity entity)
        {
            using var db = new DiplomContext();
            var element = db.Entities.FirstOrDefault(rec => rec.Id == entity.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(entity, element);
            db.SaveChanges();
        }
        public void Update(IEnumerable<Entity> entitys)
        {
            using var db = new DiplomContext();

            foreach (var entity in entitys)
            {
                var element = db.Entities.FirstOrDefault(rec => rec.Id == entity.Id);
                if (element == null)
                {
                    throw new Exception($"Элемент с Id {entity.Id} не найден");
                }
                CreateModel(entity, element);
            }

            db.SaveChanges();
        }
        private Entity CreateModel(Entity model, Entity entity)
        {
            entity.Geo = model.Geo;
            entity.Date = model.Date;
            entity.Organization = model.Organization;
            entity.Namedentity = model.Namedentity;
            entity.Person = model.Person;
            entity.Street = model.Street;
            entity.Money = model.Money;
            entity.Address = model.Address;
            return entity;

        }
        public List<Entity> GetFullList()
        {
            using var db = new DiplomContext();
            return db.Entities.ToList();
        }
        public Entity GetByRequestId(int id)
        {
            using var db = new DiplomContext();
            return db.Entities.FirstOrDefault(c => c.RequestId == id);
        }
        public Entity GetById(int id)
        {
            using var db = new DiplomContext();
            return db.Entities.FirstOrDefault(c => c.Id == id);
        }
        public void AddFullList(List<Entity> req)
        {
            using var db = new DiplomContext();
            db.Entities.AddRange(req);
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            using var db = new DiplomContext();
            db.Entities.Remove(db.Entities.Find(id));
            db.SaveChanges();
        }
        public void Delete(Entity entity)
        {
            using var db = new DiplomContext();
            db.Entities.Remove(entity);
            db.SaveChanges();
        }

    }
}
