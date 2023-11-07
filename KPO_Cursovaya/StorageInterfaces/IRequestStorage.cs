using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPO_Cursovaya.Models;
namespace KPO_Cursovaya.StorageInterfaces
{
    public interface IRequestStorage
    {
        void Insert(Request request);

        void Update(Request request);
        List<Request> GetFullList();

        List<Request> GetByVkId(int id);

        void Delete(int id);

        void Delete(Request request);

        void AddFullList(List<Request> req);
    }
}
