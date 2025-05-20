using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using DAL.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class ProfileService : IProfileService
    {
        private readonly PizzaShopContext _context;
        private readonly IHttpContextAccessor _httpContentAccessor;
        private readonly ILogger<ProfileService> _logger;
        public ProfileService(PizzaShopContext context, IHttpContextAccessor httpContentAccessor, ILogger<ProfileService> logger)
        {
            _logger = logger;
            _context = context;
            _httpContentAccessor = httpContentAccessor;
        }
        public async Task<ProfileViewModel?> GetUserDataFromUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return null;
                }
                var role = await _context.Roles.FindAsync(user?.RoleId);
                var country = await _context.Countries.FindAsync(user?.CountryId);
                var state = await _context.States.FindAsync(user?.StateId);
                var city = await _context.Cities.FindAsync(user?.CityId);
                if (user == null)
                {
                    return null;
                }

                ProfileViewModel ProfileViewModel = new ProfileViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.Phone,
                    Address = user.Address,
                    ZipCode = user.ZipCode,
                    Username = user.Username,
                    ProfileImageURL = user.ProfileImage,
                    Role = role?.Name,
                    CountryId = user.CountryId ?? 0,
                    StateId = user.StateId ?? 0,
                    CityId = user.CityId ?? 0,
                    RoleId = user.RoleId,
                };
                return ProfileViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user data for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<IActionResult> UpdateUserDataFromUserIdAsync(int userId, ProfileViewModel ProfileViewModel)
        {
            try
            {
                if (userId != ProfileViewModel.Id)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Unauthorized access"
                    });
                }
                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == ProfileViewModel.Username.ToLower() && u.Id != userId))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Username already exists"
                    });
                }
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }

                user.FirstName = ProfileViewModel.FirstName ?? user.FirstName;
                user.LastName = ProfileViewModel.LastName;
                user.Phone = ProfileViewModel.PhoneNumber ?? user.Phone;
                user.Address = ProfileViewModel.Address;
                user.ZipCode = ProfileViewModel.ZipCode;
                user.Username = ProfileViewModel.Username ?? user.Username;
                user.CountryId = ProfileViewModel.CountryId;
                user.StateId = ProfileViewModel.StateId;
                user.CityId = ProfileViewModel.CityId;
                user.UpdatedBy = userId;

                if (ProfileViewModel.UserProfileImage != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileViewModel.UserProfileImage.FileName);
                    if (!ProfileViewModel.UserProfileImage.ContentType.Contains("image"))
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Invalid file type"
                        });
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile-images", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await ProfileViewModel.UserProfileImage.CopyToAsync(fileStream);
                    }
                    user.ProfileImage = fileName;
                }

                _httpContentAccessor.HttpContext?.Session.SetString("Username", user.Username);
                _httpContentAccessor.HttpContext?.Session.SetString("ProfileImageURL", user.ProfileImage ?? string.Empty);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User data updated successfully for user ID {UserId}", userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Profile updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user data for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while updating the profile",
                    error = ex.Message
                });
            }
        }
        public async Task<string> GetCountryByIdAsync(int countryId)
        {
            try
            {
                var country = await _context.Countries.FindAsync(countryId);
                return country?.Name ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving country by ID {CountryId}", countryId);
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        public async Task<string> GetStateByIdAsync(int stateId)
        {
            try
            {
                if (stateId == 0)
                {
                    return "";
                }
                var state = await _context.States.FindAsync(stateId);
                return state?.Name ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving state by ID {StateId}", stateId);
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        public async Task<string> GetCityByIdAsync(int cityId)
        {
            try
            {
                if (cityId == 0)
                {
                    return "";
                }
                var city = await _context.Cities.FindAsync(cityId);
                return city?.Name ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving city by ID {CityId}", cityId);
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        public async Task<List<Country>> GetCountriesAsync()
        {
            try
            {
                return await _context.Countries.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving countries");
                Console.WriteLine(ex.Message);
                return new List<Country>();
            }
        }
        public async Task<List<State>> GetStatesAsync(int countryId)
        {
            try
            {
                return await _context.States.Where(s => s.CountryId == countryId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving states for country ID {CountryId}", countryId);
                Console.WriteLine(ex.Message);
                return new List<State>();
            }
        }
        public async Task<List<City>> GetCitiesAsync(int stateId)
        {
            try
            {
                return await _context.Cities.Where(c => c.StateId == stateId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving cities for state ID {StateId}", stateId);
                Console.WriteLine(ex.Message);
                return new List<City>();
            }
        }
        public async Task<(List<Country> countries, List<State> states, List<City> cities)> SetCountriesStatesCitiesToViewBagAsync(ProfileViewModel profileViewModel)
        {
            try
            {
                var countries = await _context.Countries.ToListAsync();
                var states = await _context.States.Where(s => s.CountryId == profileViewModel.CountryId).ToListAsync();
                var cities = await _context.Cities.Where(c => c.StateId == profileViewModel.StateId).ToListAsync();
                return (countries, states, cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving countries, states, and cities");
                Console.WriteLine(ex.Message);
                return (new List<Country>(), new List<State>(), new List<City>());
            }
        }
    }
}