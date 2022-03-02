using System;
using GoLondonAPI.Domain.Enums;

namespace GoLondonAPI.Domain.Services
{
    public interface IMetaService 
    {
        List<LineMode> GetLineModes();
        List<DisruptionDelayType> GetDelayTypes();
        Task<List<string>> GetAllLineIdsAsync(List<LineMode> modes);
    }
}

