namespace DAL.ViewModels
{
    public class DashboardViewModel
    {
        public double TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public double AverageOrderValue { get; set; }
        public double AverageWaitingTime { get; set; }
        public List<DashboardItems>? TopSellingItems { get; set; }
        public List<DashboardItems>? LeastSellingItems { get; set; }
        public int WaitingListCount { get; set; }
        public int NewCustomerCount { get; set; }
        public RevenueData? RevenueData { get; set; }
        public RevenueData? CustomerGrowthData { get; set; }
    }
}