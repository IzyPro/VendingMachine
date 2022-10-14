using Match.Data;
using Match.Interfaces;
using Match.Models;
using Match.Models.RequestModels;
using Match.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Match.Services
{
    public class ProductService : IProductService
    {
        private readonly MatchDbContext _dbContext;
        private readonly IUserService _userService;
        public ProductService(MatchDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Tuple<bool, List<int>, string>> BuyProductAsync(Guid productId, int productCount)
        {
            var (exists, product, message) = await GetProductAsync(productId);
            if (product == null || !exists)
                return new Tuple<bool, List<int>, string>(false, new List<int>(), "Invalid product id");

            var (success, user) = await _userService.GetUserAsync();
            if (user == null || !success)
                return new Tuple<bool, List<int>, string>(false, new List<int>(), "Unable to fetch user");

            var totalExpense = productCount * product.Price;

            if (totalExpense > user.AccountBalance)
                return new Tuple<bool, List<int>, string>(false, new List<int>(), "Insuffficient funds");

            user.AccountBalance -= totalExpense;
            var change = await CalculateChangeAsync(user.AccountBalance);
            
            var (updated, UpdatedUser) = await _userService.UpdateUserBalanceAsync(user.AccountBalance);
            if (UpdatedUser == null || !updated)
                return new Tuple<bool, List<int>, string>(false, new List<int>(), "Unable to update user data");

            return new Tuple<bool, List<int>, string>(true, change, "Purchase successful");
        }

        public async Task<List<int>> CalculateChangeAsync(decimal accountBalance)
        {
            var change = new List<int>();

            for (int i = 0; i < Constants.AcceptedCoins.Length; i++)
            {
                if (accountBalance < 0.05m)
                    break;
                while (accountBalance * 100 >= Constants.AcceptedCoins[i])
                {
                    var coin = Math.Round((decimal)Constants.AcceptedCoins[i] / 100, 2);
                    accountBalance -= coin;
                    change.Add(Constants.AcceptedCoins[i]);
                }
            }
            return change;
        }

        public async Task<Tuple<bool, Product?, string>> CreateProductAsync(ProductRequestModel model)
        {
            if (model == null)
                return new Tuple<bool, Product?, string>(false, null, "Enter a valid product request");

            var (success, user) = await _userService.GetUserAsync();
            if (user == null || !success)
                return new Tuple<bool, Product?, string>(false, null, "Unable to fetch user");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DateCreated = DateTime.Now,
                CreatedBy = user.Email
            };

            await _dbContext.AddAsync(product);
            var result = await _dbContext.SaveChangesAsync();
            return new Tuple<bool, Product?, string>(result > 0, product, result > 0 ? "Product added successfully" : "Unable to add product");
        }

        public async Task<Tuple<bool, string>> DeleteProductAsync(Guid productId)
        {
            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.Id == productId);
            if (product == null)
                return new Tuple<bool, string>(false, "Invalid product Id");

            _dbContext.Remove(product);
            var result = await _dbContext.SaveChangesAsync();
            return new Tuple<bool, string>(result > 0, result > 0 ? "Product deleted successfully" : "Unable to delete product");
        }

        public async Task<Tuple<bool, Product?, string>> GetProductAsync(Guid productId)
        {
            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.Id == productId);
            return new Tuple<bool, Product?, string>(product != null, product, product != null ? "Product retrieved successfully" : "Invalid product Id");
        }

        public async Task<Tuple<bool, Product?, string>> UpdateProductAsync(Guid Id, ProductRequestModel model)
        {
            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.Id == Id);
            if (product == null)
                return new Tuple<bool, Product?, string>(false, null, "Invalid product Id");

            var (success, loggedInUser) = await _userService.GetUserAsync();
            if (loggedInUser == null || !success)
                return new Tuple<bool, Product?, string>(false, null, "Unable to fetch user");
            if(!loggedInUser.Email.Equals(product.CreatedBy, StringComparison.InvariantCultureIgnoreCase))
                return new Tuple<bool, Product?, string>(false, null, "Permission denied");

            product.Price = model.Price;
            product.Description = model.Description;
            product.Name = model.Name;
            product.ModifiedDate = DateTime.Now;
            product.ModifiedBy = loggedInUser?.Email;

            _dbContext.Update(product);
            var result = await _dbContext.SaveChangesAsync();
            return new Tuple<bool, Product?, string>(result > 0, product, result > 0 ? "Product updated successfully" : "Unable to update product");
        }
    }
}
