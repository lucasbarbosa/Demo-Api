using Demo.Domain.Handlers;
using System.Collections.Generic;

namespace Demo.Domain.Interfaces
{
    public interface INotificatorHandler
    {
        bool HasErrors();

        List<Notification> GetErrors();

        void AddError(string error);
    }
}