using System.Collections.Generic;
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.EmployeeModels;
using OrganizationEntity.Core.Models.RoomModels;

namespace OrganizationEntity.Core.Models.ForApplicationModels
{
    public class GetCreateOrUpdateApplicationDTO
    {
        public List<RoomDTO> SelectRoom { get; set; }
        public List<DepartmentDTO> SelectDepartment { get; set; }
        public List<EmployeeDTO> SelectEmployee { get; set; }

        public GetCreateOrUpdateApplicationDTO() { }

        public GetCreateOrUpdateApplicationDTO(List<RoomDTO> sekectRoom, List<DepartmentDTO> selectDept, List<EmployeeDTO> selectEmployee)
        {
            SelectDepartment = selectDept;
            SelectEmployee = selectEmployee;
            SelectRoom = sekectRoom;
        }
    }
}