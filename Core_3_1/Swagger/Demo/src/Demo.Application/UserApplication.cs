using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using System.Collections.Generic;

namespace Demo.Application
{
    public class UserApplication : IUserApplication
    {
        #region Attributes

        private readonly IUserRepository _userRepository;

        #endregion

        #region Constructors

        public UserApplication(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #endregion

        #region Public Methods

        public IList<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public User GetById(uint id)
        {
            return _userRepository.GetById(id);
        }

        public User Create(User user)
        {
            return _userRepository.Create(user);
        }

        public User Update(User user)
        {
            return _userRepository.Update(user);
        }

        public bool DeleteById(uint id)
        {
            return _userRepository.DeleteById(id);
        }

        #endregion
    }
}