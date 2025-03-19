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
        public IActionResult Index()
        {
            List<TaxesFee> taxes = _taxAndFeeService.GetTaxes();
            TaxAndFeeViewModel taxAndFeeViewModel = new TaxAndFeeViewModel
            {
                Taxes = taxes
            };
            return View(taxAndFeeViewModel);
        }

        [HttpPost]
        public IActionResult SaveChangesOfDefault (bool isDefault, int id)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _taxAndFeeService.SaveChangesOfIsDefault(isDefault, id, userId);
        }

        [HttpPost]
        public IActionResult SaveChangesOfEnabled (bool isEnabled, int id)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _taxAndFeeService.SaveChangesOfIsEnabled(isEnabled, id, userId);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _taxAndFeeService.Delete(id, userId);
        }

        [HttpGet]
        public IActionResult Search(string searchValue)
        {
            TaxAndFeeViewModel taxAndFeeViewModel = _taxAndFeeService.Search(searchValue);
            return PartialView("_TaxAndFeeTable", taxAndFeeViewModel);
        }

        [HttpPost]
        public IActionResult AddTax(int taxId, string taxName, bool isEnabled, string taxType, decimal taxAmount)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _taxAndFeeService.AddTax(taxId, taxName, isEnabled, taxType, taxAmount, userId);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return _taxAndFeeService.Edit(id);
        }

        [HttpGet]
        public IActionResult GetTaxes()
        {
            List<TaxesFee> taxes = _taxAndFeeService.GetTaxes();
            TaxAndFeeViewModel taxAndFeeViewModel = new TaxAndFeeViewModel
            {
                Taxes = taxes
            };
            return PartialView("_TaxAndFeeTable", taxAndFeeViewModel);
        }
    }
}