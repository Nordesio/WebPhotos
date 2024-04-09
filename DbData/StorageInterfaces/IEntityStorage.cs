using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbData.Models;

namespace DbData.StorageInterfaces
{
    public interface IEntityStorage
    {
        void Insert(Entity entity);

        void Update(Entity entity);
        void Update(IEnumerable<Entity> entities);
        List<Entity> GetFullList();

        Entity GetByRequestId(int id);
        Entity GetById(int id);
        void Delete(int id);

        void Delete(Entity entity);

        void AddFullList(List<Entity> req);
    }
}
