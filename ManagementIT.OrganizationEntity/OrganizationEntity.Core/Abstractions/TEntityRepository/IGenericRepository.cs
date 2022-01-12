using OrganizationEntity.Core.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Abstractions.TEntityRepository
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<OrganizationEntityActionResult<IEnumerable<T>>> GetAllEntitiesAsync(string iniciator);
        Task<OrganizationEntityActionResult<IEnumerable<T>>> GetEntitiesByNameAsync(string name, string iniciator);
        Task<OrganizationEntityActionResult<T>> GetEntityByNameAsync(string name, string iniciator);
        Task<OrganizationEntityActionResult<T>> GetEntityByIdAsync(int id, string iniciator);
        Task<OrganizationEntityActionResult> AddEntityAsync(T entity, string iniciator);
        Task<OrganizationEntityActionResult> UpdateEntityAsync(T entity, string iniciator);
        Task<OrganizationEntityActionResult> DeleteEntityAsync(T entity, string iniciator);
        Task<OrganizationEntityActionResult> DeleteRangeAsync(IEnumerable<T> entities, string iniciator);
        bool ExistEntityByName(string name, int? Tid = null);
    }
}