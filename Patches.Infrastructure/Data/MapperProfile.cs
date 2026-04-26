using AutoMapper;
using Patches.Domain.Entities;
using Patches.Shared.Commands;
using Patches.Shared.Queries;

namespace Patches.Infrastructure.Data;

public class MapperProfile: Profile
{
    public MapperProfile()
    {
        CreateMap<AddModuleCommand, Module>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.VendorId, opt => opt.Ignore())
            .ForMember(dest => dest.Vendor, opt => opt.Ignore())
            .ForMember(dest => dest.ConnectionPoints, opt => opt.Ignore());
        CreateMap<ConnectionPoint, ModuleConnectionPoint>();
        CreateMap<Module, AddModuleResult>();
        CreateMap<Module, ModuleListItem>();
        CreateMap<Module, PatchMatrixItemDto>();
    }
}
