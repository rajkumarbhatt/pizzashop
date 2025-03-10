using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces
{
    public interface IProfileService
    {
        public ProfileViewModel GetUserDataFromUserId(int userId);
        public IActionResult UpdateUserDataFromUserId(int userId, ProfileViewModel ProfileViewModel);
        public string GetCountryById(int countryId);
        public string GetStateById(int stateId);
        public string GetCityById(int cityId);
        public List<Country> GetCountries();
        public List<State> GetStates(int countryId);
        public List<City> GetCities(int stateId);
        public (List<Country> countries, List<State> states, List<City> cities) SetCountriesStatesCitiesToViewBag(ProfileViewModel ProfileViewModel);
    }
}