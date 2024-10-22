namespace KL.MySql.Entities;

public class Category
{
    public ushort Id { get; set; }
    public string Name { get; set; }
    public ushort ThresholdPercent { get; set; }
    public ushort BufferSize { get; set; }
}