using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ITableAndSectionService
    {
        public List<Section> GetSections();
        public List<Table> GetTables();
        public List<Table> GetTablesBySectionId(int sectionId);
        public TableAndSectionViewModel TablesFilter(int sectionId, string searchValue, int pageIndex, int pageSize);
        public IActionResult AddSection(string sectionName, string sectionDescription, int sectionId, int userId);
        public Section GetSectionById(int sectionId);
        public IActionResult DeleteSection(int sectionId, int userId);
        public IActionResult DeleteTables(List<int> tableIds, int userId);
        public IActionResult DeleteTable(int tableId, int userId);
        public IActionResult AddTable(int tableId, string tableName, string tableStatus, int tableCapacity, int sectionId, int userId);
        public Table GetTableById(int tableId);
    }
}