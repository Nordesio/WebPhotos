using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbData.Models;
namespace DbData.StorageInterfaces
{
    public interface IVkuserStorage
    {
        void Insert(Vkuser vkuser);

        void Update(Vkuser vkuser);
        List<Vkuser> GetFullList();
        List<Vkuser> GetListByUser(int id);
        Vkuser GetById(int id);
        void Delete(int id);

        void Delete(Vkuser vkuser);
    }
}
