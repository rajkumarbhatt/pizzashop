using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentaion.Controllers
{
    [CustomAuth]
    public class TableAndSection : Controller
    {
        private readonly ITableAndSectionService _tableAndSectionService;
        private readonly IJwtService _jwtService;
        public TableAndSection(ITableAndSectionService tableAndSectionService, IJwtService jwtService)
        {
            _tableAndSectionService = tableAndSectionService;
            _jwtService = jwtService;
        }
        public async Task<IActionResult> Index()
        {
            TableAndSectionViewModel tableAndSectionViewModel = await _tableAndSectionService.GetTableAndSectionViewModelAsync();
            return View(tableAndSectionViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> TablesFilter(int pageIndex, int pageSize, int sectionId, string searchValue = null)
        {
            TableAndSectionViewModel tableAndSectionViewModel = await _tableAndSectionService.TablesFilterAsync(sectionId, searchValue, pageIndex, pageSize);
            return PartialView("_TablePartial", tableAndSectionViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSection(string sectionName, string sectionDescription, int sectionId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _tableAndSectionService.AddSectionAsync(sectionName, sectionDescription, sectionId, userId);
        }

        [HttpGet]
        public async Task<IActionResult> EditSection(int sectionId)
        {
            Section section = await _tableAndSectionService.GetSectionByIdAsync(sectionId);
            return new JsonResult(new { section.Name, section.Description });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _tableAndSectionService.DeleteSectionAsync(sectionId, userId);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTables(List<int> tableIds)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _tableAndSectionService.DeleteTablesAsync(tableIds, userId);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTable(int tableId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _tableAndSectionService.DeleteTableAsync(tableId, userId);
        }

        [HttpPost]
        public async Task<IActionResult> AddTable(AddTableViewModal addTableViewModal)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _tableAndSectionService.AddTableAsync(addTableViewModal, userId);
        }

        [HttpGet]
        public async Task<IActionResult> EditTable(int tableId)
        {
            Table table = await _tableAndSectionService.GetTableByIdAsync(tableId);
            return new JsonResult(new { table.Id, table.Name, table.Status, table.Capacity, table.SectionId });
        }
        [HttpGet]
        public async Task<IActionResult> SectionsFilter(int pageIndex, int pageSize, int sectionId)
        {
            TableAndSectionViewModel tableAndSectionViewModel = await _tableAndSectionService.SectionsFilterAsync(sectionId, pageIndex, pageSize);
            return PartialView("_SectionsPartial", tableAndSectionViewModel);
        }
    }
}