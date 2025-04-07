using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;

namespace BLL.Interfaces
{
    public interface IKotMenuService
    {
        public KotMenuViewModel GetKotMenu(int? orderId);
        public KotMenuViewModel GetKotMenuItemsBasedOnCategory(int categoryId);
        public KotMenuViewModel SearchMenuItemsKot(string search, int categoryId);
        public JsonResult AddToFavourites (int itemId, int userId);
        public JsonResult DeleteFromFavourites (int itemId, int userId);
        public KotMenuViewModel GetCustomerDetails(int orderId);
        public JsonResult UpdateCustomerDetails (WaitingListModal waitingListModal, int userId);
        public KotMenuViewModel GetSelectModifiersModalData(int itemId);
        public IActionResult AddItemToOrder (int itemId, int orderId, List<int> modifierIds, int userId);
    }
}