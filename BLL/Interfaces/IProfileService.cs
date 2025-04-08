using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProfileService
    {
        public Task<ProfileViewModel?> GetUserDataFromUserIdAsync(int userId);
        public Task<IActionResult> UpdateUserDataFromUserIdAsync(int userId, ProfileViewModel ProfileViewModel);
        public Task<string> GetCountryByIdAsync(int countryId);
        public Task<string> GetStateByIdAsync(int stateId);
        public Task<string> GetCityByIdAsync(int cityId);
        public Task<List<Country>> GetCountriesAsync();
        public Task<List<State>> GetStatesAsync(int countryId);
        public Task<List<City>> GetCitiesAsync(int stateId);
        public Task<(List<Country> countries, List<State> states, List<City> cities)> SetCountriesStatesCitiesToViewBagAsync(ProfileViewModel ProfileViewModel);
    }
}