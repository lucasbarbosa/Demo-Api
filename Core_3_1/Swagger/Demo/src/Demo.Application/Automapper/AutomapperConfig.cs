using AutoMapper;
using Demo.Application.ViewModels;
using Demo.Domain.Entities;

namespace Demo.Application.Automapper
{
    public class AutomapperConfig : Profile
    {
        #region Constructors

        public AutomapperConfig()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
        }

        #endregion
    }
}