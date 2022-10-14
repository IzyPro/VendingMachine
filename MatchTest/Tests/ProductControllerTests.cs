using Match.Controllers;
using Match.Interfaces;
using Match.Models;
using Match.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MatchTest.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _service;
        private static readonly Product[] _products = GetSampleProduct();
        public ProductControllerTests()
        {
            _service = new Mock<IProductService>();
        }

        [Theory]
        [InlineData(100, "bc9042e7-8791-417d-80f4-1d93f879fec6", 2, 51)]
        [InlineData(2.5, "bc9042e7-8791-417d-80f4-1d93f879fec6", 1, 2)]
        [InlineData(0, "bc9042e7-8791-417d-80f4-1d93f879fec6", 1, 0.5)]
        [InlineData(50, "bc9042e7-8791-417d-80f4-1d93f879fec6", 3, 15)]
        public void BuyProductTest_SufficientBalance(decimal userBalance, Guid productId, int productCount, decimal productAmount)
        {
            // Arrange
            _service
                .Setup(x => x.BuyProductAsync(productId, productCount))
                .ReturnsAsync(new Tuple<bool, List<int>, string>(userBalance >= productAmount * productCount, new List<int>(), "Purchase Successful"));
            var controller = new ProductController(_service.Object);

            // Act
            var actionResult = controller.Buy(productId, productCount);
            var result = actionResult.Result as OkObjectResult;
            var badResult = actionResult.Result as BadRequestObjectResult;
            var actualOnSuccess = result?.Value as ResponseModel<List<int>>;
            var actualOnFailure = badResult?.Value as ResponseModel<List<int>>;
            var expected = userBalance >= productAmount * productCount;

            // Assert
            if(expected)
                Assert.IsType<OkObjectResult>(result);
            else
                Assert.IsType<BadRequestObjectResult>(badResult);

            Assert.Equal(expected, userBalance >= productAmount * productCount ? actualOnSuccess?.Success : actualOnFailure?.Success);
        }


        [Theory]
        [MemberData(nameof(ChangeTestData))]
        public async void BuyProductTest_GetsCorrectChange(decimal userBalance, List<int> expected)
        {
            // Arrange
            var userService = new Mock<IUserService>();
            var _productService = new ProductService(null, userService.Object);

            // Act
            var actual = await _productService.CalculateChangeAsync(userBalance);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetProductById_ShouldReturnBadRequest()
        {
            _service
                .Setup(x => x.GetProductAsync(_products[0].Id))
                .ReturnsAsync(new Tuple<bool, Product?, string>(false, _products[0], "Failure"));
            var controller = new ProductController(_service.Object);

            var actionResult = controller.Get(Guid.Parse("bc9042e7-8791-417d-80f4-1d93f879fec6"));
            var result = actionResult?.Result as BadRequestObjectResult;
            var actual = result?.Value as ResponseModel<Product>;

            Assert.IsNotType<OkObjectResult>(result);
        }

        [Fact]
        public void GetProductById_ShouldBeSuccessful()
        {
            // Arrange
            _service
                .Setup(x => x.GetProductAsync(_products[0].Id))
                .ReturnsAsync(new Tuple<bool, Product?, string>(true, _products[0], "success"));
            var controller = new ProductController(_service.Object);

            var actionResult = controller.Get(_products[0].Id);
            var result = actionResult?.Result as OkObjectResult;
            var actual = result?.Value as ResponseModel<Product>;

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(_products[0], actual?.Data);
        }

        [Fact]
        public void ProductCountTest()
        {
            var products = GetSampleProduct();

            Assert.Equal(3, products.Count());
        }

        private static Product[] GetSampleProduct()
        {
            var products = new Product[]
            {
                new Product
                {
                    Id = Guid.Parse("bc9042e7-8791-417d-80f4-1d93f879fec6"),
                    Name = "Noreous",
                    Description = "Noreous Biscuit",
                    Price = 0.25m
                },
                new Product
                {
                    Id = Guid.Parse("2a63ced0-becb-4537-af42-7f70f5397f9f"),
                    Name = "Snicker",
                    Description = "SNicker Bar",
                    Price = 0.75m
                },
                new Product
                {
                    Id = Guid.Parse("7aa0b9ec-0f96-4621-aa7f-fec120c30c1f"),
                    Name = "Coke Soda",
                    Description = "1L Coke Soda",
                    Price = 0.5m
                },
            };
            return products;
        }

        public static IEnumerable<object[]> ChangeTestData()
        {
            yield return new object[] { 1.5, new List<int> { 100, 50 } };
            yield return new object[] { 1, new List<int> { 100 } };
            yield return new object[] { 0.3, new List<int> { 20, 10 } };
            yield return new object[] { 0.03, new List<int>() };
        }
    }
}
