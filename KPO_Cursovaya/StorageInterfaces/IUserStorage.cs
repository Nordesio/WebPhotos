using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;
namespace DB.StorageInterfaces
{
    public interface IUserStorage
    {
        void InsertUser(User user);

        void Update(User user);
        List<User> GetFullList();
        string GetRole(User user);
        User GetById(string id);
        User GetByEmail(User user);
        User GetByEmailAndPass(User user);
        

        void Delete(string id);

        void Delete(User user);
    }
}
