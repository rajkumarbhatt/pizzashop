using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class TaxAndFeeService : ITaxAndFeeService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<TaxAndFeeService> _logger;
        public TaxAndFeeService(PizzaShopContext context, ILogger<TaxAndFeeService> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<List<TaxesFee>> GetTaxesAsync()
        {
            try
            {
                return await _context.TaxesFees.Where(t => t.IsDeleted == false).OrderBy(t => t.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving taxes");
                Console.WriteLine(ex.Message);
                return new List<TaxesFee>();
            }
        }
        public async Task<IActionResult> SaveChangesOfIsDefaultAsync(bool isDefault, int id, int userId)
        {
            try
            {
                TaxesFee tax = await _context.TaxesFees.FindAsync(id);
                tax.IsDefault = isDefault;
                tax.UpdatedBy = (short?)userId;
                tax.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new JsonResult(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes");
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while saving changes"
                });
            }
        }
        public async Task<IActionResult> SaveChangesOfIsEnabledAsync(bool isEnabled, int id, int userId)
        {
            try
            {
                TaxesFee tax = await _context.TaxesFees.FindAsync(id);
                tax.IsEnabled = isEnabled;
                tax.UpdatedBy = (short?)userId;
                tax.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new JsonResult(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes");
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while saving changes"
                });
            }
        }
        public async Task<IActionResult> DeleteAsync(int id, int userId)
        {
            try
            {
                TaxesFee tax = await _context.TaxesFees.FindAsync(id);
                tax.IsDeleted = true;
                tax.UpdatedBy = (short?)userId;
                tax.UpdatedAt = DateTime.Now;
                _context.TaxesFees.Update(tax);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tax with ID {Id} deleted successfully by user ID {UserId}", id, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Tax deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting tax with ID {Id}", id);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the tax"
                });
            }
        }
        public async Task<TaxAndFeeViewModel> SearchAsync(string searchValue)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for taxes");
                Console.WriteLine(ex.Message);
                return new TaxAndFeeViewModel
                {
                    Taxes = new List<TaxesFee>()
                };
            }

        }
        public async Task<IActionResult> AddTaxAsync(AddTaxViewModal addTaxViewModal, int userId)
        {
            try
            {
                string taxName = addTaxViewModal.TaxName ?? string.Empty;
                string taxType = addTaxViewModal.TaxType ?? string.Empty;
                decimal taxAmount = addTaxViewModal.TaxAmount ?? 0;
                bool isEnabled = addTaxViewModal.IsEnabled;
                int taxId = addTaxViewModal.TaxId;
                if (taxId == -1)
                {
                    if (await _context.TaxesFees.AnyAsync(t => t.Name.ToLower() == taxName.ToLower() && t.IsDeleted == false))
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Tax already exists"
                        });
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
                    _logger.LogInformation("Tax {TaxName} added successfully by user ID {UserId}", taxName, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Tax added successfully"
                    });
                }
                else
                {
                    if (await _context.TaxesFees.AnyAsync(t => t.Name.ToLower() == taxName.ToLower() && t.Id != taxId && t.IsDeleted == false))
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Tax already exists"
                        });
                    }
                    TaxesFee tax = await _context.TaxesFees.FindAsync(taxId);
                    tax.Name = taxName;
                    tax.IsEnabled = isEnabled;
                    tax.TaxType = taxType;
                    tax.Amount = taxAmount;
                    tax.UpdatedBy = (short?)userId;
                    tax.UpdatedAt = DateTime.Now;
                    _context.TaxesFees.Update(tax);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Tax {TaxName} updated successfully by user ID {UserId}", taxName, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Tax updated successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding/updating tax");
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while adding/updating the tax"
                });
            }
        }
        public async Task<IActionResult> EditAsync(int taxId)
        {
            try
            {
                TaxesFee tax = await _context.TaxesFees.FindAsync(taxId);
                return new JsonResult(new
                {
                    tax = tax
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing tax with ID {Id}", taxId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while editing the tax"
                });
            }
        }
        public async Task<TaxAndFeeViewModel> GetTaxAndFeeViewModelAsync()
        {
            try
            {
                List<TaxesFee> taxes = await GetTaxesAsync();
                TaxAndFeeViewModel taxAndFeeViewModel = new TaxAndFeeViewModel
                {
                    Taxes = taxes
                };
                return taxAndFeeViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the tax and fee view model");
                Console.WriteLine(ex.Message);
                return new TaxAndFeeViewModel
                {
                    Taxes = new List<TaxesFee>()
                };
            }
        }
    }
}