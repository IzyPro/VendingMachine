using Match.Interfaces;
using Match.Models;
using Match.Models.RequestModels;
using Match.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Match.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET api/<ProductController>/5
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(ResponseModel<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(Guid Id)
        {
            var (success, product, message) = await _productService.GetProductAsync(Id);
            if (success && product != null)
            {
                var responseModel = new ResponseModel<Product>
                {
                    Data = product,
                    Success = true,
                    Message = message,
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<Product?>
                {
                    Data = null,
                    Success = false,
                    Message = message,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        [HttpPost("buy")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ResponseModel<List<int>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<List<int>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Buy([FromQuery] Guid id, [FromQuery] int productCount)
        {
            var (success, change, message) = await _productService.BuyProductAsync(id, productCount);
            if (success && change != null)
            {
                var responseModel = new ResponseModel<List<int>>
                {
                    Data = change,
                    Success = true,
                    Message = message,
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<List<int>>
                {
                    Data = change,
                    Success = false,
                    Message = message,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }


        // POST api/<ProductController>
        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ResponseModel<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ProductRequestModel model)
        {
            var (success, product, message) = await _productService.CreateProductAsync(model);
            if (success && product != null)
            {
                var responseModel = new ResponseModel<Product>
                {
                    Data = product,
                    Success = true,
                    Message = message,
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<Product>
                {
                    Data = null,
                    Success = false,
                    Message = message,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ResponseModel<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProductRequestModel model)
        {
            var (success, product, message) = await _productService.UpdateProductAsync(id, model);
            if (success && product != null)
            {
                var responseModel = new ResponseModel<Product>
                {
                    Data = product,
                    Success = true,
                    Message = message,
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<Product>
                {
                    Data = null,
                    Success = false,
                    Message = message,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var (success, message) = await _productService.DeleteProductAsync(id);
            if (success)
            {
                var responseModel = new ResponseModel<string?>
                {
                    Data = null,
                    Success = true,
                    Message = message,
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<string?>
                {
                    Data = null,
                    Success = false,
                    Message = message,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }
    }
}
