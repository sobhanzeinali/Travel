using MediatR;
using Microsoft.EntityFrameworkCore;
using Travel.Application.Common.Exceptions;
using Travel.Application.Common.Interfaces;
using Travel.Domain.Entities;

namespace Travel.Application.TourLists.Commands.UpdateTourList;

public class UpdateTourListCommand : IRequest
{
    public int Id { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string About { get; set; }
}

public class UpdateTourListCommandHandler : IRequestHandler<UpdateTourListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTourListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTourListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TourLists.Where(t => t.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (entity==null)
        {
            throw new NotFoundException(nameof(TourList), request.Id);
        }

        entity.City = request.City;
        entity.Country = request.Country;
        entity.About = request.About;
        _context.TourLists.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}