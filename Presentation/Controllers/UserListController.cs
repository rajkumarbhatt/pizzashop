using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.ViewModels;
using BLL.Interfaces;

namespace Presentaion.Controllers
{
    [CustomAuth]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class UserListController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;
        private readonly IUserListService _userListService;
        private readonly IProfileService _profileService;
        private readonly IEmailService _emailService;
        public UserListController(IJwtService jwtService, INavBarService navBarService, IUserListService userListService, IProfileService profileService, IEmailService emailService)
        {
            _jwtService = jwtService;
            _navBarService = navBarService;
            _userListService = userListService;
            _profileService = profileService;
            _emailService = emailService;
        }

        public async Task<IActionResult> IndexAsync(int pageIndex = 1, int pageSize = 5)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            UserListViewModel userListViewModel = await _userListService.GetUsersListViewModelAsync(pageIndex, pageSize, userId);
            return View(userListViewModel);
        }

        public async Task<IActionResult> SearchUserAsync(int pageIndex = 1, int pageSize = 5, string? searchValue = null, string sortColumn = "FirstName", string sortColumnDirection = "asc")
        {
            UserListViewModel userListViewModel = await _userListService.GetUsersListViewModelSearchAsync(pageIndex, pageSize, sortColumn, sortColumnDirection, searchValue ?? "");
            return PartialView("_UserList", userListViewModel);
        }

        [HttpDelete]
        [Route("UserList/DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            var userIdLoggedIn = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _userListService.DeleteUserAsync(userId, userIdLoggedIn);
        }

        [HttpGet]
        [Route("UserList/EditUser/{userId}")]
        public async Task<IActionResult> EditUserAsync(int userId)
        {
            var userIdLoggedIn = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            EditUserViewModel userViewModel = await _userListService.GetUserDataFromUserIdAsync(userId, userIdLoggedIn);
            var countries = await _profileService.GetCountriesAsync();
            var states = await _profileService.GetStatesAsync(userViewModel.CountryId ?? 0);
            var cities = await _profileService.GetCitiesAsync(userViewModel.StateId ?? 0);
            var roles = await _userListService.GetRolesAsync(userIdLoggedIn);
            ViewBag.Roles = roles;
            ViewBag.Countries = countries;
            ViewBag.States = states;
            ViewBag.Cities = cities;
            return View(userViewModel);
        }

        [HttpPost]
        [Route("UserList/EditUser/{userId}")]
        public async Task<IActionResult> EditUserAsync(EditUserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Validation Error" });
            }
            var userIdLoggedIn = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _userListService.UpdateUserDataFromUserIdAsync(userIdLoggedIn, userViewModel);
        }

        [HttpGet]
        [Route("UserList/CreateUser")]
        public async Task<IActionResult> CreateUserAsync()
        {
            var userIdLoggedIn = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            var countries = await _profileService.GetCountriesAsync();
            var roles = await _userListService.GetRolesAsync(userIdLoggedIn);
            ViewBag.Roles = roles;
            ViewBag.Countries = countries;
            CreateUserViewModel createUserViewModel = await _userListService.GetCreateUserViewModelAsync();
            return View(createUserViewModel);
        }

        [HttpPost]
        [Route("UserList/CreateUser")]
        public async Task<IActionResult> CreateUserAsync(CreateUserViewModel createUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Validation Error" });
            }
            var userIdLoggedIn = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            await _emailService.SendCreateUserEmailAsync(createUserViewModel.Email, createUserViewModel.Password);
            return await _userListService.CreateUserAsync(userIdLoggedIn, createUserViewModel);
        }
    }

}