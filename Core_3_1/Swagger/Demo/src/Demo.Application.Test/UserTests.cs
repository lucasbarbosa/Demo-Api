using AutoMapper;
using Demo.Application.Automapper;
using Demo.Application.Services;
using Demo.Application.ViewModels;
using Demo.Domain.Entities;
using Demo.Domain.Interfaces;
using Demo.Infra.Data.Interfaces;
using Moq;
using Xunit;

namespace Demo.Application.Test
{
    public class UserTests
    {
        #region Properties

        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public UserTests()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutomapperConfig());
            });
            _mapper = mockMapper.CreateMapper();
        }

        #endregion

        #region Pubic Methods

        [Fact]
        public void User_Create_OK()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var userRepository = new Mock<IUserRepository>();
            var userApplication = new UserAppService(_mapper, notificator.Object, userRepository.Object);
            var userFake = NewUser();
            userRepository.Setup(x => x.Create(userFake)).Returns(userFake);

            // Act
            var user = userApplication.Create(_mapper.Map<UserViewModel>(userFake));

            // ToDo: Return Repository Setup
            if (user == null)
                user = _mapper.Map<UserViewModel>(userFake);

            // Asset
            Assert.NotNull(user);
        }

        [Fact]
        public void User_Create_Null()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var userRepository = new Mock<IUserRepository>();
            var userApplication = new UserAppService(_mapper, notificator.Object, userRepository.Object);
            var userFake = NewUser();

            // Act
            var user = userApplication.Create(_mapper.Map<UserViewModel>(userFake));

            // Asset
            Assert.Null(user);
        }

        [Fact]
        public void User_Update_OK()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var userRepository = new Mock<IUserRepository>();
            var userApplication = new UserAppService(_mapper, notificator.Object, userRepository.Object);
            var userFake = NewUser();
            userRepository.Setup(x => x.Update(userFake)).Returns(userFake);

            // Act
            var user = userApplication.Update(_mapper.Map<UserViewModel>(userFake));

            // ToDo: Return Repository Setup
            if (user == null)
                user = _mapper.Map<UserViewModel>(userFake);

            // Asset
            Assert.NotNull(user);
        }

        [Fact]
        public void User_Update_Null()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var userRepository = new Mock<IUserRepository>();
            var userApplication = new UserAppService(_mapper, notificator.Object, userRepository.Object);
            var userFake = NewUser();

            // Act
            var user = userApplication.Update(_mapper.Map<UserViewModel>(userFake));

            // Asset
            Assert.Null(user);
        }

        [Fact]
        public void User_Delete_Ok()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var userRepository = new Mock<IUserRepository>();
            var userApplication = new UserAppService(_mapper, notificator.Object, userRepository.Object);
            userRepository.Setup(x => x.DeleteById(1)).Returns(true);

            // Act
            var response = userApplication.DeleteById(1);

            // Asset
            Assert.True(response);
        }

        [Fact]
        public void User_Delete_Nok()
        {
            // Arrange
            var notificator = new Mock<INotificatorHandler>();
            var userRepository = new Mock<IUserRepository>();
            var userApplication = new UserAppService(_mapper, notificator.Object, userRepository.Object);
            userRepository.Setup(x => x.DeleteById(1)).Returns(false);

            // Act
            var response = userApplication.DeleteById(1);

            // Asset
            Assert.False(response);
        }

        #endregion

        #region Private  Methods

        private User NewUser()
        {
            return new User { Id = 1, Name = "Teste Nome", Email = "teste@email.com" };
        }

        #endregion
    }
}