using Match.Models;
using Match.Models.RequestModels;

namespace Match.Interfaces
{
    public interface IProductService
    {
        Task<Tuple<bool, Product?, string>> GetProductAsync(Guid productId);
        Task<Tuple<bool, Product?, string>> CreateProductAsync(ProductRequestModel product);
        Task<Tuple<bool, Product?, string>> UpdateProductAsync(Guid Id, ProductRequestModel product);
        Task<Tuple<bool, string>> DeleteProductAsync(Guid productId);
        Task<Tuple<bool, List<int>, string>> BuyProductAsync(Guid productId, int productCount);
    }
}
