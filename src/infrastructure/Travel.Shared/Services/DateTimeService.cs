using Travel.Application.Common.Interfaces;

namespace Travel.Shared.Services;

public class DateTimeServices : IDateTime
{
    public DateTime NowUtc => DateTime.UtcNow;
}