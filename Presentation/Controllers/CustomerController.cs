using BLL.Interfaces;
using ClosedXML.Excel;
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

        public IActionResult Index()
        {
            CustomerViewModel customerViewModel = _customerService.GetCustomerDetails();
            return View(customerViewModel);
        }

        [HttpGet]
        public IActionResult FilterCustomers(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate)
        {
            CustomerViewModel customerViewModel = _customerService.FilterCustomers(pageIndex, pageSize, searchValue, time, sort, order, fromDate, toDate);
            return PartialView("_CustomersList", customerViewModel);
        }

        [HttpGet]
        public IActionResult ExportCustomers(string time, string searchValue, string fromDate, string toDate)
        {
            return File(_customerService.ExportCustomers(time, searchValue, fromDate, toDate), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
        }

        [HttpGet]
        public IActionResult GetCustomerDetails(int id)
        {
            CustomerHistory customerHistory = _customerService.GetCustomerDetails(id);
            return PartialView("_CustomerDetails", customerHistory);
        }
    }
}