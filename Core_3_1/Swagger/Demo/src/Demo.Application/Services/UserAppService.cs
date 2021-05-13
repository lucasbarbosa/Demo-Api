using AutoMapper;
using Demo.Application.Interfaces;
using Demo.Application.ViewModels;
using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using Demo.Infra.Data.Interfaces;
using System.Collections.Generic;

namespace Demo.Application.Services
{
    public class UserAppService : BaseServices, IUserAppService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly INotificatorHandler _notificator;
        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructors

        public UserAppService(IMapper mapper, INotificatorHandler notificator, IUserRepository userRepository)
        {
            _mapper = mapper;
            _notificator = notificator;
            _userRepository = userRepository;
        }

        #endregion

        #region Public Methods

        public IList<UserViewModel> GetAll()
        {
            var response = _mapper.Map<IList<UserViewModel>>(_userRepository.GetAll());

            return response;
        }

        public UserViewModel GetById(uint id)
        {
            var response = _mapper.Map<UserViewModel>(_userRepository.GetById(id));

            if (response == null)
                _notificator.AddError("Usuário não encontrado");

            return response;
        }

        public UserViewModel Create(UserViewModel user)
        {
            var response = _mapper.Map<UserViewModel>(_userRepository.GetById(user.Id));

            if (response != null)
            {
                _notificator.AddError($"Usuário (Id: {user.Id}) já cadastrado.");
            }
            else
            {
                response = _mapper.Map<UserViewModel>(_userRepository.Create(_mapper.Map<User>(user)));

                if (response == null)
                    _notificator.AddError("Usuário não cadastrado");
            }

            return response;
        }

        public UserViewModel Update(UserViewModel user)
        {
            var response = _mapper.Map<UserViewModel>(_userRepository.Update(_mapper.Map<User>(user)));

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