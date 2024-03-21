﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbData.Models;
namespace DbData.StorageInterfaces
{
    public interface IRoleStorage
    {
        void Insert(Role role);

        void Update(Role role);
        List<Role> GetFullList();
      
        Role GetById(int id);

        void Delete(int id);

        void Delete(Role role);
    }
}
