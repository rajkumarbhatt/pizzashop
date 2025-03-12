using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.ViewModels;
using BLL.Interfaces;

namespace Presentaion.Controllers
{
    [CustomAuth]
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

        public IActionResult Index(int pageIndex = 1, int pageSize = 5)
        {
            var userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            var roleId = _navBarService.GetRoleIdFromUserId(userId);
            var (users, totalUsers) = _userListService.GetUsers(pageIndex, pageSize);
            var rolePermissions = _navBarService.GetRolePermissionsFromRoleId(roleId);
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var userListViewModel = new UserListViewModel
            {
                Users = users,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalUsers = totalUsers,
                Permissions = rolePermissions
            };

            return View(userListViewModel);
        }

        public IActionResult SearchUser(int pageIndex = 1, int pageSize = 5, string? searchValue = null, string sortColumn = "FirstName", string sortColumnDirection = "asc")
        {
            var userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            var username = _navBarService.GetUsernameFromUserId(userId);
            var profileImageURL = _navBarService.GetProfileImageUrlFromUserId(userId);
            var roleId = _navBarService.GetRoleIdFromUserId(userId);

            var (users, totalUsers) = _userListService.GetUsersWithSearch(pageIndex, pageSize, searchValue ?? "", sortColumn, sortColumnDirection);
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            // Store original page index before adjustment
            int originalPageIndex = pageIndex;

            // Adjust pageIndex if out of bounds
            pageIndex = totalPages > 0 ? Math.Clamp(pageIndex, 1, totalPages) : 1;

            // Re-fetch if page index was adjusted
            if (pageIndex != originalPageIndex)
            {
                (users, totalUsers) = _userListService.GetUsersWithSearch(pageIndex, pageSize, searchValue ?? "", sortColumn, sortColumnDirection);
                totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            }

            var userListViewModel = new UserListViewModel
            {
                Users = users,
                Username = username,
                ProfileImageURL = profileImageURL,
                RoleId = roleId,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalUsers = totalUsers
            };

            return PartialView("_UserList", userListViewModel);
        }


        [HttpDelete]
        [Route("UserList/DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            return _userListService.DeleteUser(userId); 

        }

        [HttpGet]
        [Route("UserList/EditUser/{userId}")]
        public IActionResult EditUser(int userId)
        {
            var userIdLoggedIn = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            EditUserViewModel userViewModel = _userListService.GetUserDataFromUserId(userId, userIdLoggedIn);

            var countries = _profileService.GetCountries();
            var states = _profileService.GetStates(userViewModel.CountryId ?? 0);
            var cities = _profileService.GetCities(userViewModel.StateId ?? 0);
            var roles = _userListService.GetRoles();

            ViewBag.Roles = roles;
            ViewBag.Countries = countries;
            ViewBag.States = states;
            ViewBag.Cities = cities;

            return View(userViewModel);
        }

        [HttpPut]
        [Route("UserList/EditUser/{userId}")]
        public IActionResult EditUser(EditUserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {             
                return new JsonResult(new { success = false, message = "Validation Error" });
            }
            var userIdLoggedIn = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _userListService.UpdateUserDataFromUserId(userIdLoggedIn, userViewModel);
        }

        [HttpGet]
        [Route("UserList/CreateUser")]
        public IActionResult CreateUser()
        {
            var countries = _profileService.GetCountries();
            var roles = _userListService.GetRoles();
            var userIdLoggedIn = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            var usernameLoggedIn = _navBarService.GetUsernameFromUserId(userIdLoggedIn);
            var profileImageURL = _navBarService.GetProfileImageUrlFromUserId(userIdLoggedIn);
            var roleId = _navBarService.GetRoleIdFromUserId(userIdLoggedIn);

            CreateUserViewModel createUserViewModel = new CreateUserViewModel
            {
                Username = usernameLoggedIn,
                ProfileImageURL = profileImageURL,
                RoleId = roleId,
                FirstName = "", 
                Email = "", 
                Password = "", 
                UsernameRequestedUser = ""
            };

            ViewBag.Roles = roles;
            ViewBag.Countries = countries;

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
            var userIdLoggedIn = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");

            await _emailService.SendCreateUserEmailAsync(createUserViewModel.Email, createUserViewModel.Password);

            return _userListService.CreateUser(userIdLoggedIn, createUserViewModel);
        }
    }

}