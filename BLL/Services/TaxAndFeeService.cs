using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class TaxAndFeeService : ITaxAndFeeService
    {
        private readonly PizzaShopContext _context;

        public TaxAndFeeService(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<List<TaxesFee>> GetTaxesAsync()
        {
            return await _context.TaxesFees.Where(t => t.IsDeleted == false).OrderBy(t => t.Id).ToListAsync();
        }

        public async Task<IActionResult> SaveChangesOfIsDefaultAsync(bool isDefault, int id, int userId)
        {
            TaxesFee tax = await _context.TaxesFees.FindAsync(id);
            tax.IsDefault = isDefault;
            tax.UpdatedBy = (short?)userId;
            tax.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> SaveChangesOfIsEnabledAsync(bool isEnabled, int id, int userId)
        {
            TaxesFee tax = await _context.TaxesFees.FindAsync(id);
            tax.IsEnabled = isEnabled;
            tax.UpdatedBy = (short?)userId;
            tax.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> DeleteAsync(int id, int userId)
        {
            TaxesFee tax = await _context.TaxesFees.FindAsync(id);
            tax.IsDeleted = true;
            tax.UpdatedBy = (short?)userId;
            tax.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Tax deleted successfully" });
        }

        public async Task<TaxAndFeeViewModel> SearchAsync(string searchValue)
        {
            List<TaxesFee> taxes;
            if (string.IsNullOrEmpty(searchValue))
            {
            taxes = await _context.TaxesFees
                .Where(t => t.IsDeleted == false)
                .OrderBy(t => t.Id)
                .ToListAsync();
            }
            else
            {
            searchValue = searchValue.ToLower();
            taxes = await _context.TaxesFees
                .Where(t => t.IsDeleted == false && 
                    (t.Name.ToLower().Contains(searchValue) || 
                     t.TaxType.ToLower().Contains(searchValue) || 
                     t.Amount.ToString().Contains(searchValue)))
                .OrderBy(t => t.Id)
                .ToListAsync();
            }

            TaxAndFeeViewModel taxAndFeeViewModel = new TaxAndFeeViewModel
            {
            Taxes = taxes
            };
            return taxAndFeeViewModel;
        }

        public async Task<IActionResult> AddTaxAsync(int taxId, string taxName, bool isEnabled, string taxType, decimal taxAmount, int userId)
        {
            if (taxId == -1)
            {
            if (await _context.TaxesFees.AnyAsync(t => t.Name == taxName && t.IsDeleted == false))
            {
                return new JsonResult(new { success = false, message = "Tax already exists" });
            }
            TaxesFee tax = new TaxesFee
            {
                Name = taxName,
                IsEnabled = isEnabled,
                TaxType = taxType,
                Amount = taxAmount,
                CreatedBy = (short)(short?)userId,
                CreatedAt = DateTime.Now
            };
            await _context.TaxesFees.AddAsync(tax);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Tax added successfully" });
            }
            else
            {
            if (await _context.TaxesFees.AnyAsync(t => t.Name == taxName && t.Id != taxId && t.IsDeleted == false))
            {
                return new JsonResult(new { success = false, message = "Tax already exists" });
            }
            TaxesFee tax = await _context.TaxesFees.FindAsync(taxId);
            tax.Name = taxName;
            tax.IsEnabled = isEnabled;
            tax.TaxType = taxType;
            tax.Amount = taxAmount;
            tax.UpdatedBy = (short?)userId;
            tax.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Tax updated successfully" });
            }
        }

        public async Task<IActionResult> EditAsync(int taxId)
        {
            TaxesFee tax = await _context.TaxesFees.FindAsync(taxId);
            return new JsonResult(new { tax = tax });
        }
        public async Task<TaxAndFeeViewModel> GetTaxAndFeeViewModelAsync()
        {
            List<TaxesFee> taxes = await GetTaxesAsync();
            TaxAndFeeViewModel taxAndFeeViewModel = new TaxAndFeeViewModel
            {
            Taxes = taxes
            };
            return taxAndFeeViewModel;
        }
    }
}