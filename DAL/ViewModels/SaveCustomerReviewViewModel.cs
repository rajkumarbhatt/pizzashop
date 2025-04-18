namespace DAL.ViewModels
{
    public class SaveCustomerReviewViewModel
    {
        public int FoodRating { get; set; }
        public int ServiceRating { get; set; }
        public int AmbienceRating { get; set; }
        public string? OrderReviewByCustomer { get; set; }
        public int OrderId { get; set; }
    }
}