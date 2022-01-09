using MediatR;

namespace Travel.Application.TourLists.Queries.ExportTours;

public class ExportToursQuery :IRequest<ExportToursVm>
{
    public int ListId { get; set; }
}