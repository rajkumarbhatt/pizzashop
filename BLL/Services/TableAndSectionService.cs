using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class TableAndSectionService : ITableAndSectionService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<TableAndSectionService> _logger;
        public TableAndSectionService(PizzaShopContext context, ILogger<TableAndSectionService> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<List<Section>> GetSectionsAsync()
        {
            try
            {
                return await _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching sections.");
                return new List<Section>();
            }
        }

        public async Task<List<Table>> GetTablesAsync()
        {
            try
            {
                return await _context.Tables.Where(t => t.IsDeleted == false).OrderBy(t => t.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching tables.");
                return new List<Table>();
            }
        }

        public async Task<List<Table>> GetTablesBySectionIdAsync(int sectionId)
        {
            try
            {
                return await _context.Tables.Where(t => t.SectionId == sectionId && t.IsDeleted == false).OrderBy(t => t.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching tables for sectionId {sectionId}.");
                return new List<Table>();
            }
        }

        public async Task<TableAndSectionViewModel> TablesFilterAsync(int sectionId, string searchValue, int pageIndex, int pageSize)
        {
            try
            {
                searchValue = searchValue?.ToLower() ?? "";
                List<Table> tables = await _context.Tables
                    .Where(t => t.SectionId == sectionId && t.IsDeleted == false)
                    .OrderBy(t => t.Id)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    tables = tables.Where(t =>
                            (t.Name != null && t.Name.ToLower().Contains(searchValue)) ||
                            (t.Status != null && t.Status.ToLower().Contains(searchValue)) ||
                            t.Capacity.ToString().Contains(searchValue))
                        .ToList();
                }
                int totalPages = (int)Math.Ceiling((double)tables.Count / pageSize);
                if (pageIndex > totalPages)
                {
                    pageIndex = totalPages;
                }
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                TableAndSectionViewModel tableAndSectionViewModel = new TableAndSectionViewModel
                {
                    Sections = await _context.Sections.ToListAsync(),
                    Tables = tables.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalTables = tables.Count,
                    TotalPages = (int)Math.Ceiling((double)tables.Count / pageSize)
                };
                return tableAndSectionViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while filtering tables.");
                Console.WriteLine(ex.Message);
                return new TableAndSectionViewModel
                {
                    Sections = new List<Section>(),
                    Tables = new List<Table>(),
                    PageSize = 5,
                    PageIndex = 1,
                    TotalTables = 0,
                    TotalPages = 0
                };
            }
        }

        public async Task<IActionResult> AddSectionAsync(string sectionName, string sectionDescription, int sectionId, int userId)
        {
            try
            {
                if (await _context.Sections.AnyAsync(s => s.Name.ToLower() == sectionName.ToLower() && s.IsDeleted == false && s.Id != sectionId))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Section already exists"
                    });
                }
                if (sectionId == -1)
                {
                    Section section = new Section
                    {
                        Name = sectionName,
                        Description = sectionDescription,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };
                    await _context.Sections.AddAsync(section);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Section {sectionName} added successfully by user {UserId}", sectionName, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Section added successfully"
                    });
                }
                else
                {
                    Section section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId);
                    section.Name = sectionName;
                    section.Description = sectionDescription;
                    section.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Section {sectionName} updated successfully by user {UserId}", sectionName, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Section updated successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding/updating the section.");
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while adding/updating the section",
                    error = ex.Message
                });
            }
        }

        public async Task<Section> GetSectionByIdAsync(int sectionId)
        {
            try
            {
                Section section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId && s.IsDeleted == false) ?? new Section();
                return section;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching section with ID {SectionId}.", sectionId);
                Console.WriteLine(ex.Message);
                return new Section();
            }
        }

        public async Task<IActionResult> DeleteSectionAsync(int sectionId, int userId)
        {
            try
            {
                Section section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId) ?? new Section();
                if (await _context.Tables.AnyAsync(t => t.SectionId == sectionId && t.IsDeleted == false && (t.Status == "Occupied" || t.Status == "Running" || t.Status == "Assigned")))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Section contains occupied table(s)"
                    });
                }
                section.IsDeleted = true;
                section.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                List<Table> tables = await _context.Tables.Where(t => t.SectionId == sectionId).ToListAsync();
                foreach (Table table in tables)
                {
                    table.IsDeleted = true;
                    table.UpdatedBy = userId;
                    table.UpdatedAt = DateTime.Now;
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Section {SectionId} deleted successfully by user {UserId}", sectionId, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Section deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting section with ID {SectionId}.", sectionId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the section",
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> DeleteTablesAsync(List<int> tableIds, int userId)
        {
            try
            {
                if (tableIds.Count == 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "No tables selected"
                    });
                }
                foreach (var tableId in tableIds)
                {
                    Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId);
                    if (table.Status == "Occupied" || table.Status == "Running" || table.Status == "Assigned")
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Table is occupied"
                        });
                    }
                }
                List<Table> tables = await _context.Tables.Where(t => tableIds.Contains(t.Id)).ToListAsync();
                foreach (Table table in tables)
                {
                    table.IsDeleted = true;
                    table.UpdatedBy = userId;
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tables deleted successfully by user {UserId}", userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Tables deleted successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.LogError(ex, "An error occurred while deleting tables.");
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the tables",
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> DeleteTableAsync(int tableId, int userId)
        {
            try
            {
                Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId) ?? new Table();
                if (table.Status == "Occupied" || table.Status == "Running" || table.Status == "Assigned")
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Table is occupied"
                    });
                }
                table.IsDeleted = true;
                table.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Table {TableId} deleted successfully by user {UserId}", tableId, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Table deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting table with ID {TableId}.", tableId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the table",
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> AddTableAsync(AddTableViewModal addTableViewModal, int userId)
        {
            try
            {
                string tableName = addTableViewModal.TableName;
                string tableStatus = addTableViewModal.TableStatus;
                int tableCapacity = addTableViewModal.TableCapacity;
                int sectionId = addTableViewModal.SectionId;
                int tableId = addTableViewModal.TableId;
                if (tableId == -1)
                {
                    if (await _context.Tables.AnyAsync(t => t.Name.ToLower() == tableName.ToLower() && t.SectionId == sectionId && t.IsDeleted == false))
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Table already exists"
                        });
                    }
                    Table table = new Table
                    {
                        Name = tableName,
                        Status = tableStatus,
                        Capacity = tableCapacity,
                        SectionId = sectionId,
                        CreatedBy = userId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = userId
                    };
                    await _context.Tables.AddAsync(table);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Table {TableName} added successfully by user {UserId}", tableName, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Table added successfully"
                    });
                }
                else
                {
                    if (addTableViewModal.TableStatus != "Available")
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Cannot edit an occupied table"
                        });
                    }
                    Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId);
                    if (await _context.Tables.AnyAsync(t => t.Name.ToLower() == tableName.ToLower() && t.SectionId == sectionId && t.IsDeleted == false && t.Id != tableId))
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Table already exists"
                        });
                    }
                    table.Name = tableName;
                    table.Status = tableStatus;
                    table.Capacity = tableCapacity;
                    table.SectionId = sectionId;
                    table.UpdatedBy = userId;
                    table.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Table {TableName} updated successfully by user {UserId}", tableName, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Table updated successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding/updating the table.");
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while adding/updating the table",
                    error = ex.Message
                });
            }
        }

        public async Task<Table> GetTableByIdAsync(int tableId)
        {
            try
            {
                Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId && t.IsDeleted == false) ?? new Table();
                return table;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching table with ID {TableId}.", tableId);
                Console.WriteLine(ex.Message);
                return new Table();
            }
        }

        public async Task<TableAndSectionViewModel> GetTableAndSectionViewModelAsync()
        {
            try
            {
                List<Section> sections = await GetSectionsAsync();
                if (sections.Count == 0)
                {
                    return new TableAndSectionViewModel
                    {
                        Sections = new List<Section>(),
                        Tables = new List<Table>(),
                        PageSize = 5,
                        PageIndex = 1,
                        TotalTables = 0,
                        TotalPages = 0
                    };
                }

                List<Table> tables = await GetTablesBySectionIdAsync(sections[0].Id);
                TableAndSectionViewModel tableAndSectionViewModel = new TableAndSectionViewModel
                {
                    Sections = sections,
                    Tables = tables.Skip(0).Take(5).ToList(),
                    PageSize = 5,
                    PageIndex = 1,
                    TotalTables = tables.Count,
                    TotalPages = (int)Math.Ceiling((double)tables.Count / 5)
                };
                return tableAndSectionViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching table and section view model.");
                Console.WriteLine(ex.Message);
                return new TableAndSectionViewModel
                {
                    Sections = new List<Section>(),
                    Tables = new List<Table>(),
                    PageSize = 5,
                    PageIndex = 1,
                    TotalTables = 0,
                    TotalPages = 0
                };
            }
        }
        public async Task<TableAndSectionViewModel> SectionsFilterAsync(int sectionId, int pageIndex, int pageSize)
        {
            try
            {
                if (sectionId == -1)
                {
                    sectionId = await _context.Sections.Where(s => s.IsDeleted == false).Select(s => s.Id).FirstOrDefaultAsync();
                }
                List<Table> tables = await _context.Tables.Where(t => t.SectionId == sectionId && t.IsDeleted == false).OrderBy(t => t.Id).ToListAsync();
                TableAndSectionViewModel tableAndSectionViewModel = new TableAndSectionViewModel
                {
                    Sections = await _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToListAsync(),
                    Tables = tables.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalTables = tables.Count,
                    TotalPages = (int)Math.Ceiling((double)tables.Count / pageSize)
                };
                return tableAndSectionViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while filtering sections.");
                Console.WriteLine(ex.Message);
                return new TableAndSectionViewModel
                {
                    Sections = new List<Section>(),
                    Tables = new List<Table>(),
                    PageSize = 5,
                    PageIndex = 1,
                    TotalTables = 0,
                    TotalPages = 0
                };
            }
        }
    }
}