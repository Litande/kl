namespace KL.MySql.Entities;

public class Exchange
{
    public ushort Id { get; set; }
    public string Name { get; set; }
    public string? TradingHours { get; set; }
}