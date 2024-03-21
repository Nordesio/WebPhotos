using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbData.Models;
namespace DbData.StorageInterfaces
{
    public interface IUserStorage
    {
        void InsertUser(User user);
      
        void Update(User user);
        List<User> GetFullList();
       
        User GetById(int id);
        User GetByEmail(User user);
        User GetByEmailAndPass(User user);


        void Delete(int id);

        void Delete(User user);
    }
}
