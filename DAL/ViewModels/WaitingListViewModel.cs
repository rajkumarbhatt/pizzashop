using DAL.Models;

namespace DAL.ViewModels;

public class WaitingListViewModel
{
    public List<SectionAndNumberOfWaitingList>? SectionAndNumberOfWaitingLists { get; set; }
    public List<WaitingListTable>? WaitingList { get; set; }
    public List<Section>? Sections { get; set; }
    public WaitingListModal? waitingListModal { get; set; }
}