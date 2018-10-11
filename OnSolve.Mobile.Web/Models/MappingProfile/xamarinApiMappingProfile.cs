using AutoMapper;
using OnSolve.Mobile.Web.Swn402Entities;

namespace OnSolve.Mobile.Web.Models
{
    public class XamarinApiMappingProfile : Profile
    {
        public XamarinApiMappingProfile()
        {     
           CreateMap<ContactDetail, ContactDetailModel>();            
        }
    }
}
