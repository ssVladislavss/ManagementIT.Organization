using System.Security.Claims;
using System.Threading.Tasks;
using OrganizationEntity.Core.Models.ForApplicationModels;
using OrganizationEntity.Core.ResponseModels;

namespace OrganizationEntity.Core.Abstractions.Service
{
    public interface IDependencyEntityService
    {
        Task<OrganizationEntityActionResult<GetCreateOrUpdateApplicationDTO>> GetDependencyForApplication(ClaimsPrincipal principal);
    }
}