using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers
{

    [CustomAuth]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            CustomerViewModel customerViewModel = await _customerService.GetCustomerDetailsAsync();
            return View(customerViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FilterCustomers(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate)
        {
            CustomerViewModel customerViewModel = await _customerService.FilterCustomersAsync(pageIndex, pageSize, searchValue, time, sort, order, fromDate, toDate);
            return PartialView("_CustomersList", customerViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ExportCustomers(string time, string searchValue, string fromDate, string toDate)
        {
            return File(await _customerService.ExportCustomersAsync(time, searchValue, fromDate, toDate), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerDetails(int id)
        {
            CustomerHistory customerHistory = await _customerService.GetCustomerDetailsAsync(id);
            return PartialView("_CustomerDetails", customerHistory);
        }
    }
}