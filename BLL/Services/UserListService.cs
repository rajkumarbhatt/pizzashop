using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DAL.DBContext;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class UserListService : IUserListService
    {
        private readonly PizzaShopContext _context;
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserListService(PizzaShopContext context, IJwtService jwtService, INavBarService navBarService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtService = jwtService;
            _navBarService = navBarService;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<User> GetUsers()
        {
            return _context.Users.Where(u => u.IsDeleted == false).ToList();
        }

        public JsonResult DeleteUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            user.IsDeleted = true;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "User deleted successfully" });
        }

        public User GetUserDataById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId) ?? new User();
        }

        public string GetRoleById(int roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
            return role?.Name ?? "";
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public EditUserViewModel GetUserDataFromUserId(int userId, int userIdLoggedIn)
        {
            var usernameLoggedIn = _navBarService.GetUsernameFromUserId(userIdLoggedIn);
            var profileImageURLLoggedIn = _navBarService.GetProfileImageUrlFromUserId(userIdLoggedIn);
            var user = GetUserDataById(userId);
            var roleIdLoggedIn = _navBarService.GetRoleIdFromUserId(userIdLoggedIn);
            var userViewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName ?? "",
                Email = user.Email,
                PhoneNumber = user.Phone,
                Address = user.Address,
                ZipCode = user.ZipCode,
                Status = user.Status ?? false,
                Username = usernameLoggedIn,
                RoleId = roleIdLoggedIn,
                ProfileImageURL = profileImageURLLoggedIn,
                RoleIdRequestedUser = user.RoleId,
                CountryId = user.CountryId,
                StateId = user.StateId,
                CityId = user.CityId,
                UsernameRequestedUSer = user.Username,
            };
            return userViewModel;
        }

        public JsonResult UpdateUserDataFromUserId(int userIdLoggedIn, EditUserViewModel userViewModel)
        {
            var user = GetUserDataById(userViewModel.Id);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            if (_context.Users.Any(u => u.Username == userViewModel.UsernameRequestedUSer && u.Id != userViewModel.Id))
            {
                return new JsonResult(new { success = false, message = "Username already exists" });
            }
            if (_context.Users.Any(u => u.Email == userViewModel.Email && u.Id != userViewModel.Id))
            {
                return new JsonResult(new { success = false, message = "Email already exists" });
            }
            user.FirstName = userViewModel.FirstName;
            user.LastName = userViewModel.LastName;
            user.Email = userViewModel.Email ?? "";
            user.Phone = userViewModel.PhoneNumber ?? "";
            user.Address = userViewModel.Address;
            user.ZipCode = userViewModel.ZipCode;
            user.Status = userViewModel.Status;
            user.Username = userViewModel.UsernameRequestedUSer;
            user.RoleId = userViewModel.RoleIdRequestedUser ?? 0;
            user.CountryId = userViewModel.CountryId;
            user.StateId = userViewModel.StateId;
            user.CityId = userViewModel.CityId;
            user.UpdatedBy = userIdLoggedIn;
            user.UpdatedAt = DateTime.Now;

            if (userViewModel.ProfileImage != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userViewModel.ProfileImage.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile-images", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    userViewModel.ProfileImage.CopyTo(fileStream);
                }
                user.ProfileImage = fileName;
            }
            if (userViewModel.Id == userIdLoggedIn)
            {
                _httpContextAccessor.HttpContext?.Session.SetString("Username", user.Username);
                _httpContextAccessor.HttpContext?.Session.SetString("ProfileImageURL", user.ProfileImage ?? "");
                _httpContextAccessor.HttpContext?.Session.SetInt32("RoleId", user.RoleId);
            }
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "User updated successfully" });
        }

        public JsonResult CreateUser(int userIdLoggedIn, CreateUserViewModel createUserViewModel)
        {
            if (_context.Users.Any(u => u.Username == createUserViewModel.UsernameRequestedUser))
            {
                return new JsonResult(new { success = false, message = "Username already exists" });
            }
            if (_context.Users.Any(u => u.Email == createUserViewModel.Email))
            {
                return new JsonResult(new { success = false, message = "Email already exists" });
            }
            var user = new User
            {
                FirstName = createUserViewModel.FirstName,
                LastName = createUserViewModel.LastName,
                Email = createUserViewModel.Email,
                Phone = createUserViewModel.PhoneNumber ?? "",
                Address = createUserViewModel.Address,
                ZipCode = createUserViewModel.ZipCode,
                Username = createUserViewModel.UsernameRequestedUser,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserViewModel.Password),
                RoleId = createUserViewModel.RoleIdRequestedUser,
                CountryId = createUserViewModel.CountryId,
                StateId = createUserViewModel.StateId,
                CityId = createUserViewModel.CityId,
                CreatedBy = userIdLoggedIn,
                CreatedAt = DateTime.Now,
                UpdatedBy = userIdLoggedIn,
                UpdatedAt = DateTime.Now,
                Status = true,
                IsDeleted = false
            };
            if (createUserViewModel.ProfileImage != null)
            {
                // store filename in db and save image in wwwroot/profile-images
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createUserViewModel.ProfileImage.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile-images", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    createUserViewModel.ProfileImage.CopyTo(fileStream);
                }
                user.ProfileImage = fileName;
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "User created successfully" });
        }

        public int GetTotalUsers()
        {
            return _context.Users.Count();
        }

        public (List<User>, int totalRecords) GetUsers(int pageIndex, int pageSize)
        {
            var query = _context.Users.Where(u => u.IsDeleted == false).OrderBy(u => u.FirstName);
            int totalRecords = query.Count();
            var users = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return (users, totalRecords);
        }

        public (List<User>, int totalRecords) GetUsersWithSearch(int pageIndex, int pageSize, string searchValue, string sortColumn, string sortColumnDirection)
        {
            var query = _context.Users.Where(u => u.IsDeleted == false);
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(u => u.FirstName.ToLower().Contains(searchValue));
            }
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                if (sortColumnDirection == "asc")
                {
                    switch (sortColumn)
                    {
                        case "FirstName":
                            query = query.OrderBy(u => u.FirstName);
                            break;
                        case "RoleId":
                            query = query.OrderBy(u => u.RoleId);
                            break;
                    }
                }
                else
                {
                    switch (sortColumn)
                    {
                        case "FirstName":
                            query = query.OrderByDescending(u => u.FirstName);
                            break;
                        case "RoleId":
                            query = query.OrderByDescending(u => u.RoleId);
                            break;
                    }
                }
            }
            int totalRecords = query.Count();
            var users = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return (users, totalRecords);
        }

    }
}