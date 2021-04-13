using AutoMapper;
using Demo.Api.ViewModels;
using Demo.Domain.Entities;

namespace Demo.Api.Configuration
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