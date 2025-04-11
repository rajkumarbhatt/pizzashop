using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class TableAndSectionService : ITableAndSectionService
    {
        private readonly PizzaShopContext _context;
        public TableAndSectionService(PizzaShopContext context)
        {
            _context = context;
        }
        public async Task<List<Section>> GetSectionsAsync()
        {
            return await _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToListAsync();
        }

        public async Task<List<Table>> GetTablesAsync()
        {
            return await _context.Tables.Where(t => t.IsDeleted == false).OrderBy(t => t.Id).ToListAsync();
        }

        public async Task<List<Table>> GetTablesBySectionIdAsync(int sectionId)
        {
            return await _context.Tables.Where(t => t.SectionId == sectionId && t.IsDeleted == false).OrderBy(t => t.Id).ToListAsync();
        }

        public async Task<TableAndSectionViewModel> TablesFilterAsync(int sectionId, string searchValue, int pageIndex, int pageSize)
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

        public async Task<IActionResult> AddSectionAsync(string sectionName, string sectionDescription, int sectionId, int userId)
        {
            if (sectionId == -1)
            {
            if (await _context.Sections.AnyAsync(s => s.Name == sectionName))
            {
                return new JsonResult(new { success = false, message = "Section already exists" });
            }
            Section section = new Section
            {
                Name = sectionName,
                Description = sectionDescription,
                CreatedBy = userId,
                UpdatedBy = userId
            };
            await _context.Sections.AddAsync(section);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Section added successfully" });
            }
            else
            {
            Section section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId);
            section.Name = sectionName;
            section.Description = sectionDescription;
            section.UpdatedBy = userId;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Section updated successfully" });
            }
        }

        public async Task<Section> GetSectionByIdAsync(int sectionId)
        {
            Section section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId && s.IsDeleted == false) ?? new Section();
            return section;
        }

        public async Task<IActionResult> DeleteSectionAsync(int sectionId, int userId)
        {
            Section section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId) ?? new Section();
            if (await _context.Tables.AnyAsync(t => t.SectionId == sectionId && t.IsDeleted == false && (t.Status == "Occupied" || t.Status == "Running" || t.Status == "Assigned")))
            {
            return new JsonResult(new { success = false, message = "Section contains occupied table(s)" });
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
            return new JsonResult(new { success = true, message = "Section deleted successfully" });
        }

        public async Task<IActionResult> DeleteTablesAsync(List<int> tableIds, int userId)
        {
            if (tableIds.Count == 0)
            {
            return new JsonResult(new { success = false, message = "No tables selected" });
            }
            foreach (var tableId in tableIds)
            {
            Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId);
            if (table.Status == "Occupied" || table.Status == "Running" || table.Status == "Assigned")
            {
                return new JsonResult(new { success = false, message = "Table is occupied" });
            }
            }
            List<Table> tables = await _context.Tables.Where(t => tableIds.Contains(t.Id)).ToListAsync();
            foreach (Table table in tables)
            {
            table.IsDeleted = true;
            table.UpdatedBy = userId;
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Tables deleted successfully" });
        }

        public async Task<IActionResult> DeleteTableAsync(int tableId, int userId)
        {
            Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId) ?? new Table();
            if (table.Status == "Occupied" || table.Status == "Running" || table.Status == "Assigned")
            {
            return new JsonResult(new { success = false, message = "Table is occupied" });
            }
            table.IsDeleted = true;
            table.UpdatedBy = userId;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Table deleted successfully" });
        }

        public async Task<IActionResult> AddTableAsync(int tableId, string tableName, string tableStatus, int tableCapacity, int sectionId, int userId)
        {
            if (tableId == -1)
            {
            if (await _context.Tables.AnyAsync(t => t.Name == tableName && t.SectionId == sectionId && t.IsDeleted == false))
            {
                return new JsonResult(new { success = false, message = "Table already exists" });
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
            return new JsonResult(new { success = true, message = "Table added successfully" });
            }
            else
            {
            Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId);
            if (await _context.Tables.AnyAsync(t => t.Name == tableName && t.SectionId == sectionId && t.IsDeleted == false && t.Id != tableId))
            {
                return new JsonResult(new { success = false, message = "Table already exists" });
            }
            table.Name = tableName;
            table.Status = tableStatus;
            table.Capacity = tableCapacity;
            table.SectionId = sectionId;
            table.UpdatedBy = userId;
            table.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Table updated successfully" });
            }
        }

        public async Task<Table> GetTableByIdAsync(int tableId)
        {
            Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId && t.IsDeleted == false) ?? new Table();
            return table;
        }

        public async Task<TableAndSectionViewModel> GetTableAndSectionViewModelAsync()
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
    }
}