using BLL.Interfaces;
using BLL.Services;
using DAL.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Presentaion.Controllers
{
    public class CustomAuth : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestedUrl = context.HttpContext.Request.Path.Value;
            if (requestedUrl == null || !IsAuthorizedAsync(context.HttpContext.User, requestedUrl).Result)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "PageNotFound" },
                { "action", "Index" }
            });
            }
        }

        private async Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, string requestedUrl)
        {
            var permissionNameObj = new Dictionary<string, string>
            {
                { "UserList", "Users" },
                { "Dashboard", "Dashboard" },
                { "Profile", "Dashboard" },
                { "ChangePassword", "Dashboard" },
                { "RoleAndPermission", "RoleAndPermission" },
                { "Menu", "Menu" },
                { "TableAndSection", "TableAndSection" },
                { "TaxAndFee", "TaxAndFee" },
                { "Order", "Order" },
                { "Customer", "Customers" },
                { "account", "Dashboard"},
                { "OrderApp", "OrderApp" }
            };

            PizzaShopContext db = new PizzaShopContext();
            if (user.Identity?.IsAuthenticated == false)
            {
                return false;
            }

            var roleId = int.Parse(user.Claims.ElementAt(4).Value);
            INavBarService _navBarService = new NavBarService(db);
            IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
            var permissions = await _navBarService.GetRolePermissionsFromRoleIdAsync(roleId);
            var permissionsBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(permissions);
            _httpContextAccessor.HttpContext?.Session.Set("permissions", permissionsBytes);

            var controller = requestedUrl.Split('/')[1];
            if (!permissionNameObj.TryGetValue(controller, out var permissionName))
            {
                return false;
            }

            var permission = await db.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.Permission.Name == permissionName);

            if (permission == null)
            {
                return false;
            }

            if (requestedUrl.ToLower().Contains("edit") || requestedUrl.ToLower().Contains("update") || requestedUrl.ToLower().Contains("create") || requestedUrl.ToLower().Contains("add"))
            {
                return permission.CanView == true && permission.CanEdit == true;
            }
            else if (requestedUrl.ToLower().Contains("delete"))
            {
                return permission.CanView == true && permission.CanDelete == true;
            }

            return permission.CanView == true;
        }
    }
}

