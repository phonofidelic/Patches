namespace Patches.Domain.Entities;

public class Module: BaseEntity<Guid>
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public int HorizontalPitch { get; set; }
    public int VerticalUnits { get; set; }
    public Guid? VendorId { get; set; } 
    public Vendor? Vendor { get; set; }
    public ICollection<ConnectionPoint> ConnectionPoints { get; set; } = [];
}
