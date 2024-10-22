using KL.MySql.Entities.Enums;

namespace KL.MySql.Entities;

public class Instrument
{
    public uint Id { get; set; }
    
    public string Name { get; set; }
    public string ProviderSymbol { get; set; }
    public string UnixSymbol { get; set; }
    
    public InstrumentStatus Status { get; set; }
    
    public ushort FeedId { get; set; }
    
    public ushort? CategoryId { get; set; }
    
    public ushort? ExchangeId { get; set; }
    
    public ushort QuotePrecision { get; set; }
    
    public ushort TickTimeout { get; set; }
    
    
}