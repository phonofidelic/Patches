
using System.ComponentModel.DataAnnotations.Schema;
using Patches.Domain.ValueObjects;

namespace Patches.Domain.Entities;

public class Connection  
{
    public int PatchId { get; set; }
    public int InputId { get; set; }
    public int OutputId { get; set; }
    public Patch Patch { get; set; } = default!;
    public ConnectionPoint Input { get; set; } = default!;
    public ConnectionPoint Output { get; set; } = default!;
    
    [NotMapped]
    public SignalType SignalType { get; set; } = default!;
}
