using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IOrderAppService
    {
        public OrderAppViewModel GetOrderAppViewModel();
        public IActionResult AddToWaitingList(string email, string name, string mobileNumber, string sectionId, string numberOfPeople, int userId);
    }
}