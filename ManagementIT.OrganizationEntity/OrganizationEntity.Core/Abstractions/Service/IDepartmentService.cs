using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Abstractions.Service
{
    public interface IDepartmentService
    {
        Task<OrganizationEntityActionResult<IEnumerable<DepartmentDTO>>> GetAllAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<DepartmentDTO>> GetByIdAsync(int id, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> AddAsync(DepartmentDTO model, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> UpdateAsync(DepartmentDTO model, ClaimsPrincipal principal);
        bool ExistEntityByName(string name, int? Tid = null);
        Task<OrganizationEntityActionResult> DeleteAsync(int deptId, ClaimsPrincipal principal);

        Task<OrganizationEntityActionResult<CreateOrEditDeptModel>> GetCreateDeptAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<CreateOrEditDeptModel>> GetUpdateDeptAsync(int deptId, ClaimsPrincipal principal);
    }
}
