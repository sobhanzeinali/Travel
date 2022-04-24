using Travel.Application.Common.Mappings;
using Travel.Domain.Entities;

namespace Travel.Application.Dtos.Tour;

public class TourListDto : IMapFrom<TourList>
{
    public TourListDto()
    {
        TourPackage = new List<TourPackageDto>();
    }

    public IList<TourPackageDto> TourPackage { get; set; }
    public int Id { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string About { get; set; }
}