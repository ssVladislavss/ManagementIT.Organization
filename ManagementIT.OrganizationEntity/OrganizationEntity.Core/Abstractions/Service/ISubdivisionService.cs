using OrganizationEntity.Core.Models.SubdivisionModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Abstractions.Service
{
    public interface ISubdivisionService
    {
        Task<OrganizationEntityActionResult<IEnumerable<SubdivisionDTO>>> GetAllAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<SubdivisionDTO>> GetByIdAsync(int id, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> AddAsync(SubdivisionDTO model, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> UpdateAsync(SubdivisionDTO model, ClaimsPrincipal principal);
        bool ExistEntityByName(string name, int? Tid = null);
        Task<OrganizationEntityActionResult> DeleteAsync(int subId, ClaimsPrincipal principal);
    }
}
