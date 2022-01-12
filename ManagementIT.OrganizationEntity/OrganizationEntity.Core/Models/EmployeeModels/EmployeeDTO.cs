using OrganizationEntity.Core.Models.DepartmentModels;
using OrganizationEntity.Core.Models.EmployeePhotoModels;
using OrganizationEntity.Core.Models.PositionModels;

namespace OrganizationEntity.Core.Models.EmployeeModels
{
    public class EmployeeDTO : BaseEntity
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

        public EmployeeDTO() { }

        public EmployeeDTO(string surName, string name, string patronymic, string workTelephone, string mobileTelephone, string mail, string userName,
            DepartmentDTO dept = null, PositionDTO position = null,
            EmployeePhotoDTO photo = null, int id = 0)
        {
            Id = id;
            Surname = surName;
            Name = name;
            Patronymic = patronymic;
            Departament = dept;
            Position = position;
            WorkTelephone = workTelephone;
            MobileTelephone = mobileTelephone;
            Mail = mail;
            UserName = userName;
            Photo = photo;
        }
        public EmployeeDTO(string surName, string name, string patronymic, string workTelephone, string mobileTelephone, string mail, string userName,
            int deptId, int positionId,
            int photoId = 0, int id = 0)
        {
            Id = id;
            Surname = surName;
            Name = name;
            Patronymic = patronymic;
            DepartamentId = deptId;
            PositionId = positionId;
            WorkTelephone = workTelephone;
            MobileTelephone = mobileTelephone;
            Mail = mail;
            UserName = userName;
            PhotoId = photoId;
        }

        public static EmployeeDTO GetEmployeeModel(string surName, string name, string patronymic,
            string workTelephone, string mobileTelephone, string mail, string userName,
            DepartmentDTO dept = null, PositionDTO position = null,
            EmployeePhotoDTO photo = null, int id = 0) =>
            new EmployeeDTO(surName, name, patronymic, workTelephone, mobileTelephone, mail, userName, dept, position,
                photo, id);
    }
}