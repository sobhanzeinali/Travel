using MediatR;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Travel.Application.Common.Interfaces;
using AutoMapper;
using Travel.Application.Dtos.Tour;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Travel.Application.TourLists.Queries.GetTours
{
    public class GetToursQuery : IRequest<ToursVm>
    {
    }

    public class GetTourQueryHandler : IRequestHandler<GetToursQuery, ToursVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;

        public GetTourQueryHandler(IApplicationDbContext context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        public async Task<ToursVm> Handle(GetToursQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "GetTours";
            ToursVm toursLists;
            string serializedTourList;
            var redisTourLists = await _distributedCache.GetAsync(cacheKey, cancellationToken);
            if (redisTourLists == null)
            {
                toursLists = new ToursVm
                {
                    Lists = await _context.TourLists.ProjectTo<TourListDto>(_mapper.ConfigurationProvider)
                        .OrderBy(x => x.City).ToListAsync(cancellationToken)
                };

                serializedTourList = JsonConvert.SerializeObject(toursLists);
                redisTourLists = Encoding.UTF8.GetBytes(serializedTourList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                await _distributedCache.SetAsync(cacheKey, redisTourLists, options, cancellationToken);
                return toursLists;
            }

            serializedTourList = Encoding.UTF8.GetString(redisTourLists);
            toursLists = JsonConvert.DeserializeObject<ToursVm>(serializedTourList);
            return toursLists;
        }
    }
}