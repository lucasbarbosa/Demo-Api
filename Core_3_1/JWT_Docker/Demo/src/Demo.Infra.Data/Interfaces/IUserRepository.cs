using Demo.Domain.Entities;

namespace Demo.Infra.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetById(uint id);

        bool DeleteById(uint id);
    }
}