using System.ComponentModel.DataAnnotations.Schema;
using Patches.Domain.ValueObjects;

namespace Patches.Domain.Entities;

public class ConnectionPoint: BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    
    [NotMapped]
    public required ConnectionPointType Type { get; set; }
}
