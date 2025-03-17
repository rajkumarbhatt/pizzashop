using DAL.Models;
using Section = DAL.Models.Section;

namespace DAL.ViewModels
{
    public class TableAndSectionViewModel
    {
        public List<Section>? Sections { get; set; }
        public List<Table>? Tables { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalTables { get; set; }
        public int TotalPages { get; set; }
    }
}