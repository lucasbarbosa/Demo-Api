using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using System.Collections.Generic;

namespace Demo.Domain.Handlers
{
    public class NotificatorHandler : INotificator
    {
        #region Attributes

        private readonly List<Notification> _errors;

        #endregion

        public bool HasErrors()
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.List<Notification> GetErrors()
        {
            throw new System.NotImplementedException();
        }

        public void AddError(string error)
        {
            throw new System.NotImplementedException();
        }
    }
}