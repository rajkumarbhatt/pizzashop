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
        public IActionResult Index()
        {
            List<Section> sections = _tableAndSectionService.GetSections();
            List<Table> tables = _tableAndSectionService.GetTablesBySectionId(sections[0].Id);
            TableAndSectionViewModel tableAndSectionViewModel = new TableAndSectionViewModel
            {
                Sections = sections,
                Tables = tables.Skip(0).Take(5).ToList(),
                PageSize = 5,
                PageIndex = 1,
                TotalTables = tables.Count,
                TotalPages = (int)Math.Ceiling((double)tables.Count / 5)
            };
            return View(tableAndSectionViewModel);
        }

        [HttpGet]
        public IActionResult TablesFilter(int pageIndex, int pageSize, int sectionId, string searchValue = null)
        {
            TableAndSectionViewModel tableAndSectionViewModel = _tableAndSectionService.TablesFilter(sectionId, searchValue, pageIndex, pageSize);
            return PartialView("_TablePartial", tableAndSectionViewModel);
        }

        [HttpPost]
        public IActionResult AddSection(string sectionName, string sectionDescription, int sectionId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _tableAndSectionService.AddSection(sectionName, sectionDescription, sectionId, userId);
        }

        [HttpGet]
        public IActionResult EditSection(int sectionId)
        {
            Section section = _tableAndSectionService.GetSectionById(sectionId);
            return new JsonResult(new { section.Name, section.Description });
        }

        [HttpDelete]
        public IActionResult DeleteSection(int sectionId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _tableAndSectionService.DeleteSection(sectionId, userId);
        }

        [HttpDelete]
        public IActionResult DeleteTables(List<int> tableIds)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _tableAndSectionService.DeleteTables(tableIds, userId);
        }

        [HttpDelete]
        public IActionResult DeleteTable(int tableId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _tableAndSectionService.DeleteTable(tableId, userId);
        }

        [HttpPost]
        public IActionResult AddTable(int tableId, string tableName, string tableStatus, int tableCapacity, int sectionId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _tableAndSectionService.AddTable(tableId, tableName, tableStatus, tableCapacity, sectionId, userId);
        }

        [HttpGet]
        public IActionResult EditTable(int tableId)
        {
            Table table = _tableAndSectionService.GetTableById(tableId);
            return new JsonResult(new { table.Id, table.Name, table.Status, table.Capacity, table.SectionId });
        }
    }
}