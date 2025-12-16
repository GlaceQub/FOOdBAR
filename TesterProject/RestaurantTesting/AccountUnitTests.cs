using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Controllers;
using Restaurant.Models;
using Restaurant.ViewModels.Account;
using Restaurant.Data.UnitOfWork;
using Moq;

namespace RestaurantTesting
{
    public class AccountUnitTests
    {
        private readonly Mock<UserManager<CustomUser>> _userManagerMock;
        private readonly Mock<SignInManager<CustomUser>> _signInManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AccountUnitTests()
        {
            var userStoreMock = new Mock<IUserStore<CustomUser>>();
            _userManagerMock = new Mock<UserManager<CustomUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<CustomUser>>();
            _signInManagerMock = new Mock<SignInManager<CustomUser>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Login_Post_ReturnsRedirect_WhenLoginSuccessful()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { Email = "test@example.com", Password = "Password123!" };
            var user = new CustomUser { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginViewModel.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user.UserName, loginViewModel.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);

            // Act
            var result = await controller.Login(loginViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Login_Post_ReturnsView_WhenUserNotFound()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { Email = "notfound@example.com", Password = "Password123!" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(loginViewModel.Email)).ReturnsAsync((CustomUser)null);

            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);

            // Act
            var result = await controller.Login(loginViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginViewModel, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("Email"));
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Login_Post_ReturnsView_WhenPasswordIncorrect()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { Email = "test@example.com", Password = "WrongPassword!" };
            var user = new CustomUser { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginViewModel.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user.UserName, loginViewModel.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);

            // Act
            var result = await controller.Login(loginViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginViewModel, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("Password"));
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Login_Post_ReturnsView_WhenUserLockedOut()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { Email = "test@example.com", Password = "Password123!" };
            var user = new CustomUser { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginViewModel.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user.UserName, loginViewModel.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);

            // Act
            var result = await controller.Login(loginViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginViewModel, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(""));
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Registratie_Post_ReturnsView_WhenModelStateInvalid()
        {
            // Arrange
            var landenRepoMock = new Mock<ILandRepository>();
            landenRepoMock.Setup(x => x.GetActieveLandenAsync()).ReturnsAsync(new List<Land>());
            _unitOfWorkMock.SetupGet(x => x.Landen).Returns(landenRepoMock.Object);
            var model = new RegistratieViewModel { Email = "", Password = "" };
            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);
            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await controller.Registratie(model, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Registratie_Post_ReturnsView_WhenEmailAlreadyExists()
        {
            // Arrange
            var landenRepoMock = new Mock<ILandRepository>();
            landenRepoMock.Setup(x => x.GetActieveLandenAsync()).ReturnsAsync(new List<Land>());
            _unitOfWorkMock.SetupGet(x => x.Landen).Returns(landenRepoMock.Object);
            var model = new RegistratieViewModel { Email = "test@example.com", Password = "Password123!" };
            var existingUser = new CustomUser { Email = "test@example.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(existingUser);

            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);

            // ModelState is valid
            // Act
            var result = await controller.Registratie(model, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.True(controller.ModelState.ContainsKey("Email"));
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Registratie_Post_ReturnsConfirmationView_WhenSuccess()
        {
            // Arrange
            var model = new RegistratieViewModel { Email = "test@example.com", Password = "Password123!", Land = 1 };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((CustomUser)null);

            var landenRepoMock = new Mock<ILandRepository>();
            landenRepoMock.Setup(x => x.GetActieveLandenAsync()).ReturnsAsync(new[] { new Land { Id = 1, Naam = "België", Actief = true } });
            _unitOfWorkMock.SetupGet(x => x.Landen).Returns(landenRepoMock.Object);

            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);

            // Act
            var result = await controller.Registratie(model, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("RegistratieBevestigen", viewResult.ViewName);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Logout_Post_RedirectsToHome()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

            var controller = new AccountController(
                _signInManagerMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _unitOfWorkMock.Object);

            // Act
            var result = await controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}