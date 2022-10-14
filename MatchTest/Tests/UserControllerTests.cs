using Match.Controllers;
using Match.Interfaces;
using Match.Models;
using Match.Models.DTOs;
using Match.Utilities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MatchTest.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _service;
        private static readonly User[] _users = GetSampleUsers();
        public UserControllerTests()
        {
            _service = new Mock<IUserService>();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(12)]
        [InlineData(15)]
        [InlineData(60)]
        [InlineData(200)]
        public async void DepositTest_InvalidAmount(int amount)
        {
            //Arrange
            _service
                .Setup(x => x.DepositAsync(amount))
                .ReturnsAsync(new Tuple<bool, User?, string?>(Constants.AcceptedCoins.Contains(amount), null, "Result"));
            var controller = new UserController(_service.Object);

            //Act
            var result = await controller.Deposit(amount) as BadRequestObjectResult;
            var actual = result?.Value as ResponseModel<string?>;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(Constants.AcceptedCoins.Contains(amount), actual?.Success);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(50)]
        [InlineData(100)]
        public async void DepositTest_ShouldBeSuccessful(int amount)
        {
            //Arrange
            _service
                .Setup(x => x.DepositAsync(amount))
                .ReturnsAsync(new Tuple<bool, User?, string?>(Constants.AcceptedCoins.Contains(amount), _users[0], "Result"));
            var controller = new UserController(_service.Object);

            //Act
            var result = await controller.Deposit(amount) as OkObjectResult;
            var actual = result?.Value as ResponseModel<LoginResponseDTO>;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(Constants.AcceptedCoins.Contains(amount), actual?.Success);
        }

        [Fact]
        public void UserCountTest()
        {
            var users = GetSampleUsers();

            Assert.Equal(3, users.Count());
        }
        private static User[] GetSampleUsers()
        {
            var products = new User[]
            {
                new User
                {
                    Id = "bc9042e7-8791-417d-80f4-1d93f879fec6",
                    FirstName = "John",
                    LastName = "Doe",
                    AccountBalance = 0.25m,
                    DateCreated = DateTime.Now.AddDays(-3),
                    Email = "jd@email.com"
                },
                new User
                {
                    Id = "2a63ced0-becb-4537-af42-7f70f5397f9f",
                    FirstName = "Jane",
                    LastName = "Doe",
                    AccountBalance = 0.25m,
                    DateCreated = DateTime.Now.AddDays(-1),
                    Email = "jd@email.com"
                },
                new User
                {
                    Id = "7aa0b9ec-0f96-4621-aa7f-fec120c30c1f",
                    FirstName = "John",
                    LastName = "Ogbu",
                    AccountBalance = 0.25m,
                    DateCreated = DateTime.Now.AddYears(-5),
                    Email = "oj@email.com"
                },
            };
            return products;
        }
    }
}
