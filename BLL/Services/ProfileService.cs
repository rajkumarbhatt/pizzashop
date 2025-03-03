using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using DAL.DBContext;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class ProfileService : IProfileService
    {
        private readonly PizzaShopContext _context;
        private readonly IHttpContextAccessor _httpContentAccessor;

        public ProfileService(PizzaShopContext context, IHttpContextAccessor httpContentAccessor)
        {
            _context = context;
            _httpContentAccessor = httpContentAccessor;
        }

        public ProfileViewModel GetUserDataFromUserId(int userId)
        {
            var user = _context.Users.Find(userId);
            var role = _context.Roles.Find(user.RoleId);
            var country = _context.Countries.Find(user.CountryId);
            var state = _context.States.Find(user.StateId);
            var city = _context.Cities.Find(user.CityId);
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
                Role = role.Name,
                CountryId = user.CountryId ?? 0,
                StateId = user.StateId ?? 0,
                CityId = user.CityId ?? 0,
                RoleId = user.RoleId,
            };
            return ProfileViewModel;
        }

        public IActionResult UpdateUserDataFromUserId(int userId, ProfileViewModel ProfileViewModel)
        {
            if (userId != ProfileViewModel.Id)
            {
                return new JsonResult (new { success=false, message = "Unauthorized access" });
            } 
            if (_context.Users.Any(u => u.Username == ProfileViewModel.Username && u.Id != userId))
            {
                return new JsonResult(new { success=false, message = "Username already exists" });
            }  
            var user = _context.Users.Find(userId);
            user.FirstName = ProfileViewModel.FirstName;
            user.LastName = ProfileViewModel.LastName;
            user.Phone = ProfileViewModel.PhoneNumber;
            user.Address = ProfileViewModel.Address;
            user.ZipCode = ProfileViewModel.ZipCode;
            user.Username = ProfileViewModel.Username;
            user.CountryId = ProfileViewModel.CountryId;
            user.StateId = ProfileViewModel.StateId;
            user.CityId = ProfileViewModel.CityId;
            user.UpdatedBy = userId;

            if (ProfileViewModel.UserProfileImage != null)
            {
                // store filename in db and save image in wwwroot/profile-images
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileViewModel.UserProfileImage.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile-images", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    ProfileViewModel.UserProfileImage.CopyTo(fileStream);
                }
                user.ProfileImage = fileName;
            }
            
            _httpContentAccessor.HttpContext.Session.SetString("Username", user.Username);
            _httpContentAccessor.HttpContext.Session.SetString("ProfileImageURL", user.ProfileImage);

            _context.Users.Update(user);
            _context.SaveChanges();

            return new JsonResult(new { success=true, message = "Profile updated successfully" });
        }

        public string GetCountryById(int countryId)
        {
            return _context.Countries.Find(countryId).Name;
        }

        public string GetStateById(int stateId)
        {
            if (stateId == 0)
            {
                return "";
            }
            return _context.States.Find(stateId).Name;
        }

        public string GetCityById(int cityId)
        {
            if (cityId == 0)
            {
                return "";
            }
            return _context.Cities.Find(cityId).Name;
        }

        public List<Country> GetCountries()
        {
            return _context.Countries.ToList();
        }

        public List<State> GetStates(int countryId)
        {
            return _context.States.Where(s => s.CountryId == countryId).ToList();
        }

        public List<City> GetCities(int stateId)
        {
            return _context.Cities.Where(c => c.StateId == stateId).ToList();
        }

        public (List<Country> countries, List<State> states, List<City> cities) SetCountriesStatesCitiesToViewBag(ProfileViewModel profileViewModel)
        {
            var countries = _context.Countries.ToList();
            var states = _context.States.Where(s => s.CountryId == profileViewModel.CountryId).ToList();
            var cities = _context.Cities.Where(c => c.StateId == profileViewModel.StateId).ToList();
            return (countries, states, cities);
        }
    }
}