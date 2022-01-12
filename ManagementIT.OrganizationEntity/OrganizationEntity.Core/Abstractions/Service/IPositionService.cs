using OrganizationEntity.Core.Models.PositionModels;
using OrganizationEntity.Core.ResponseModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Abstractions.Service
{
    public interface IPositionService
    {
        Task<OrganizationEntityActionResult<IEnumerable<PositionDTO>>> GetAllAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<PositionDTO>> GetByIdAsync(int id, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> AddAsync(PositionDTO model, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> UpdateAsync(PositionDTO model, ClaimsPrincipal principal);
        bool ExistEntityByName(string name, int? Tid = null);
        Task<OrganizationEntityActionResult> DeleteAsync(int positionId, ClaimsPrincipal principal);
    }
}
