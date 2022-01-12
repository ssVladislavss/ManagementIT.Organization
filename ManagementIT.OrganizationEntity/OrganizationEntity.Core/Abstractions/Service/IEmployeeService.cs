using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrganizationEntity.Core.Models.EmployeeModels;
using OrganizationEntity.Core.ResponseModels;

namespace OrganizationEntity.Core.Abstractions.Service
{
    public interface IEmployeeService
    {
        Task<OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>> GetAllAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<IEnumerable<EmployeeDTO>>> GetByDeptIdAsync(int deptId, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<EmployeeDTO>> GetByIdAsync(int employeeId, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<EmployeeDTO>> GetByUserNameAsync(string userName);

        Task<OrganizationEntityActionResult> AddAsync(EmployeeDTO model, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> UpdatePhoto(int employeeId, byte[] photo, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> UpdateAsync(EmployeeDTO model, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> DeleteAsync(int employeeId, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> DeletePhotoAsync(int employeeId, ClaimsPrincipal principal);

        Task<OrganizationEntityActionResult<CreateOrEditEmployeeDTO>> GetCreateAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<CreateOrEditEmployeeDTO>> GetUpdateAsync(int employeeId, ClaimsPrincipal principal);
    }
}
