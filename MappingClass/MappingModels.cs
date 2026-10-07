using AutoMapper;
using VendorMngSystem.Dtos;
using VendorMngSystem.Models;

namespace VendorMngSystem.MappingClass
{
    public class MappingModels:Profile
    {
        public MappingModels() 
        {

            

             CreateMap<VendorDocuments, VendorDocumentsDto>()
                .ForMember(dest => dest.VendorsDocumentId, opt => opt.MapFrom(src => src.VendorsDocumentId))
                .ForMember(dest => dest.VendorFileName, opt => opt.MapFrom(src => src.VendorFileName))
                .ForMember(dest => dest.VendorFilePath, opt => opt.MapFrom(src => src.VendorFilePath));

            CreateMap<VendorDocumentsDto, VendorDocuments>()
                .ForMember(dest => dest.vendors, opt => opt.Ignore());

            CreateMap<Vendors, VendorsDto>()
                .ForMember(dest => dest.ExistingDocuments, opt => opt.MapFrom(src => src.vendorDocuments));

            CreateMap<VendorsDto, Vendors>()
                .ForMember(dest => dest.vendorsCategories, opt => opt.Ignore())
                .ForMember(dest => dest.VendorsCategoryId, opt => opt.MapFrom(src => src.VendorsCategoryId));
                

       }
    }
}
