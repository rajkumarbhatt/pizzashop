using DAL.Models;

namespace DAL.ViewModels
{
    public class OrderViewModal
    {
        public List<OrderTable> Orders { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int totalOrders { get; set; }
        public int totalPages { get; set; }
    }
}