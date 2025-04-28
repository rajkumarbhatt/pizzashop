using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface ITaxAndFeeService
    {
        public Task<List<TaxesFee>> GetTaxesAsync();
        public Task<IActionResult> SaveChangesOfIsDefaultAsync(bool isDefault, int id, int userId);
        public Task<IActionResult> SaveChangesOfIsEnabledAsync(bool isEnabled, int id, int userId);
        public Task<IActionResult> DeleteAsync(int id, int userId);
        public Task<TaxAndFeeViewModel> SearchAsync(string searchValue);
        public Task<IActionResult> AddTaxAsync(AddTaxViewModal addTaxViewModal, int userId);
        public Task<IActionResult> EditAsync(int taxId);
        public Task<TaxAndFeeViewModel> GetTaxAndFeeViewModelAsync();
    }
}