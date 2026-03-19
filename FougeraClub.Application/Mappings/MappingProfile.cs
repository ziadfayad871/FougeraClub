using AutoMapper;
using FougeraClub.Application.DTOs;
using FougeraClub.Domain.Entities;

namespace FougeraClub.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));
            
            CreateMap<PurchaseOrderDto, PurchaseOrder>()
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>().ReverseMap()
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore());
        }
    }
}
