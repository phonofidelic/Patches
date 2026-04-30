using System;

namespace Patches.Shared.Dtos;

public class ConnectionPointDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ModuleId { get; set; }
    public string Type { get; set; } = string.Empty;
}
