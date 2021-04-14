using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region Attributes

        private static List<User> _memoryUsers = new List<User>();

        #endregion

        #region Pubic Methods

        public IList<User> GetAll()
        {
            return _memoryUsers;
        }

        public User GetById(uint id)
        {
            if (_memoryUsers.Any(u => u.Id == id))
            {
                var user = _memoryUsers.FirstOrDefault(u => u.Id == id);

                return user;
            }
            else
            {
                return null;
            }
        }

        public User Create(User user)
        {
            user.Id = NewId();
            
            _memoryUsers.Add(user);
            user = _memoryUsers.FirstOrDefault(u => u.Id == user.Id);

            return user;
        }

        public User Update(User user)
        {
            if (_memoryUsers.Any(u => u.Id == user.Id))
            {
                var index = _memoryUsers.FindIndex(u => u.Id == user.Id);
                _memoryUsers[index] = user;
                user = _memoryUsers.FirstOrDefault(u => u.Id == user.Id);

                return user;
            }
            else
            {
                return null;
            }
        }

        public bool DeleteById(uint id)
        {
            if(_memoryUsers.Any(x => x.Id == id))
            {
                _memoryUsers.RemoveAll(x => x.Id == id);
                var delete = !_memoryUsers.Any(x => x.Id == id);

                return delete;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Private Methods

        private uint NewId()
        {
            var id = _memoryUsers.Max(u => u?.Id) + 1;

            if (id == null)
                id = 1;

            return (uint)id;
        }

        #endregion
    }
}