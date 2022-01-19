using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travel.Application.Common.Interfaces;
using AutoMapper;
using Travel.Application.Dtos.Tour;
using AutoMapper.QueryableExtensions;

namespace Travel.Application.TourLists.Queries.GetTours
{
    public class GetToursQuery : IRequest<ToursVm>
    {
    }
    public class GetTourQueryHandler : IRequestHandler<GetToursQuery, ToursVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetTourQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ToursVm> Handle(GetToursQuery request, CancellationToken cancellationToken)
        {
            return new ToursVm
            {
                Lists = await _context.TourLists.ProjectTo<TourListDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.City).ToListAsync(cancellationToken)
            };
        }
    }
}
