using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Services
{
    public class TableAndSectionService : ITableAndSectionService
    {
        private readonly PizzaShopContext _context;
        public TableAndSectionService(PizzaShopContext context)
        {
            _context = context;
        }
        public List<Section> GetSections()
        {
            return _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToList();
        }

        public List<Table> GetTables()
        {
            return _context.Tables.Where(t => t.IsDeleted == false).OrderBy(t => t.Id).ToList();
        }

        public List<Table> GetTablesBySectionId(int sectionId)
        {
            return _context.Tables.Where(t => t.SectionId == sectionId && t.IsDeleted == false).OrderBy(t => t.Id).ToList();
        }

        public TableAndSectionViewModel TablesFilter(int sectionId, string searchValue, int pageIndex, int pageSize)
        {
            searchValue = searchValue?.ToLower() ?? "";
            List<Table> tables = _context.Tables.Where(t => t.SectionId == sectionId && t.IsDeleted == false).OrderBy(t => t.Id).ToList();
            if (!string.IsNullOrEmpty(searchValue))
            {
                tables = tables.Where(t => (t.Name != null && t.Name.ToLower().Contains(searchValue)) || (t.Status != null && t.Status.ToLower().Contains(searchValue)) || (t.Capacity.ToString().Contains(searchValue))).ToList();
            }
            TableAndSectionViewModel tableAndSectionViewModel = new TableAndSectionViewModel
            {
                Sections = _context.Sections.ToList(),
                Tables = tables.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageSize = pageSize,
                PageIndex = pageIndex,
                TotalTables = tables.Count,
                TotalPages = (int)Math.Ceiling((double)tables.Count / pageSize)
            };
            return tableAndSectionViewModel;
        }

        public IActionResult AddSection(string sectionName, string sectionDescription, int sectionId, int userId)
        {
            if (sectionId == -1)
            {
                if (_context.Sections.Any(s => s.Name == sectionName))
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
                _context.Sections.Add(section);
                _context.SaveChanges();
                return new JsonResult(new { success = true, message = "Section added successfully" });
            }
            else
            {
                Section section = _context.Sections.FirstOrDefault(s => s.Id == sectionId);
                section.Name = sectionName;
                section.Description = sectionDescription;
                section.UpdatedBy = userId;
                _context.SaveChanges();
                return new JsonResult(new { success = true, message = "Section updated successfully" });
            }
        }

        public Section GetSectionById(int sectionId)
        {
            Section section = _context.Sections.FirstOrDefault(s => s.Id == sectionId && s.IsDeleted == false) ?? new Section();
            return section;
        }

        public IActionResult DeleteSection(int sectionId, int userId)
        {
            Section section = _context.Sections.FirstOrDefault(s => s.Id == sectionId) ?? new Section();
            if (_context.Tables.Any(t => t.SectionId == sectionId && t.IsDeleted == false && (t.Status == "Occupied" || t.Status == "Running" || t.Status == "Assigned")))
            {
                return new JsonResult(new { success = false, message = "Section contains occupied table(s)" });
            }
            section.IsDeleted = true;
            section.UpdatedBy = userId;
            _context.SaveChanges();
            List<Table> tables = _context.Tables.Where(t => t.SectionId == sectionId).ToList();
            foreach (Table table in tables)
            {
                table.IsDeleted = true;
                table.UpdatedBy = userId;
                _context.SaveChanges();
            }
            return new JsonResult(new { success = true, message = "Section deleted successfully" });
        }

        public IActionResult DeleteTables (List<int> tableIds, int userId)
        {
            if (tableIds.Count == 0)
            {
                return new JsonResult(new { success = false, message = "No tables selected" });
            }
            foreach (var tableId in tableIds)
            {
                Table table = _context.Tables.FirstOrDefault(t => t.Id == tableId);
                if (table.Status == "Occupied" || table.Status == "Running" || table.Status == "Assigned")
                {
                    return new JsonResult(new { success = false, message = "Table is occupied" });
                }
            }
            List<Table> tables = _context.Tables.Where(t => tableIds.Contains(t.Id)).ToList();
            foreach (Table table in tables)
            {
                table.IsDeleted = true;
                table.UpdatedBy = userId;
            }
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Tables deleted successfully" });
        }

        public IActionResult DeleteTable(int tableId, int userId)
        {
            Table table = _context.Tables.FirstOrDefault(t => t.Id == tableId) ?? new Table();
            if (table.Status == "Occupied" || table.Status == "Running" || table.Status == "Assigned")
            {
                return new JsonResult(new { success = false, message = "Table is occupied" });
            }
            table.IsDeleted = true;
            table.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Table deleted successfully" });
        }

        public IActionResult AddTable(int tableId, string tableName, string tableStatus, int tableCapacity, int sectionId, int userId)
        {
            if (tableId == -1)
            {
                Table table = new Table
                {
                    Name = tableName,
                    Status = tableStatus,
                    Capacity = tableCapacity,
                    SectionId = sectionId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                _context.Tables.Add(table);
                _context.SaveChanges();
                return new JsonResult(new { success = true, message = "Table added successfully" });
            }
            else
            {
                Table table = _context.Tables.FirstOrDefault(t => t.Id == tableId);
                table.Name = tableName;
                table.Status = tableStatus;
                table.Capacity = tableCapacity;
                table.SectionId = sectionId;
                table.UpdatedBy = userId;
                _context.SaveChanges();
                return new JsonResult(new { success = true, message = "Table updated successfully" });
            }
        }

        public Table GetTableById(int tableId)
        {
            Table table = _context.Tables.FirstOrDefault(t => t.Id == tableId && t.IsDeleted == false) ?? new Table();
            return table;
        }
    }
}