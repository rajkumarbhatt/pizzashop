using System.Text.Json.Serialization;

namespace DAL.ViewModels;

public class SectionAndNumberOfWaitingList
{
    [JsonPropertyName("section_id")]
    public int SectionId { get; set; }
    [JsonPropertyName("section_name")]
    public string? SectionName { get; set; }
    [JsonPropertyName("number_of_waiting_list")]
    public int NumberOfWaitingList { get; set; }
}