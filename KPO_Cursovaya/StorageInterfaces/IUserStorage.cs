using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPO_Cursovaya.Models;
namespace KPO_Cursovaya.StorageInterfaces
{
    public interface IUserStorage
    {
        void InsertUser(User user);
        string GetRoleByEmail(User user);
        void Update(User user);
        List<User> GetFullList();
        string GetRole(User user);
        User GetById(int id);
        User GetByEmail(User user);
        User GetByEmailAndPass(User user);


        void Delete(int id);

        void Delete(User user);
    }
}
