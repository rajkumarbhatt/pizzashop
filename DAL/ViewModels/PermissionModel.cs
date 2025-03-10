namespace DAL.ViewModels
{
    public class PermissionModel
    {
        public int PermissionId { get; set; }
        public string? Name { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}