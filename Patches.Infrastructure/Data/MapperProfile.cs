using AutoMapper;
using Patches.Domain.Entities;
using Patches.Shared.Commands;

namespace Patches.Infrastructure.Data;

public class MapperProfile: Profile
{
    public MapperProfile()
    {
        CreateMap<AddModuleCommand, Module>();
        CreateMap<Module, AddModuleResult>();
    }
}
