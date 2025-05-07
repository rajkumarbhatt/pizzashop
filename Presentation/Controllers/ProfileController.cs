using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;

namespace Presentaion.Controllers
{
    [CustomAuth]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ProfileController : Controller
    {
        private readonly IProfileService _ProfileService;
        private readonly IJwtService _jwtService;
        public ProfileController(IProfileService ProfileService, IJwtService jwtService)
        {
            _ProfileService = ProfileService;
            _jwtService = jwtService;
        }
        public async Task<IActionResult> Index()
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            ProfileViewModel ProfileViewModel = await _ProfileService.GetUserDataFromUserIdAsync(userId) ?? new ProfileViewModel();
            var country = await _ProfileService.GetCountryByIdAsync(ProfileViewModel.CountryId ?? 0);
            var state = await _ProfileService.GetStateByIdAsync(ProfileViewModel.StateId ?? 0);
            var city = await _ProfileService.GetCityByIdAsync(ProfileViewModel.CityId ?? 0);
            ViewBag.Country = country;
            ViewBag.State = state;
            ViewBag.City = city;
            return View(ProfileViewModel);
        }

        public async Task<IActionResult> Edit()
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            ProfileViewModel profileViewModel = await _ProfileService.GetUserDataFromUserIdAsync(userId) ?? new ProfileViewModel();
            var (countries, states, cities) = await _ProfileService.SetCountriesStatesCitiesToViewBagAsync(profileViewModel);
            ViewBag.Countries = countries;
            ViewBag.States = states;
            ViewBag.Cities = cities;
            return View(profileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel ProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Validation errors" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _ProfileService.UpdateUserDataFromUserIdAsync(userId, ProfileViewModel);
        }

        public async Task<JsonResult> GetStates(int countryId)
        {
            var states = await _ProfileService.GetStatesAsync(countryId);
            return Json(states);
        }

        public async Task<JsonResult> GetCities(int stateId)
        {
            var cities = await _ProfileService.GetCitiesAsync(stateId);
            return Json(cities);
        }
    }
};
