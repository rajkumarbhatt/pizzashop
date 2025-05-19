using System.Text.Json.Serialization;

public class MarkAsReadyModal
{
    [JsonPropertyName("order_item_id")]
    public int OrderItemId { get; set; }
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
