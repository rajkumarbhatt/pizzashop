using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;

namespace Presentaion.Controllers
{
    [CustomAuth]
    public class ProfileController : Controller
    {
        private readonly IProfileService _ProfileService;   
        private readonly IJwtService _jwtService;
        public ProfileController(IProfileService ProfileService, IJwtService jwtService)
        {
            _ProfileService = ProfileService;
            _jwtService = jwtService;
        }
        public IActionResult Index()
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            ProfileViewModel ProfileViewModel = _ProfileService.GetUserDataFromUserId(userId);
            var country = _ProfileService.GetCountryById(ProfileViewModel.CountryId ?? 0);
            var state = _ProfileService.GetStateById(ProfileViewModel.StateId ?? 0);
            var city = _ProfileService.GetCityById(ProfileViewModel.CityId ?? 0);
            ViewBag.Country = country;
            ViewBag.State = state;
            ViewBag.City = city;
            return View(ProfileViewModel); 
        }
        public IActionResult Edit()
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            ProfileViewModel profileViewModel = _ProfileService.GetUserDataFromUserId(userId);
            var (countries, states, cities) = _ProfileService.SetCountriesStatesCitiesToViewBag(profileViewModel);
            ViewBag.Countries = countries;
            ViewBag.States = states;
            ViewBag.Cities = cities;
            return View(profileViewModel);
        }
        
        [HttpPost]
        public IActionResult EditProfile(ProfileViewModel ProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Validation errors" });
            }
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _ProfileService.UpdateUserDataFromUserId(userId, ProfileViewModel);
        }

        public JsonResult GetStates(int countryId)
        {
            var states = _ProfileService.GetStates(countryId);
            return Json(states);
        } 

        public JsonResult GetCities(int stateId)
        {
            var cities = _ProfileService.GetCities(stateId);
            return Json(cities);
        }


    }
};
