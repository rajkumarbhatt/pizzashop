using DAL.ViewModels;

namespace BLL.Interfaces
{
    public interface IKotMenuService
    {
        public KotMenuViewModel GetKotMenu();
        public KotMenuViewModel GetKotMenuItemsBasedOnCategory(int categoryId);
        public KotMenuViewModel SearchMenuItemsKot(string search, int categoryId);
    }
}