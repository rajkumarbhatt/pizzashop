using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.CodeAnalysis.Differencing;

namespace Presentaion.Controllers
{
    [CustomAuth]
    public class RoleAndPermission : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;
        private readonly IUserListService _userListService;
        private readonly IRoleAndPermissionService _roleAndPermissionService;

        public RoleAndPermission(IJwtService jwtService, INavBarService navBarService, IUserListService userListService, IRoleAndPermissionService roleAndPermissionService)
        {
            _jwtService = jwtService;
            _navBarService = navBarService;
            _userListService = userListService;
            _roleAndPermissionService = roleAndPermissionService;
        }
        public async Task<IActionResult> Index()
        {
            RoleAndPermissionViewModel roleAndPermissionViewModel = await _roleAndPermissionService.GetRoleAndPermissionViewModelAsync();
            return View(roleAndPermissionViewModel);
        }

        [HttpGet]
        [Route("/RoleAndPermission/ViewPermissions/{roleIdRequested}")]
        public async Task<IActionResult> ViewPermissions(int roleIdRequested)
        {
            EditPermissionsViewModel editPermissionsViewModel = await _roleAndPermissionService.GetEditPermissionsViewModelAsync(roleIdRequested);
            return View(editPermissionsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPermissions([FromBody] List<PermissionChangeModel> changedPermissions)
        {
            var result = await _roleAndPermissionService.UpdateRolePermissionsAsync(changedPermissions);
            return result;
        }
    }
}