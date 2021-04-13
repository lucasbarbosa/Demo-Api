using Demo.Domain.Entities;
using System.Collections.Generic;

namespace Demo.Domain.Interfaces
{
    public interface IUserApplication
    {
        IList<User> GetAll();

        User GetById(uint id);

        User Create(User user);

        User Update(User user);

        bool DeleteById(uint id);
    }
}