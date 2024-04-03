using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbData.Models;
namespace DbData.StorageInterfaces
{
    public interface IRequestStorage
    {
        void Insert(Request request);

        void Update(Request request);
        List<Request> GetFullList();

        List<Request> GetByVkId(int id);
        Request GetById(int id);
        void Delete(int id);
        
        void Delete(Request request);
        void DeleteByVkUser(Vkuser vkuser);

        void AddFullList(List<Request> req);
    }
}
