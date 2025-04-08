using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ITableAndSectionService
    {
        Task<List<Section>> GetSectionsAsync();
        Task<List<Table>> GetTablesAsync();
        Task<List<Table>> GetTablesBySectionIdAsync(int sectionId);
        Task<TableAndSectionViewModel> TablesFilterAsync(int sectionId, string searchValue, int pageIndex, int pageSize);
        Task<IActionResult> AddSectionAsync(string sectionName, string sectionDescription, int sectionId, int userId);
        Task<Section> GetSectionByIdAsync(int sectionId);
        Task<IActionResult> DeleteSectionAsync(int sectionId, int userId);
        Task<IActionResult> DeleteTablesAsync(List<int> tableIds, int userId);
        Task<IActionResult> DeleteTableAsync(int tableId, int userId);
        Task<IActionResult> AddTableAsync(int tableId, string tableName, string tableStatus, int tableCapacity, int sectionId, int userId);
        Task<Table> GetTableByIdAsync(int tableId);
        Task<TableAndSectionViewModel> GetTableAndSectionViewModelAsync();
    }
}