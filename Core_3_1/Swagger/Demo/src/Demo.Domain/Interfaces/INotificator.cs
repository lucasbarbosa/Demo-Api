using Demo.Domain.Entities;
using System.Collections.Generic;

namespace Demo.Domain.Interfaces
{
    public interface INotificator
    {
        bool HasErrors();

        List<Notification> GetErrors();

        void AddError(string error);
    }
}