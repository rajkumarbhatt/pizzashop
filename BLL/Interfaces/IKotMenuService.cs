using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;

namespace BLL.Interfaces
{
    public interface IKotMenuService
    {
        public KotMenuViewModel GetKotMenu();
        public KotMenuViewModel GetKotMenuItemsBasedOnCategory(int categoryId);
        public KotMenuViewModel SearchMenuItemsKot(string search, int categoryId);
        public JsonResult AddToFavourites (int itemId, int userId);
        public JsonResult DeleteFromFavourites (int itemId, int userId);
        public KotMenuViewModel GetCustomerDetails(int orderId);
        public JsonResult UpdateCustomerDetails (WaitingListModal waitingListModal, int userId);
    }
}