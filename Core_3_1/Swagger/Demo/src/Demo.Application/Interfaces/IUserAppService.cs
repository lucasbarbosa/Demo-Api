using Demo.Application.ViewModels;
using System.Collections.Generic;

namespace Demo.Application.Interfaces
{
    public interface IUserAppService
    {
        IList<UserViewModel> GetAll();

        UserViewModel GetById(uint id);

        UserViewModel Create(UserViewModel user);

        UserViewModel Update(UserViewModel user);

        bool DeleteById(uint id);
    }
}