using Match.Interfaces;
using Match.Models;
using Match.Models.RequestModels;
using Match.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Match.Services
{
    public class UserService : IUserService
	{
		private UserManager<User> _userManager;
		private IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IDistributedCache _cache;

		public UserService(UserManager<User> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IDistributedCache memoryCache)
		{
			_userManager = userManager;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
			_cache = memoryCache;
		}

		//GENRATE TOKEN
		public async Task<string> GenerateToken(User user)
		{
			var userRole = await _userManager.GetRolesAsync(user);
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, $"{user.FirstName} {" "} {user.LastName}"),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
			};

			foreach (var role in userRole)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(Constants.tokenExpiryInMinutes),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}



		//LOGIN
		public async Task<Tuple<bool, User?, string?>> LoginUserAsync(LoginRequestModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null)
				return new Tuple<bool, User?, string?>(false, null, "Incorrect email or password");

			var existingSession = await _cache.GetStringAsync(user.Email);
			if (!string.IsNullOrEmpty(existingSession))
				return new Tuple<bool, User?, string?>(false, null, "There is already an active session using your account");

			var isCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

			if (!isCorrect)
				return new Tuple<bool, User?, string?>(false, null, "Incorrect email or password");

			
			var token = await GenerateToken(user);

			await _cache.SetStringAsync(user.Email, user.Email, Constants.cacheOptions);

			return new Tuple<bool, User?, string?>(true, user, token);
		}



		//REGISTER
		public async Task<Tuple<bool, User?, string?>> RegisterUserAsync(RegisterRequestModel model)
		{
			if (model == null)
				throw new NullReferenceException("Invalid signup model");

			var existinguser = await _userManager.FindByEmailAsync(model.Email);
			if (existinguser != null)
				return new Tuple<bool, User?, string?>(false, null, "User already exists");

			if (model.Password != model.ConfirmPassword)
				return new Tuple<bool, User?, string?>(false, null, "Passwords do not match");

			var user = new User
			{
				Email = model.Email,
				UserName = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				DateCreated = DateTime.Now,
				CreatedBy = Constants.DefaultCreator
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded)
				return new Tuple<bool, User?, string?>(false, null, "Unable to register user");
			
			var addToRole = await _userManager.AddToRoleAsync(user, model.Role.ToString());
			if(!addToRole.Succeeded)
				await _userManager.DeleteAsync(user);
			return new Tuple<bool, User?, string?>(true, user, "User Created Successfully");
		}

		public async Task<Tuple<bool, User?, string?>> DepositAsync(int coin)
        {
			if (!Constants.AcceptedCoins.Contains(coin))
				return new Tuple<bool, User?, string?>(false, null, "Invalid amount. Coin must be a valid amount");

			var (success, user) = await GetUserAsync();
			if (user == null || !success)
				return new Tuple<bool, User?, string?>(false, null, "Unable to fetch user data");

			user.AccountBalance += coin / 100;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
				return new Tuple<bool, User?, string?>(false, null, "Deposit failed");

			return new Tuple<bool, User?, string?>(true, user, "Deposit successful");
		}

		
		public async Task<Tuple<bool, User?>> UpdateUserAsync(UpdateUserRequestModel model)
		{
			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
				return new Tuple<bool, User?>(false, null);

			var (success, loggedInUser) = await GetUserAsync();

			user.Email = model.Email;
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.ModifiedDate = DateTime.Now;
			user.ModifiedBy = loggedInUser?.Email;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
				return new Tuple<bool, User?>(false, null);
			return new Tuple<bool, User?>(true, user);
		}

		public async Task<Tuple<bool, User?>> UpdateUserBalanceAsync(decimal newBalance)
		{
			var (success, user) = await GetUserAsync();
			if (user == null || !success)
				return new Tuple<bool, User?>(false, null);

			user.AccountBalance = newBalance;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
				return new Tuple<bool, User?>(false, null);
			return new Tuple<bool, User?>(true, user);
		}

		public async Task<Tuple<bool, User?>> GetUserById(string userId)
        {
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return new Tuple<bool, User?>(false, null);

			return new Tuple<bool, User?>(true, user);
		}

        public async Task<Tuple<bool, User?>> GetUserAsync()
		{
			var userID = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userID == null)
				return new Tuple<bool, User?>(false, null);

			return await GetUserById(userID);
		}

		public async Task<List<string>> GetUserRolesAsync()
		{
			var (success, user) = await GetUserAsync();
			if (!success || user == null)
				return new List<string>();

			return new List<string>(await _userManager.GetRolesAsync(user));
		}

		public async Task<string> DeleteUserAsync()
		{
			var (success, user) = await GetUserAsync();
			if (!success || user == null)
				return "Invalid user";

			var result = await _userManager.DeleteAsync(user);
			if (!result.Succeeded)
				return "Unable to delete user";

			return "User deleted successfully";
		}

        public async Task<Tuple<bool, string?>> LogoutUserAsync(LoginRequestModel model)
        {
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return new Tuple<bool, string?>(false, "Incorrect email or password");

			var isCorrect = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!isCorrect)
				return new Tuple<bool, string?>(false, "Incorrect email or password");

			await _cache.RemoveAsync(user.Email);

			return new Tuple<bool, string?>(true, "User logged out of all sessions");
		}
    }
}
