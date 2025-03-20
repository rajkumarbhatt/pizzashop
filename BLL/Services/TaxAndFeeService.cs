using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Services
{
    public class TaxAndFeeService : ITaxAndFeeService
    {
        private readonly PizzaShopContext _context;

        public TaxAndFeeService(PizzaShopContext context)
        {
            _context = context;
        }

        public List<TaxesFee> GetTaxes()
        {
            return _context.TaxesFees.Where(t => t.IsDeleted == false).OrderBy(t => t.Id).ToList();
        }

        public IActionResult SaveChangesOfIsDefault(bool isDefault, int id, int userId)
        {
            TaxesFee tax = _context.TaxesFees.Find(id);
            tax.IsDefault = isDefault;
            tax.UpdatedBy = (short?)userId;
            tax.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return new JsonResult(new { success = true });
        }

        public IActionResult SaveChangesOfIsEnabled(bool isEnabled, int id, int userId)
        {
            TaxesFee tax = _context.TaxesFees.Find(id);
            tax.IsEnabled = isEnabled;
            tax.UpdatedBy = (short?)userId;
            tax.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return new JsonResult(new { success = true });
        }

        public IActionResult Delete(int id, int userId)
        {
            TaxesFee tax = _context.TaxesFees.Find(id);
            tax.IsDeleted = true;
            tax.UpdatedBy = (short?)userId;
            tax.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Tax deleted successfully" });
        }

        public TaxAndFeeViewModel Search(string searchValue)
        {
            List<TaxesFee> taxes = new List<TaxesFee>();
            if (searchValue == "" || searchValue == null)
            {   
                taxes = _context.TaxesFees.Where(t => t.IsDeleted == false).OrderBy(t => t.Id).ToList();
            }
            else
            {
                searchValue = searchValue.ToLower();
                taxes = _context.TaxesFees.Where(t => t.IsDeleted == false && (t.Name.ToLower().Contains(searchValue) || t.TaxType.ToLower().Contains(searchValue) || t.Amount.ToString().Contains(searchValue))).OrderBy(t => t.Id).ToList();
            }
            TaxAndFeeViewModel taxAndFeeViewModel = new TaxAndFeeViewModel
            {
                Taxes = taxes
            };
            return taxAndFeeViewModel;
        }

        public IActionResult AddTax(int taxId, string taxName, bool isEnabled, string taxType, decimal taxAmount, int userId)
        {
            if (taxId == -1)
            {
                if (_context.TaxesFees.Any(t => t.Name == taxName && t.IsDeleted == false))
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
                _context.TaxesFees.Add(tax);
                _context.SaveChanges();
                return new JsonResult(new { success = true, message = "Tax added successfully" });
            } else {
                if (_context.TaxesFees.Any(t => t.Name == taxName && t.Id != taxId && t.IsDeleted == false))
                {
                    return new JsonResult(new { success = false, message = "Tax already exists" });
                }
                TaxesFee tax = _context.TaxesFees.Find(taxId);
                tax.Name = taxName;
                tax.IsEnabled = isEnabled;
                tax.TaxType = taxType;
                tax.Amount = taxAmount;
                tax.UpdatedBy = (short?)userId;
                tax.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return new JsonResult(new { success = true, message = "Tax updated successfully" });
            }
        }

        public IActionResult Edit(int taxId)
        {
            TaxesFee tax = _context.TaxesFees.Find(taxId);
            return new JsonResult(new { tax = tax });
        }
    }
}