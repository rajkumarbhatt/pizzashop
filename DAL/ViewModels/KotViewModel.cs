using DAL.Models;

namespace DAL.ViewModels;

public class KotViewModel
{
    public List<Category>? Categories;
    public List<KotOrderCard>? KotOrderCards;
    public int PageSize;
    public int PageIndex;
    public int TotalPages;
}