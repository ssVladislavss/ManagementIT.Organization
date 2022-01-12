using OrganizationEntity.Core.Models.BuildingModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Abstractions.Service
{
    public interface IBuildingService
    {
        Task<OrganizationEntityActionResult<IEnumerable<BuildingDTO>>> GetAllAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<BuildingDTO>> GetByIdAsync(int buildingId, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> AddAsync(BuildingDTO model, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> UpdateAsync(BuildingDTO model, ClaimsPrincipal principal);
        bool ExistEntityByName(string name, int? Tid = null);
        Task<OrganizationEntityActionResult> DeleteAsync(int buildingId, ClaimsPrincipal principal);
    }
}
