using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentaion.Controllers
{
    [CustomAuth]
    public class TaxAndFee : Controller
    {
        private readonly ITaxAndFeeService _taxAndFeeService;
        private readonly IJwtService _jwtService;
        public TaxAndFee(ITaxAndFeeService taxAndFeeService, IJwtService jwtService)
        {
            _taxAndFeeService = taxAndFeeService;
            _jwtService = jwtService;
        }
        public async Task<IActionResult> Index()
        {
            TaxAndFeeViewModel taxAndFeeViewModel = await _taxAndFeeService.GetTaxAndFeeViewModelAsync();
            return View(taxAndFeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveChangesOfDefault(bool isDefault, int id)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"]);
            return await _taxAndFeeService.SaveChangesOfIsDefaultAsync(isDefault, id, userId);
        }

        [HttpPost]
        public async Task<IActionResult> SaveChangesOfEnabled(bool isEnabled, int id)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"]);
            return await _taxAndFeeService.SaveChangesOfIsEnabledAsync(isEnabled, id, userId);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"]);
            return await _taxAndFeeService.DeleteAsync(id, userId);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchValue)
        {
            TaxAndFeeViewModel taxAndFeeViewModel = await _taxAndFeeService.SearchAsync(searchValue);
            return PartialView("_TaxAndFeeTable", taxAndFeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTax(AddTaxViewModal addTaxViewModal)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"]);
            return await _taxAndFeeService.AddTaxAsync(addTaxViewModal, userId);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            return await _taxAndFeeService.EditAsync(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxes()
        {
            TaxAndFeeViewModel taxAndFeeViewModel = await _taxAndFeeService.GetTaxAndFeeViewModelAsync();
            return PartialView("_TaxAndFeeTable", taxAndFeeViewModel);
        }
    }
}