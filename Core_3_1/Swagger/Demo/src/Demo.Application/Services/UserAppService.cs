using Demo.Application.Interfaces;
using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using Demo.Infra.Data.Interfaces;
using System.Collections.Generic;

namespace Demo.Application.Services
{
    public class UserAppService : IUserAppService
    {
        #region Properties

        private readonly INotificator _notificator;
        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructors

        public UserAppService(INotificator notificator, IUserRepository userRepository)
        {
            _notificator = notificator;
            _userRepository = userRepository;
        }

        #endregion

        #region Public Methods

        public IList<User> GetAll()
        {
            var response = _userRepository.GetAll();

            return response;
        }

        public User GetById(uint id)
        {
            var response = _userRepository.GetById(id);

            if (response == null)
                _notificator.AddError("Usuário não encontrado");

            return response;
        }

        public User Create(User user)
        {
            var response = _userRepository.GetById(user.Id);

            if (response != null)
            {
                _notificator.AddError($"Usuário (Id: {user.Id}) já cadastrado.");
            }
            else
            {
                response = _userRepository.Create(user);

                if (response == null)
                    _notificator.AddError("Usuário não cadastrado");
            }

            return response;
        }

        public User Update(User user)
        {
            var response = _userRepository.Update(user);

            if (response == null)
                _notificator.AddError("Usuário não encontrado");

            return response;
        }

        public bool DeleteById(uint id)
        {
            var response = _userRepository.DeleteById(id);

            if (!response)
                _notificator.AddError("Usuário não encontrado");

            return response;
        }

        #endregion
    }
}