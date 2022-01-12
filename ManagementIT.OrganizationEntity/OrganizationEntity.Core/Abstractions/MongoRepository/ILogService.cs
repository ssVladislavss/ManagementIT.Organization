using System.Collections.Generic;
using System.Threading.Tasks;
using OrganizationEntity.Core.Models.LogMessageModels;

namespace OrganizationEntity.Core.Abstractions.MongoRepository
{
    public interface ILogService
    {
        Task Create(LogMessage log);
        Task<LogMessage> GetAsync(string id);
        Task<IEnumerable<LogMessage>> GetLogsAsync(string? type, string? iniciator);
        Task DeleteRangeAsync();
        Task<bool> DeleteSelectedAsync(List<string> ids);
        Task<bool> DeleteAsync(string id);
    }
}