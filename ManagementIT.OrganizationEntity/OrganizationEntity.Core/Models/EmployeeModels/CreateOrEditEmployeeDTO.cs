using System.Collections.Generic;
using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.EmployeePhotoModels;
using OrganizationEntity.Core.Models.PositionModels;

namespace OrganizationEntity.Core.Models.EmployeeModels
{
    public class CreateOrEditEmployeeDTO : BaseEntity
    {
        public string Surname { get; set; }
        public override string Name { get; set; }
        public string Patronymic { get; set; }
        public DepartmentDTO Departament { get; set; }
        public PositionDTO Position { get; set; }
        public string WorkTelephone { get; set; }
        public string MobileTelephone { get; set; }
        public string Mail { get; set; }
        public string UserName { get; set; }
        public EmployeePhotoDTO Photo { get; set; }
        public int DepartamentId { get; set; }
        public int PositionId { get; set; }
        public int PhotoId { get; set; }
        public string Password { get; set; }

        public List<DepartmentDTO> SelectDepartment { get; set; }
        public List<PositionDTO> SelectPosition { get; set; }

        public CreateOrEditEmployeeDTO() { }

        public CreateOrEditEmployeeDTO(List<DepartmentDTO> depts, List<PositionDTO> positions, EmployeeDTO employee = null)
        {
            SelectDepartment = depts;
            SelectPosition = positions;
            if (employee != null)
            {
                Id = employee.Id;
                Surname = employee.Surname;
                Name = employee.Name;
                Patronymic = employee.Patronymic;
                MobileTelephone = employee.MobileTelephone;
                WorkTelephone = employee.WorkTelephone;
                Mail = employee.Mail;
                UserName = employee.UserName;
                DepartamentId = employee.Departament.Id;
                PositionId = employee.Position.Id;
                PhotoId = employee.PhotoId;
                Photo = employee.Photo;
            }
        }
    }
}