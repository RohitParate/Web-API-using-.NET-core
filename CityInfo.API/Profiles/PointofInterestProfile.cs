using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointofInterestProfile : Profile
    {
        public PointofInterestProfile() {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap<Models.PointOfInterestupdateDto, Entities.PointOfInterest>();
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestupdateDto>();
        }
    }
}
