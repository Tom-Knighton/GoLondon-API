using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;

namespace GoLondonAPI.Domain.Services
{
    public interface ILineService
    {
        Task<List<Disruption>> GetDisruptionsAsync(LineMode mode);
        Task<List<Line>> GetLineInfo(List<LineMode> modes, bool includeDetail = false);
        Task<List<Line>> GetLineInfo(List<string> lineIds, bool includeDetail = false);
        Task<LineModeGroupStatusType> GetGeneralLineStatus(List<LineMode> lineModes);
        Task<List<LineRoutes>> GetRoutesForLines(string[] lineIdentifiers, bool fixCoordinates = true);
    }
}

