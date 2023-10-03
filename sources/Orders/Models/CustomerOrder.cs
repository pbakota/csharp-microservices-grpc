using System.Text.Json;

namespace Orders.Models;

public record CustomerOrder {
    public string Item  { get; set; } = null!;
    public int Quantity  { get; set; }
    public double Amount { get; set; }
    public string PaymentMethod  { get; set; } = null!;
    public long OrderId  { get; set; }
    public string? Address  { get; set; }
    public override string ToString() => JsonSerializer.Serialize(this);
}