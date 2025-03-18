using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ITaxAndFeeService
    {
        public List<TaxesFee> GetTaxes();
        public IActionResult SaveChangesOfIsDefault(bool isDefault, int id, int userId);
        public IActionResult SaveChangesOfIsEnabled(bool isEnabled, int id, int userId);
        public IActionResult Delete(int id, int userId);
        public TaxAndFeeViewModel Search(string searchValue);
        public IActionResult AddTax(int taxId, string taxName, bool isEnabled, string taxType, decimal taxAmount, int userId);
        public IActionResult Edit(int taxId);
    }
}