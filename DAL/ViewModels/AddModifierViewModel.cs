namespace DAL.ViewModels
{
    public class AddModifierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public List<int> ModifierGroupIds { get; set; }
    }
}