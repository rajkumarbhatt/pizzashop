using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DAL.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class UserListService : IUserListService
    {
        private readonly PizzaShopContext _context;
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserListService> _logger;

        public UserListService(ILogger<UserListService> logger, PizzaShopContext context, IJwtService jwtService, INavBarService navBarService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _jwtService = jwtService;
            _navBarService = navBarService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                return await _context.Users.Where(u => u.IsDeleted == false).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users");
                Console.WriteLine(ex.Message);
                return new List<User>();
            }
        }

        public async Task<JsonResult> DeleteUserAsync(int userId, int userIdLoggedIn)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }
                user.IsDeleted = true;
                user.UpdatedBy = userIdLoggedIn;
                user.UpdatedAt = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} deleted successfully by user with ID {UserIdLoggedIn}", userId, userIdLoggedIn);
                return new JsonResult(new
                {
                    success = true,
                    message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the user",
                    error = ex.Message
                });
            }
        }

        public async Task<User> GetUserDataByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? new User();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user data for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return new User();
            }
        }

        public async Task<string> GetRoleByIdAsync(int roleId)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                return role?.Name ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving role data for role ID {RoleId}", roleId);
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public async Task<List<Role>> GetRolesAsync(int userId)
        {
            try
            {
                int roleId = await _navBarService.GetRoleIdFromUserIdAsync(userId);
                if (roleId == 2)
                {
                    return await _context.Roles.Where(r => r.Id != 1).ToListAsync();
                }
                return await _context.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles");
                Console.WriteLine(ex.Message);
                return new List<Role>();
            }
        }

        public async Task<EditUserViewModel> GetUserDataFromUserIdAsync(int userId, int userIdLoggedIn)
        {
            try
            {
                var usernameLoggedIn = await _navBarService.GetUsernameFromUserIdAsync(userIdLoggedIn);
                var profileImageURLLoggedIn = await _navBarService.GetProfileImageUrlFromUserIdAsync(userIdLoggedIn);
                var user = await GetUserDataByIdAsync(userId);
                var roleIdLoggedIn = await _navBarService.GetRoleIdFromUserIdAsync(userIdLoggedIn);
                var userViewModel = new EditUserViewModel
                {
                    idLoggednin = userIdLoggedIn,
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName ?? "",
                    Email = user.Email,
                    PhoneNumber = user.Phone ?? "",
                    Address = user.Address ?? "",
                    ZipCode = user.ZipCode ?? "",
                    Status = user.Status ?? false,
                    Username = usernameLoggedIn,
                    RoleId = roleIdLoggedIn,
                    ProfileImageURL = profileImageURLLoggedIn,
                    RoleIdRequestedUser = user.RoleId,
                    CountryId = user.CountryId ?? 0,
                    StateId = user.StateId ?? 0,
                    CityId = user.CityId ?? 0,
                    UsernameRequestedUSer = user.Username,
                };
                return userViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user data for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return new EditUserViewModel
                {
                    FirstName = string.Empty,
                    Email = string.Empty,
                    RoleIdRequestedUser = 0,
                    UsernameRequestedUSer = string.Empty
                };
            }
        }

        public async Task<JsonResult> UpdateUserDataFromUserIdAsync(int userIdLoggedIn, EditUserViewModel userViewModel)
        {
            try
            {
                var user = await GetUserDataByIdAsync(userViewModel.Id);
                if (user == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }
                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == userViewModel.UsernameRequestedUSer.ToLower() && u.Id != userViewModel.Id))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Username already exists"
                    });
                }
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == userViewModel.Email.ToLower() && u.Id != userViewModel.Id))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
                }
                user.FirstName = userViewModel.FirstName;
                user.LastName = userViewModel.LastName;
                user.Email = userViewModel.Email ?? "";
                user.Phone = userViewModel.PhoneNumber ?? "";
                user.Address = userViewModel.Address;
                user.ZipCode = userViewModel.ZipCode;
                user.Status = userViewModel.Status;
                user.Username = userViewModel.UsernameRequestedUSer;
                user.RoleId = userViewModel.RoleIdRequestedUser;
                user.CountryId = userViewModel.CountryId;
                user.StateId = userViewModel.StateId;
                user.CityId = userViewModel.CityId;
                user.UpdatedBy = userIdLoggedIn;
                user.UpdatedAt = DateTime.Now;

                if (userViewModel.ProfileImage != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userViewModel.ProfileImage.FileName);
                    if (!userViewModel.ProfileImage.ContentType.Contains("image"))
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
                        await userViewModel.ProfileImage.CopyToAsync(fileStream);
                    }
                    user.ProfileImage = fileName;
                }
                if (userViewModel.Id == userIdLoggedIn)
                {
                    _httpContextAccessor.HttpContext?.Session.SetString("Username", user.Username);
                    _httpContextAccessor.HttpContext?.Session.SetString("ProfileImageURL", user.ProfileImage ?? "");
                    _httpContextAccessor.HttpContext?.Session.SetInt32("RoleId", user.RoleId);
                }
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} updated successfully by user with ID {UserIdLoggedIn}", userViewModel.Id, userIdLoggedIn);
                return new JsonResult(new
                {
                    success = true,
                    message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with ID {UserId}", userViewModel.Id);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while updating the user",
                    error = ex.Message
                });
            }
        }

        public async Task<JsonResult> CreateUserAsync(int userIdLoggedIn, CreateUserViewModel createUserViewModel)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == createUserViewModel.UsernameRequestedUser.ToLower()))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Username already exists"
                    });
                }
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == createUserViewModel.Email.ToLower()))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Email already exists"
                    });
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
                    if (!createUserViewModel.ProfileImage.ContentType.Contains("image"))
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Invalid file type"
                        });
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(createUserViewModel.ProfileImage.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile-images", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await createUserViewModel.ProfileImage.CopyToAsync(fileStream);
                    }
                    user.ProfileImage = fileName;
                }
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} created successfully by user with ID {UserIdLoggedIn}", user.Id, userIdLoggedIn);
                return new JsonResult(new
                {
                    success = true,
                    message = "User created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user");
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while creating the user",
                    error = ex.Message
                });
            }
        }

        public async Task<int> GetTotalUsersAsync()
        {
            try
            {
                return await _context.Users.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving total users");
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<(List<User>, int totalRecords)> GetUsersAsync(int pageIndex, int pageSize, int userId)
        {
            try
            {
                var roleId = await _navBarService.GetRoleIdFromUserIdAsync(userId);
                var query = _context.Users.Where(u => u.IsDeleted == false).OrderBy(u => u.FirstName);
                if (roleId == 2)
                {
                    query = (IOrderedQueryable<User>)query.Where(u => u.RoleId != 1);
                }
                int totalRecords = await query.CountAsync();
                var users = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return (users, totalRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users");
                Console.WriteLine(ex.Message);
                return (new List<User>(), 0);
            }
        }

        public async Task<(List<User>, int totalRecords)> GetUsersWithSearchAsync(int pageIndex, int pageSize, string searchValue, string sortColumn, string sortColumnDirection)
        {
            try
            {
                int userId = await _jwtService.GetUserIdFromJwtTokenAsync(_httpContextAccessor.HttpContext?.Request.Cookies["token"] ?? "");
                var roleId = await _navBarService.GetRoleIdFromUserIdAsync(userId);
                var query = _context.Users.Where(u => u.IsDeleted == false);
                if (roleId == 2)
                {
                    query = (IOrderedQueryable<User>)query.Where(u => u.RoleId != 1);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(u => (u.FirstName.ToLower().Contains(searchValue)) || (u.Email.ToLower().Contains(searchValue)) || (u.Phone.ToLower().Contains(searchValue)));
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
                int totalRecords = await query.CountAsync();
                var users = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return (users, totalRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users with search");
                Console.WriteLine(ex.Message);
                return (new List<User>(), 0);
            }
        }

        public async Task<UserListViewModel> GetUsersListViewModelAsync(int pageIndex, int pageSize, int userId)
        {
            try
            {
                var roleId = await _navBarService.GetRoleIdFromUserIdAsync(userId);
                var (users, totalUsers) = await GetUsersAsync(pageIndex, pageSize, userId);
                var rolePermissions = await _navBarService.GetRolePermissionsFromRoleIdAsync(roleId);
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
                return userListViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users list view model");
                Console.WriteLine(ex.Message);
                return new UserListViewModel
                {
                    Users = new List<User>(),
                    PageIndex = 1,
                    PageSize = 10,
                    TotalPages = 0,
                    TotalUsers = 0,
                    Permissions = new List<PermissionModel>()
                };
            }
        }

        public async Task<UserListViewModel> GetUsersListViewModelSearchAsync(int pageIndex, int pageSize, string sortColumn, string sortColumnDirection, string searchValue)
        {
            try
            {
                var userId = await _jwtService.GetUserIdFromJwtTokenAsync(_httpContextAccessor.HttpContext?.Request.Cookies["token"] ?? "");
                var username = await _navBarService.GetUsernameFromUserIdAsync(userId);
                var profileImageURL = await _navBarService.GetProfileImageUrlFromUserIdAsync(userId);
                var roleId = await _navBarService.GetRoleIdFromUserIdAsync(userId);
                var (users, totalUsers) = await GetUsersWithSearchAsync(pageIndex, pageSize, searchValue ?? "", sortColumn, sortColumnDirection);
                int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
                int originalPageIndex = pageIndex;
                pageIndex = totalPages > 0 ? Math.Clamp(pageIndex, 1, totalPages) : 1;
                if (pageIndex != originalPageIndex)
                {
                    (users, totalUsers) = await GetUsersWithSearchAsync(pageIndex, pageSize, searchValue ?? "", sortColumn, sortColumnDirection);
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
                return userListViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users list view model with search");
                Console.WriteLine(ex.Message);
                return new UserListViewModel
                {
                    Users = new List<User>(),
                    PageIndex = 1,
                    PageSize = 10,
                    TotalPages = 0,
                    TotalUsers = 0,
                    Permissions = new List<PermissionModel>()
                };
            }
        }

        public async Task<CreateUserViewModel> GetCreateUserViewModelAsync()
        {
            try
            {
                var userIdLoggedIn = await _jwtService.GetUserIdFromJwtTokenAsync(_httpContextAccessor.HttpContext?.Request.Cookies["token"] ?? "");
                var usernameLoggedIn = await _navBarService.GetUsernameFromUserIdAsync(userIdLoggedIn);
                var profileImageURL = await _navBarService.GetProfileImageUrlFromUserIdAsync(userIdLoggedIn);
                var roleId = await _navBarService.GetRoleIdFromUserIdAsync(userIdLoggedIn);
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
                return createUserViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving create user view model");
                Console.WriteLine(ex.Message);
                return new CreateUserViewModel
                {
                    FirstName = "",
                    Email = "",
                    Password = "",
                    UsernameRequestedUser = "",
                    RoleId = 0 
                };
            }
        }
    }
}