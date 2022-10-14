using Match.Interfaces;
using Match.Models;
using Match.Models.DTOs;
using Match.Models.RequestModels;
using Match.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Match.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            var (success, user, message) = await _userService.RegisterUserAsync(model);
            if (success && user != null)
            {
                var responseModel = new ResponseModel<LoginResponseDTO>()
                {
                    Success = true,
                    Data = new LoginResponseDTO
                    {
                        UserId = user.Id,
                        email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AccountBalance = user.AccountBalance,
                    },
                    ResponseCode = Constants.SuccessResponseCode,
                    Message = message
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<User>
                {
                    Data = null,
                    Success = false,
                    Message = message,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var (result, user, token) = await _userService.LoginUserAsync(model);
            if (result && user != null)
            {
                var responseModel = new ResponseModel<LoginResponseDTO>
                {
                    Data = new LoginResponseDTO
                    {
                        token = token,
                        UserId = user.Id,
                        email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AccountBalance = user.AccountBalance,
                    },
                    Success = true,
                    ResponseCode = Constants.SuccessResponseCode,
                    Message = "User logged in successfully"
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<LoginResponseDTO>
                {
                    Data = null,
                    Success = false,
                    Message = token,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        [HttpPost("deposit/{amount}")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ResponseModel<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deposit(int amount)
        {
            var (success, user, message) = await _userService.DepositAsync(amount);
            if (success && user != null)
            {
                var responseModel = new ResponseModel<LoginResponseDTO>
                {
                    Data = new LoginResponseDTO
                    {
                        UserId = user.Id,
                        email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AccountBalance = user.AccountBalance,
                    },
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


        [HttpPost("reset")]
        [Authorize(Roles = "Buyer")]
        [ProducesResponseType(typeof(ResponseModel<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reset()
        {
            var (success, user) = await _userService.UpdateUserBalanceAsync(0);
            if (success && user != null)
            {
                var responseModel = new ResponseModel<LoginResponseDTO>
                {
                    Data = new LoginResponseDTO
                    {
                        UserId = user.Id,
                        email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AccountBalance = user.AccountBalance,
                    },
                    Success = true,
                    Message = "Reset sucessful",
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
                    Message = "Reset failed",
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            var (success, user) = await _userService.GetUserAsync();
            if(success && user != null)
            {
                var responseModel = new ResponseModel<LoginResponseDTO>
                {
                    Data = new LoginResponseDTO
                    {
                        UserId = user.Id,
                        email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AccountBalance = user.AccountBalance,
                    },
                    Success = true,
                    Message = "User retrieved successfully",
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<User>
                {
                    Data = null,
                    Success = false,
                    Message = "Failed to retrieve user",
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string id)
        {
            var (success, user) = await _userService.GetUserById(id);
            if (success && user != null)
            {
                var responseModel = new ResponseModel<LoginResponseDTO>
                {
                    Data = new LoginResponseDTO
                    {
                        UserId = user.Id,
                        email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AccountBalance = user.AccountBalance,
                    },
                    Success = true,
                    Message = "User retrieved successfully",
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<User>
                {
                    Data = null,
                    Success = false,
                    Message = "Failed to retrieve user",
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        [HttpGet("roles")]
        [ProducesResponseType(typeof(ResponseModel<List<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<List<string>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserRoles()
        {
            var roles = await _userService.GetUserRolesAsync();
            var responseModel = new ResponseModel<List<string>>
            {
                Data = roles,
                Success = true,
                Message = "User roles retrieved successfully",
                ResponseCode = Constants.SuccessResponseCode,
            };
            return Ok(responseModel);
        }

        // PUT api/<UserController>/5
        [HttpPut]
        [ProducesResponseType(typeof(ResponseModel<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] UpdateUserRequestModel model)
        {
            var (success, user) = await _userService.UpdateUserAsync(model);
            if (success && user != null)
            {
                var responseModel = new ResponseModel<LoginResponseDTO>
                {
                    Data = new LoginResponseDTO
                    {
                        UserId = user.Id,
                        email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        AccountBalance = user.AccountBalance,
                    },
                    Success = true,
                    Message = "User updated successfully",
                    ResponseCode = Constants.SuccessResponseCode,
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<User>
                {
                    Data = null,
                    Success = false,
                    Message = "Failed to update user",
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }

        // DELETE api/<UserController>/5
        [HttpDelete]
        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete()
        {
            var message = await _userService.DeleteUserAsync();
            var responseModel = new ResponseModel<string>
            {
                Data = message,
                Success = true,
                Message = message,
                ResponseCode = Constants.SuccessResponseCode,
            };
            return Ok(responseModel);
        }

        [HttpPost("logout/all")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout([FromBody] LoginRequestModel model)
        {
            var (success, message) = await _userService.LogoutUserAsync(model);
            if (success)
            {
                var responseModel = new ResponseModel<string?>
                {
                    Data = message,
                    Success = true,
                    ResponseCode = Constants.SuccessResponseCode,
                    Message = message
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new ResponseModel<string?>
                {
                    Data = message,
                    Success = false,
                    Message = message,
                    ResponseCode = Constants.BadRequestResponseCode,
                };
                return BadRequest(responseModel);
            }
        }
    }
}
