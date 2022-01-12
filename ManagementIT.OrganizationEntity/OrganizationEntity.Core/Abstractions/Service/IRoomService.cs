using OrganizationEntity.Core.Models.RoomModels;
using OrganizationEntity.Core.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationEntity.Core.Abstractions.Service
{
    public interface IRoomService
    {
        Task<OrganizationEntityActionResult<IEnumerable<RoomDTO>>> GetAllAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<RoomDTO>> GetByIdAsync(int roomId, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> AddAsync(RoomDTO model, ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult> UpdateAsync(RoomDTO model, ClaimsPrincipal principal);
        bool ExistEntityByName(string name, int? Tid = null);
        Task<OrganizationEntityActionResult> DeleteAsync(int roomId, ClaimsPrincipal principal);

        Task<OrganizationEntityActionResult<CreateOrEditRoomModel>> GetCreateRoomAsync(ClaimsPrincipal principal);
        Task<OrganizationEntityActionResult<CreateOrEditRoomModel>> GetUpdateRoomAsync(int roomId, ClaimsPrincipal principal);
    }
}
